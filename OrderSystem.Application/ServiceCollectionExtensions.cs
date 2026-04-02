using Microsoft.Extensions.DependencyInjection;
using OrderSystem.Application.Services;

namespace OrderSystem.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<OrderService>();
        services.AddScoped<ProductService>();
        services.AddScoped<CouponService>();

        return services;
    }
}