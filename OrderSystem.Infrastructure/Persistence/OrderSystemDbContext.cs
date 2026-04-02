using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OrderSystem.Domain.Entities;
using OrderSystem.Domain.ValueObjects;

namespace OrderSystem.Infrastructure.Persistence;

public class OrderSystemDbContext : DbContext
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Coupon> Coupons => Set<Coupon>();

    public OrderSystemDbContext(DbContextOptions<OrderSystemDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Customer
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(200);
            entity.Property(c => c.Email)
                .HasConversion(new EmailConverter())
                .HasMaxLength(254);
            entity.HasIndex(c => c.Email).IsUnique();
        });

        // Product
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Description).HasMaxLength(1000);
            entity.Property(p => p.Price)
                .HasConversion(new MoneyConverter())
                .HasColumnName("Price");
        });

        // Order
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Status).HasConversion<string>();
            entity.Property(o => o.Discount)
                .HasConversion(new NullableMoneyConverter())
                .HasColumnName("Discount");
            entity.HasMany(o => o.Items).WithOne().HasForeignKey("OrderId");
            entity.Metadata.FindNavigation(nameof(Order.Items))!.SetPropertyAccessMode(PropertyAccessMode.Field);
            entity.Ignore(o => o.Subtotal);
            entity.Ignore(o => o.Total);
            entity.Ignore(o => o.TotalItemCount);
        });

        // OrderItem
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(oi => oi.Id);
            entity.Property(oi => oi.ProductName).IsRequired().HasMaxLength(200);
            entity.Property(oi => oi.UnitPrice)
                .HasConversion(new MoneyConverter())
                .HasColumnName("UnitPrice");
            entity.Ignore(oi => oi.TotalPrice);
        });

        // Coupon
        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Code).IsRequired().HasMaxLength(20);
            entity.HasIndex(c => c.Code).IsUnique();
            entity.Property(c => c.DiscountAmount)
                .HasConversion(new MoneyConverter())
                .HasColumnName("DiscountAmount");
            entity.Property(c => c.MinimumOrderValue)
                .HasConversion(new NullableMoneyConverter())
                .HasColumnName("MinimumOrderValue");
            entity.Ignore(c => c.IsActive);
        });
    }

    private class EmailConverter : ValueConverter<Email, string>
    {
        public EmailConverter() : base(
            e => e.Value,
            s => new Email(s))
        {
        }
    }

    private class MoneyConverter : ValueConverter<Money, string>
    {
        public MoneyConverter() : base(
            m => $"{m.Amount}|{m.Currency}",
            s => ParseMoney(s))
        {
        }

        private static Money ParseMoney(string s)
        {
            var parts = s.Split('|');
            return new Money(decimal.Parse(parts[0]), parts[1]);
        }
    }

    private class NullableMoneyConverter : ValueConverter<Money?, string?>
    {
        public NullableMoneyConverter() : base(
            m => m.HasValue ? $"{m.Value.Amount}|{m.Value.Currency}" : null,
            s => string.IsNullOrEmpty(s) ? null : ParseMoney(s))
        {
        }

        private static Money ParseMoney(string s)
        {
            var parts = s.Split('|');
            return new Money(decimal.Parse(parts[0]), parts[1]);
        }
    }
}