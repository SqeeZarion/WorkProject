using System.Data.Common;
using System.Text;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using SpotifyWebApi.Models;
using WorkProject.Auth.Handler;
using WorkProject.Auth.Interface;
using WorkProject.Auth.Service;
using WorkProject.GrpcService;
using WorkProject.GrpcService.Albums;
using WorkProject.GrpcService.Recommendations;
using DbConnection = SpotifyWebApi.Database.DbConnection;
using Microsoft.Extensions.Logging;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using Spotify.Recommendations;
using WorkProject.Helpers;

var builder = WebApplication.CreateBuilder(args);

// --- завантажуємо ENV для локальної розробки ---
// У контейнері ENV вже передаються через docker-compose/k8s,
// а локально беремо значення з файлу Configuration.local.env

var runningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
if (!runningInContainer)
    Env.Load(Path.Combine(builder.Environment.ContentRootPath, "Configuration.local.env"));

builder.Configuration.AddEnvironmentVariables();

// --- додаємо базові сервіси ---

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- CORS ---
// дозволяємо будь-які запити (для фронту/дебагу)

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

// --- БД ---
// рядок з ENV або appsettings.json

var cs = Environment.GetEnvironmentVariable("CONNECTIONSTRINGS__DEFAULTCONNECTION")
         ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(cs))
    throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection (ENV або appsettings).");

builder.Services.AddDbContext<DbConnection>(options =>
    options.UseMySql(cs, new MySqlServerVersion(new Version(8, 0, 36)), o => o.EnableRetryOnFailure()));

// --- gRPC + HttpClient ---

builder.Services.AddGrpc();
builder.Services.AddHttpClient();

// --- сервіси авторизації ---

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<AuthService>();           // логіка авторизації Spotify
builder.Services.AddScoped<ITokenService, TokenService>(); // оновлення/отримання Spotify токенів
builder.Services.AddScoped<JwtService>();               // генерація JWT токенів для клієнта
builder.Services.AddHttpContextAccessor();              // потрібен у AuthorizationHandler
builder.Services.AddScoped<AuthorizationHandler>();  // додає Spotify access_token у HttpClient

// --- HttpClient-и для роботи з Spotify API ---
builder.Services.AddHttpClient<NewReleasesGrpcService>(client =>
{
    // 📌 Базова адреса для всіх запитів до Spotify API
    client.BaseAddress = new Uri("https://api.spotify.com/v1/");
    
    // ⚙️ Додаткові пояснення:
    // - Цей HttpClient використовується, коли ToDoAlbumGrpcService робить запити до Spotify.
    // - Перед кожним запитом спрацьовує AuthorizationHandler.
    // - AuthorizationHandler дістає access_token з ITokenService.
    // - Потім додає заголовок Authorization: Bearer <access_token>.
    // - У результаті кожен запит цього HttpClient йде до Spotify від імені конкретного користувача.
}).AddHttpMessageHandler<AuthorizationHandler>();

builder.Services.AddHttpClient<ToDoAlbumGrpcService>(client =>
{
    client.BaseAddress = new Uri("https://api.spotify.com/v1/");
}).AddHttpMessageHandler<AuthorizationHandler>();

builder.Services.AddHttpClient<RecommendationsGrpcService>(client =>
{
    client.BaseAddress = new Uri("https://api.spotify.com/v1/");
}).AddHttpMessageHandler<AuthorizationHandler>();

// --- gRPC клієнти ---

// --- gRPC клієнти без враперів ---
// Recommendations
builder.Services.AddGrpcClient<Spotify.Recommendations.SpotifyRecommendationsService.SpotifyRecommendationsServiceClient>(options =>
{
    var url = builder.Configuration["GrpcSettings:RecommendationsServiceUrl"]
              ?? "http://localhost:5201";
    Console.WriteLine($"[gRPC] Using RecommendationsServiceUrl: {url}");
    options.Address = new Uri(url);
});

// Albums
builder.Services.AddGrpcClient<Spotify.ToDoAlbum.ToDoAlbumService.ToDoAlbumServiceClient>(options =>
{
    var url = builder.Configuration["GrpcSettings:AlbumServiceUrl"]
              ?? "http://localhost:5201";
    Console.WriteLine($"[gRPC] Using AlbumServiceUrl: {url}");
    options.Address = new Uri(url);
});

// New Releases
builder.Services.AddGrpcClient<Spotify.Newrelease.NewReleasesService.NewReleasesServiceClient>(options =>
{
    var url = builder.Configuration["GrpcSettings:ReleasesServiceUrl"]
              ?? "http://localhost:5201";
    Console.WriteLine($"[gRPC] Using ReleasesServiceUrl: {url}");
    options.Address = new Uri(url);
});


// --- Налаштування Kestrel (порти для Swagger/gRPC) ---
builder.WebHost.ConfigureKestrel(options =>
{
    if (runningInContainer)
    {
        options.ListenAnyIP(5101, o => o.Protocols = HttpProtocols.Http2); // gRPC
        options.ListenAnyIP(5100, o => o.Protocols = HttpProtocols.Http1); // Swagger
    }
    else
    {
        options.ListenLocalhost(5201, lo => lo.Protocols = HttpProtocols.Http2); // gRPC локально
        options.ListenLocalhost(5200, lo => lo.Protocols = HttpProtocols.Http1); // Swagger локально
    }
});

// Вмикає HTTP/2 без TLS (h2c) для клієнтів HttpClient/gRPC-клієнтів
// Інакше .NET за замовчуванням блокує HTTP/2 без HTTPS
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

// --- JWT Authentication ---
// використовується для перевірки JWT токенів у вхідних запитах
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

var app = builder.Build();

// --- Middleware pipeline ---
if (!runningInContainer) app.UseHttpsRedirection();

app.UseCors();           // політика CORS
app.UseAuthentication(); // перевірка JWT токенів
app.UseAuthorization();  // авторизація на основі claims/ролей

// --- Автоматичне оновлення БД (міграції) ---
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("MigrationStartup");

    MigrationChecker.CheckAndApplyMigrations<DbConnection>(scope.ServiceProvider, logger);
}

// Swagger тільки у dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// gRPC сервіси
app.MapGrpcService<NewReleasesGrpcService>();
app.MapGrpcService<ToDoAlbumGrpcService>();
app.MapGrpcService<RecommendationsGrpcService>();

// контролери (REST API)
app.MapControllers();

app.Run();
