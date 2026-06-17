using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PharmacyBilling.Application.Services;

namespace PharmacyBilling.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<AuthService>();
        services.AddScoped<MedicineService>();
        services.AddScoped<DispensationService>();
        services.AddScoped<InvoiceService>();
        services.AddScoped<UserService>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
