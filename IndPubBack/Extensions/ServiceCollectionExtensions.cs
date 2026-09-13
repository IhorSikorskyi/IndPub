using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;


namespace IndPubBack.Extensions;

public static class ServiceCollectionExtensions
{
    private const string BearerSchemeId = "Bearer";

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
    {
        var signingKey = config["AppSettings:AccessToken"]
                         ?? throw new InvalidOperationException("AppSettings:AccessToken не налаштовано.");

        services.AddAuthorization();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = config["AppSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = config["AppSettings:Audience"],
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
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

                // TODO: Create ITokenRevocationStore and TokenRevocationStore, then uncomment the following code to enable token revocation checks
                //options.Events = new JwtBearerEvents
                //{
                //    OnTokenValidated = async context =>
                //    {
                //        if (context.Principal != null)
                //        {
                //            var userId = context.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                //            var revocationStore = context.HttpContext.RequestServices
                //                .GetRequiredService<ITokenRevocationStore>();

                //            if (await revocationStore.IsRevokedAsync(userId))
                //            {
                //                context.Fail("Token revoked");
                //            }
                //        }
                //    }
                //};
            });
        return services;
    }

    public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Independent Publisher API",
                Version = "v1",
                Description = "API for managing independent publishing operations"
            });

            c.AddSecurityDefinition(BearerSchemeId, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter the JWT token in the format: Bearer {token}"
            });

            c.AddSecurityRequirement(document =>
            {
                var schemeRef = new OpenApiSecuritySchemeReference(BearerSchemeId, document);
                return new OpenApiSecurityRequirement
                {
                    [schemeRef] = new List<string>()
                };
            });

            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }

    public static IServiceCollection AddConfiguredCors(this IServiceCollection services, IConfiguration config)
    {
        var allowedOrigins = config
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>()
            ?.Select(o => o.TrimEnd('/'))
            .ToArray();

        if (allowedOrigins is null || allowedOrigins.Length == 0 || allowedOrigins.Any(o => o == "*"))
        {
            throw new InvalidOperationException("CORS: incorrect configuration of AllowedOrigins.");
        }

        services.AddCors(options =>
        {
            options.AddPolicy("AllowConfiguredOrigins", policy =>
            {
                policy.WithOrigins(allowedOrigins)
                    .WithMethods("GET", "POST", "PUT", "DELETE")
                    .WithHeaders("Content-Type", "Authorization");
            });
        });

        return services;
    }
}