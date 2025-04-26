using System.Data.Common;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using SpotifyWebApi.Interface;
using SpotifyWebApi.Models;
// using SpotifyWebApi.Repositories;
using WorkProject.Auth.Handler;
using WorkProject.Auth.Interface;
using WorkProject.Auth.Service;
using WorkProject.GrpcClient;
using WorkProject.GrpcService;
using DbConnection = SpotifyWebApi.Database.DbConnection;

var builder = WebApplication.CreateBuilder(args);



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
// БД ПОТІМ НАЛОШТУЙ
// builder.Services.AddDbContext<DbConnection>(options =>
//     options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
//         new MySqlServerVersion(new Version(8, 0, 32)),
//         mySqlOptionsAction => mySqlOptionsAction.EnableRetryOnFailure()));


// //аутентифікації користувачів Spotify
// builder.Services.AddHttpClient<ISpotifyAccountService, SpotifyAccountRepos>(
//     c =>
//     {
//         c.BaseAddress = new Uri("https://accounts.spotify.com/api/");
//     });


builder.Services.AddGrpc();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<ITokenService, TokenService>();

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

builder.Services.AddTransient<AuthorizationHandler>();  

builder.Services.AddSingleton<NewReleasesGrpcClient>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var grpcServiceUrl = config["GrpcServiceUrl"];
    return new NewReleasesGrpcClient(grpcServiceUrl!);
});

builder.WebHost.ConfigureKestrel(options =>
{
    // Порт для HTTP/2 (gRPC)
    options.ListenLocalhost(5101, o => o.Protocols = HttpProtocols.Http1AndHttp2); 
    
    // Порт для HTTP/1.1 (Swagger)
    options.ListenLocalhost(5100, o => o.Protocols = HttpProtocols.Http1AndHttp2); 
});



// //Взаємодія з альбомами 
// builder.Services.AddHttpClient<ISpotifyAlbumsService, SpotifyAlbumRepos>(
//     c =>
//     {
//         c.BaseAddress = new Uri("https://api.spotify.com/v1/");
//         c.DefaultRequestHeaders.Add("Authorization", "application/.json");
//     });

var app = builder.Build();

app.MapGrpcService<NewReleasesGrpcService>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();