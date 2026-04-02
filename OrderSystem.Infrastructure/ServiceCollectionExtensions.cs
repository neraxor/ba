using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderSystem.Application.Ports;
using OrderSystem.Infrastructure.External;
using OrderSystem.Infrastructure.Persistence;
using OrderSystem.Infrastructure.Repositories;

namespace OrderSystem.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        // DbContext
        services.AddDbContext<OrderSystemDbContext>(options =>
            options.UseSqlite(connectionString));

        // Repositories
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICouponRepository, CouponRepository>();

        // External Services
        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<IPaymentGateway, FakePaymentGateway>();

        return services;
    }
}