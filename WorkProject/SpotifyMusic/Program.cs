using System.Data.Common;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using SpotifyWebApi.Models;
// using SpotifyWebApi.Repositories;
using WorkProject.Auth.Handler;
using WorkProject.Auth.Interface;
using WorkProject.Auth.Service;
using WorkProject.GrpcClient;
using WorkProject.GrpcClient.Albums;
using WorkProject.GrpcClient.Recommendations;
using WorkProject.GrpcService;
using WorkProject.GrpcService.Albums;
using WorkProject.GrpcService.Recommendations;
using DbConnection = SpotifyWebApi.Database.DbConnection;
using Microsoft.Extensions.Logging;
using DotNetEnv;
using MySqlConnector;
using WorkProject.Helpers;


var builder = WebApplication.CreateBuilder(args);

// --- завантажуємо ENV для локалки визначаємо середовище ---
var runningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

if (!runningInContainer)
    Env.Load(Path.Combine(builder.Environment.ContentRootPath, "Configuration.local.env"));

builder.Configuration.AddEnvironmentVariables();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();                                                      
        });
});

var cs = Environment.GetEnvironmentVariable("CONNECTIONSTRINGS__DEFAULTCONNECTION")
         ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(cs))
    throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection (ENV або appsettings).");

builder.Services.AddDbContext<DbConnection>(options =>
    options.UseMySql(cs, new MySqlServerVersion(new Version(8,0,36)), o => o.EnableRetryOnFailure()));

builder.Services.AddGrpc();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddScoped<JwtService>();

//HttpClient використовується для відправки запитів до якогось сервісу
builder.Services.AddHttpClient<NewReleasesGrpcService>(client =>
{
    //базова адресу для всіх запитів
    client.BaseAddress = new Uri("https://api.spotify.com/v1/");
    
    // Він отримує access token з ITokenService.
    // Додає його в заголовок Authorization: Bearer <access_token>.
    // Потім передає запит далі по ланцюжку.
}).AddHttpMessageHandler(provider => 
    new AuthorizationHandler(provider.GetRequiredService<ITokenService>()));

builder.Services.AddHttpClient<ToDoAlbumGrpcService>(client =>
{
    //базова адресу для всіх запитів
    client.BaseAddress = new Uri("https://api.spotify.com/v1/");
    
    // Він отримує access token з ITokenService.
    // Додає його в заголовок Authorization: Bearer <access_token>.
    // Потім передає запит далі по ланцюжку.
}).AddHttpMessageHandler(provider => 
    new AuthorizationHandler(provider.GetRequiredService<ITokenService>()));

builder.Services.AddHttpClient<RecommendationsGrpcService>(client =>
{
    //базова адресу для всіх запитів
    client.BaseAddress = new Uri("https://api.spotify.com/v1/");
    
    // Він отримує access token з ITokenService.
    // Додає його в заголовок Authorization: Bearer <access_token>.
    // Потім передає запит далі по ланцюжку.
}).AddHttpMessageHandler(provider => 
    new AuthorizationHandler(provider.GetRequiredService<ITokenService>()));

builder.Services.AddTransient<AuthorizationHandler>();  

builder.Services.AddSingleton<NewReleasesGrpcClient>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var grpcServiceUrl = config["GrpcServiceUrl"];
    return new NewReleasesGrpcClient(grpcServiceUrl!);
});

builder.Services.AddSingleton<ToDoAlbumGrpcClient>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var grpcServiceUrl = config["GrpcServiceUrl"];
    return new ToDoAlbumGrpcClient(grpcServiceUrl!);
});

// Реєструємо gRPC клієнт
builder.Services.AddGrpcClient<RecommendationsGrpcClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:RecommendationsServiceUrl"]!);
});

builder.WebHost.ConfigureKestrel(options =>
{
    if (runningInContainer)
    {
        // Порт для HTTP/2 (gRPC)
        options.ListenAnyIP(5101, o => o.Protocols = HttpProtocols.Http2);

        // Порт для HTTP/1.1 (Swagger)
        options.ListenAnyIP(5100, o => o.Protocols = HttpProtocols.Http1);
    }
    else
    {
        options.ListenLocalhost(5201, lo => lo.Protocols = HttpProtocols.Http2); // gRPC локально
        options.ListenLocalhost(5200, lo => lo.Protocols = HttpProtocols.Http1); // Swagger локально
    }
});

//Вмикає HTTP/2 без TLS (h2c) для клієнтського HttpClient/gRPC-клієнтів у цьому процесі.
//За замовчуванням .NET забороняє робити HTTP/2 по http:// (без HTTPS) — з міркувань безпеки.
//Цей свіч дозволяє.цей свіч впливає на клієнта, а не на Kestrel. Для самого сервера факт наявності свічa нічого не змінює.

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);


var app = builder.Build();

// редірект у контейнері вимикаємо (бо TLS не налаштований)
if (!runningInContainer) app.UseHttpsRedirection();

// Автоматичне оновлення БД
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("MigrationStartup");  
    
    MigrationChecker.CheckAndApplyMigrations<DbConnection>(scope.ServiceProvider, logger);
}

app.MapGrpcService<NewReleasesGrpcService>();
app.MapGrpcService<ToDoAlbumGrpcService>();
app.MapGrpcService<RecommendationsGrpcService>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();