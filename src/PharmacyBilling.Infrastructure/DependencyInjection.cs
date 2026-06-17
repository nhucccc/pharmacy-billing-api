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
        // EF Core - hỗ trợ cả SQL Server và PostgreSQL
        var connStr = config.GetConnectionString("DefaultConnection")!;
        var usePostgres = connStr.Contains("postgresql://", StringComparison.OrdinalIgnoreCase)
                       || connStr.Contains("postgres://", StringComparison.OrdinalIgnoreCase)
                       || connStr.StartsWith("Host=", StringComparison.OrdinalIgnoreCase)
                       || connStr.Contains("host=", StringComparison.OrdinalIgnoreCase);

        // Convert postgresql:// URL sang Npgsql connection string nếu cần
        if (usePostgres && (connStr.StartsWith("postgresql://") || connStr.StartsWith("postgres://")))
        {
            var uri = new Uri(connStr);
            var userInfo = uri.UserInfo.Split(':');
            connStr = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
        }

        services.AddDbContext<PharmacyDbContext>(opts =>
        {
            if (usePostgres)
                opts.UseNpgsql(connStr, npgsql => npgsql.EnableRetryOnFailure(3));
            else
                opts.UseSqlServer(connStr, sql => sql.EnableRetryOnFailure(3));
        });

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
