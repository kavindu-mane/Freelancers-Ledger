using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using NeonEfExample.Data;

var builder = WebApplication.CreateBuilder(args);

// Load .env file
DotNetEnv.Env.Load();

// Access environment variables
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DBCon");

if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception(
        "Connection string not found. Ensure the .env file is correctly configured and placed in the root directory."
    );
}

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.Run();
