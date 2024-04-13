using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using SpotifyWebApi.Interface;
using SpotifyWebApi.Models;
using SpotifyWebApi.Repositories;
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


builder.Services.AddHttpClient<ISpotifyAccountService, SpotifyAccountService>(
    c =>
    {
        c.BaseAddress = new Uri("https://accounts.spotify.com/api/");
    });

builder.Services.AddHttpClient<ISpotifyAlbumsService, SpotifyAlbumsService>(
    c =>
    {
        c.BaseAddress = new Uri("https://api.spotify.com/v1/");
        c.DefaultRequestHeaders.Add("Accept", "application/.json");
    });

var app = builder.Build();

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