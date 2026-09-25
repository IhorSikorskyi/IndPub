using Azure.Storage.Blobs;
using IndPubBack.Data;
using IndPubBack.Services.BackgroundSevices;
using Microsoft.EntityFrameworkCore;
using IndPubBack.Extensions;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddConfiguredCors(builder.Configuration);
builder.Services.AddSwaggerWithJwt();
builder.Services.AddJwtAuthentication(builder.Configuration);

// Configure Entity Framework and SQL Server
builder.Services.AddDbContext<IndPubDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("IndPubConnection")));

// SignalR and Data Protection
builder.Services.AddDataProtection();
builder.Services.AddSignalR();

// DI Container registrations for repositories
builder.Services.AddRepositoryServices();

// DI Container registrations for services
builder.Services.AddApplicationServices();

// Register the background service for cleaning up old refresh tokens
builder.Services.AddHostedService<RefreshTokenCleanupService>();
builder.Services.AddHostedService<UpdateBookRatingService>();

// Configure host options for concurrent service start and stop
builder.Services.Configure<HostOptions>(options =>
{
    options.ServicesStartConcurrently = true;
    options.ServicesStopConcurrently = true;
});

// Azure Blob Storage configuration
builder.Services.AddSingleton(_ => new BlobServiceClient(
    builder.Configuration.GetValue<string>("AzureStorage:ConnectionString")));

var app = builder.Build();

// Configure the HTTP request pipeline.
// Enable Swagger UI in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Conference Room Booking API v1");
    });
}

// Enable HTTPS redirection in production.
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowConfiguredOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();