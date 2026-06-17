using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PharmacyBilling.Application.Interfaces;
using PharmacyBilling.Domain.Interfaces;
using PharmacyBilling.Infrastructure.Data;
using PharmacyBilling.Infrastructure.Messaging;
using PharmacyBilling.Infrastructure.Persistence;
using PharmacyBilling.Infrastructure.Services;

namespace PharmacyBilling.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // EF Core
        services.AddDbContext<PharmacyDbContext>(opts =>
            opts.UseSqlServer(config.GetConnectionString("DefaultConnection"),
                sql => sql.EnableRetryOnFailure(3)));

        // Repositories & UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Application Services
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IMessagePublisher, RabbitMqPublisher>();
        services.AddHttpContextAccessor();

        // JWT Auth
        var jwtSecret = config["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret missing");
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                    ValidateIssuer = true,
                    ValidIssuer = config["Jwt:Issuer"] ?? "PharmacyBillingService",
                    ValidateAudience = true,
                    ValidAudience = config["Jwt:Audience"] ?? "PharmacyClients",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                opts.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = ctx =>
                    {
                        if (ctx.Exception is SecurityTokenExpiredException)
                            ctx.Response.Headers["Token-Expired"] = "true";
                        return Task.CompletedTask;
                    }
                };
            });

        // RabbitMQ Consumer Background Service
        services.AddHostedService<RabbitMqConsumer>();

        return services;
    }
}
