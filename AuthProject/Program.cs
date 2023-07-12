using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApplication1.Database;
using WebAuthCommon;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//використовуємо GetSection для отримання розділу конфігурації з назвою "AuthOptions".
var authOptionsConfiguration = configuration.GetSection("AuthOption");

//для налаштування типу AuthOptions з використанням значень, отриманих з authOptionsConfiguration.
builder.Services.Configure<AuthOption>(authOptionsConfiguration);
builder.Services.AddScoped<AuthOption>();
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

app.UseAuthorization();

app.MapControllers();



app.Run();