using Microsoft.EntityFrameworkCore;

using WebApplication1.Database;
using WebApplication1.Interface;
using WebApplication1.Models;
using WebApplication1.Services;
using WebAuthCommon;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

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

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<IPasswordService, PasswordService>();

//використовуємо GetSection для отримання розділу конфігурації з назвою "AuthTokenAcess".
var authOptionsConfiguration = configuration.GetSection("AuthOption");
var encryptConfiguration = configuration.GetSection("Encrypt");

//для налаштування типу AuthOptions з використанням значень, отриманих з authOptionsConfiguration.
builder.Services.Configure<AuthToken>(authOptionsConfiguration);
builder.Services.AddScoped<AuthToken>();

builder.Services.Configure<EncryptOptions>(encryptConfiguration);
builder.Services.AddScoped<EncryptOptions>();

builder.Services.AddDbContext<DBConnection>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 32)),
        mySqlOptionsAction => mySqlOptionsAction.EnableRetryOnFailure()));

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();   
app.UseAuthorization();

app.MapControllers();

app.Run();
