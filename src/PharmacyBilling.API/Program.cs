using System.Text.Json.Serialization;
using AspNetCoreRateLimit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PharmacyBilling.API.Middleware;
using PharmacyBilling.Application;
using PharmacyBilling.Infrastructure;
using Serilog;
using Serilog.Events;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File("logs/pharmacy-billing-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
    .CreateLogger();

try
{
    Log.Information("Starting PharmacyBilling API...");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    // ── Services ──────────────────────────────────────────────
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    builder.Services.AddControllers()
        .AddJsonOptions(opts =>
        {
            opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

    builder.Services.AddEndpointsApiExplorer();

    // ── Swagger ───────────────────────────────────────────────
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Pharmacy & Billing Service API",
            Version = "v1",
            Description = "Hệ thống quản lý kho thuốc và viện phí - Đề tài 05",
            Contact = new OpenApiContact { Name = "Nhóm 3", Email = "nhom3@pharmacy.com" }
        });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token. Example: eyJhbGci..."
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                Array.Empty<string>()
            }
        });

        // Include XML comments if available
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
    });

    // ── CORS ──────────────────────────────────────────────────
    builder.Services.AddCors(opts => opts.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

    // ── Rate Limiting ─────────────────────────────────────────
    builder.Services.AddMemoryCache();
    builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
    builder.Services.AddInMemoryRateLimiting();
    builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

    // ── Health Checks ─────────────────────────────────────────
    builder.Services.AddHealthChecks()
        .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!);

    var app = builder.Build();

    // ── Middleware Pipeline ───────────────────────────────────
    app.UseMiddleware<ValidationExceptionMiddleware>();

    if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pharmacy & Billing API v1");
            c.RoutePrefix = "swagger";
            c.DocumentTitle = "Pharmacy & Billing Service";
            c.DefaultModelsExpandDepth(-1);
        });
    }

    app.UseSerilogRequestLogging(opts =>
    {
        opts.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    });

    app.UseCors("AllowAll");
    app.UseIpRateLimiting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapHealthChecks("/health");

    // ── Auto-migrate on startup ───────────────────────────────
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<PharmacyBilling.Infrastructure.Data.PharmacyDbContext>();
        await context.Database.MigrateAsync();
        Log.Information("Database migration completed.");
    }

    Log.Information("PharmacyBilling API started on {Urls}", string.Join(", ", app.Urls));
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "PharmacyBilling API failed to start.");
}
finally
{
    Log.CloseAndFlush();
}
