using System.Data.Common;
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

builder.Services.AddDbContext<DbConnection>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 32)),
        mySqlOptionsAction => mySqlOptionsAction.EnableRetryOnFailure()));


// //аутентифікації користувачів Spotify
// builder.Services.AddHttpClient<ISpotifyAccountService, SpotifyAccountRepos>(
//     c =>
//     {
//         c.BaseAddress = new Uri("https://accounts.spotify.com/api/");
//     });

builder.Services.AddGrpc();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddHttpClient<SpotifyAlbumsGrpcService>(client =>
{
    client.BaseAddress = new Uri("https://api.spotify.com/v1/");
}).AddHttpMessageHandler(provider => 
    new AuthorizationHandler(provider.GetRequiredService<ITokenService>()));

builder.Services.AddTransient<AuthorizationHandler>();  

builder.Services.AddSingleton<SpotifyAlbumsGrpcClient>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var grpcServiceUrl = config["GrpcServiceUrl"];
    return new SpotifyAlbumsGrpcClient(grpcServiceUrl!);
});

// //Взаємодія з альбомами 
// builder.Services.AddHttpClient<ISpotifyAlbumsService, SpotifyAlbumRepos>(
//     c =>
//     {
//         c.BaseAddress = new Uri("https://api.spotify.com/v1/");
//         c.DefaultRequestHeaders.Add("Authorization", "application/.json");
//     });

var app = builder.Build();

app.MapGrpcService<SpotifyAlbumsGrpcService>();
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