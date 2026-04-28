using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using IndPubBack.Models;
using IndPubBack.DTO.Requests;
using IndPubBack.DTO.Responses;
using IndPubBack.Services.Interfaces;
using IndPubBack.Repositories.Interfaces;
using IndPubBack.Services.Implementations;
using IndPubBack.Repositories.Implementations;

var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowConfiguredOrigins", policy =>
    {
        policy.WithOrigins(allowedOrigins ?? Array.Empty<string>())
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Add Swagger for API documentation
builder.Services.AddSwaggerGen();

// SignalR and Data Protection
builder.Services.AddDataProtection();
builder.Services.AddSignalR();

// DI Container registrations for services and repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBookService, BookService>();

// Configure Entity Framework and SQL Server
builder.Services.AddDbContext<Connected>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("IndPubConnection")));

// Configure JWT Authentication and Authorization
builder.Services.AddAuthorization();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["AppSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["AppSettings:Audience"],
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:AccessToken"]!)),
            ValidateIssuerSigningKey = true
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/chatHub")))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.

// Enable Swagger UI in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable HTTPS redirection in production.
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowConfiguredOrigins");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();