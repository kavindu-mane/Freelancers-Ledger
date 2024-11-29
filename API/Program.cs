using System.Text;
using API.Data;
using API.Interfaces;
using API.Models;
using API.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

// Create the builder
var builder = WebApplication.CreateBuilder(args);

// Load .env file
DotNetEnv.Env.Load();

// Access environment variables
var ConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DBCon");
var ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
var ValidateAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
var IssuerSigningKey = Environment.GetEnvironmentVariable("JWT_SECRET");

// if any key is empty
if (string.IsNullOrEmpty(ConnectionString))
{
    throw new Exception(
        "Connection string not found. Ensure the .env file is correctly configured and placed in the root directory."
    );
}

if (string.IsNullOrEmpty(ValidIssuer))
{
    throw new Exception(
        "Valid Issuer not found. Ensure the .env file is correctly configured and placed in the root directory."
    );
}

if (string.IsNullOrEmpty(ValidateAudience))
{
    throw new Exception(
        "Validate Audience not found. Ensure the .env file is correctly configured and placed in the root directory."
    );
}

if (string.IsNullOrEmpty(IssuerSigningKey))
{
    throw new Exception(
        "Issuer Signing Key not found. Ensure the .env file is correctly configured and placed in the root directory."
    );
}

// Add services to the container.
builder.Services.AddControllers();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(ConnectionString));

// Identity
builder
    .Services.AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = true;
        options.SignIn.RequireConfirmedAccount = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// JWT Authentication
builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = ValidIssuer,
            ValidAudience = ValidateAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(IssuerSigningKey)),
        };
    });

// Add Endpoints API Explorer
builder.Services.AddEndpointsApiExplorer();

// Add Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Pleas enter valid JWT token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer",
        }
    );
    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                },
                new string[] { }
            },
        }
    );
});

// scoped services
builder.Services.AddScoped<ITokenService, TokenService>();

// Add Email Service
builder.Services.AddTransient<IEmailService, EmailService>();

var app = builder.Build();

// Add authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Add map controllers
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

// working message as json
app.MapGet("/", () => Results.Ok(new { message = "API is working" }));

app.Run();
