using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
        // ── Database ──────────────────────────────────────────────────────────
        var rawConn = config.GetConnectionString("DefaultConnection")
                      ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection missing");

        // Nếu là PostgreSQL URL (Railway) thì convert sang Npgsql format
        var isPostgres = rawConn.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase)
                      || rawConn.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase)
                      || rawConn.Contains("Host=", StringComparison.OrdinalIgnoreCase);

        var connStr = isPostgres && (rawConn.StartsWith("postgresql://") || rawConn.StartsWith("postgres://"))
            ? ConvertPostgresUrl(rawConn)
            : rawConn;

        services.AddDbContext<PharmacyDbContext>(opts =>
        {
            // Suppress PendingModelChanges warning - migration dùng SQL Server syntax
            // nhưng chạy được trên cả 2 DB nhờ EnsureCreated fallback
            opts.ConfigureWarnings(w =>
                w.Ignore(RelationalEventId.PendingModelChangesWarning));

            if (isPostgres)
                opts.UseNpgsql(connStr, npgsql => npgsql.EnableRetryOnFailure(3))
                    .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
            else
                opts.UseSqlServer(connStr, sql => sql.EnableRetryOnFailure(3))
                    .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
        });

        // ── Repositories & UnitOfWork ─────────────────────────────────────────
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // ── Application Services ──────────────────────────────────────────────
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IMessagePublisher, RabbitMqPublisher>();
        services.AddHttpContextAccessor();

        // ── JWT Authentication ────────────────────────────────────────────────
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

        // ── RabbitMQ Consumer ─────────────────────────────────────────────────
        services.AddHostedService<RabbitMqConsumer>();

        return services;
    }

    /// <summary>
    /// Convert postgresql://user:pass@host:port/db → Host=host;Port=port;Database=db;Username=user;Password=pass
    /// </summary>
    private static string ConvertPostgresUrl(string url)
    {
        var uri = new Uri(url);
        var userInfo = uri.UserInfo.Split(':');
        var user = userInfo.Length > 0 ? Uri.UnescapeDataString(userInfo[0]) : "";
        var pass = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
        var db   = uri.AbsolutePath.TrimStart('/');
        return $"Host={uri.Host};Port={uri.Port};Database={db};Username={user};Password={pass};SSL Mode=Require;Trust Server Certificate=true";
    }
}
