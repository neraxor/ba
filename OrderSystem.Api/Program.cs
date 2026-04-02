using OrderSystem.Api;
using OrderSystem.Application;
using OrderSystem.Domain.Entities;
using OrderSystem.Domain.ValueObjects;
using OrderSystem.Infrastructure;
using OrderSystem.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add exception handler
builder.Services.AddExceptionHandler<DomainExceptionHandler>();
builder.Services.AddProblemDetails();

// Add application & infrastructure
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=ordersystem.db");

var app = builder.Build();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderSystemDbContext>();
    db.Database.EnsureCreated();

    // Seed data if empty
    if (!db.Customers.Any())
    {
        var customer1 = new Customer("Max Mustermann", "max@example.com");
        var customer2 = new Customer("Erika Musterfrau", "erika@example.com");
        db.Customers.AddRange(customer1, customer2);

        var product1 = new Product("Laptop", "Gaming Laptop 15 Zoll", new Money(999.99m), 10);
        var product2 = new Product("Maus", "Wireless Gaming Maus", new Money(49.99m), 50);
        var product3 = new Product("Tastatur", "Mechanische Tastatur", new Money(129.99m), 25);
        db.Products.AddRange(product1, product2, product3);

        var coupon = new Coupon("RABATT10", new Money(10m), DateTime.UtcNow, DateTime.UtcNow.AddYears(1), 100);
        db.Coupons.Add(coupon);

        db.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();