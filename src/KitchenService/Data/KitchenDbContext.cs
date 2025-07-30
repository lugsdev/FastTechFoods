using KitchenService.Model;
using Microsoft.EntityFrameworkCore;

namespace KitchenService.Data
{
	public class KitchenDbContext : DbContext
	{
		public KitchenDbContext(DbContextOptions<KitchenDbContext> options) : base(options)
		{
		}

		public DbSet<KitchenOrder> KitchenOrders { get; set; }
		public DbSet<KitchenOrderItem> KitchenOrderItems { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Configuração da entidade Order
			modelBuilder.Entity<KitchenOrder>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.CustomerId).IsRequired();
				entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(255);
				entity.Property(e => e.TotalAmount).IsRequired().HasPrecision(10, 2);
				entity.Property(e => e.DeliveryType).IsRequired().HasMaxLength(50);
				entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
				entity.Property(e => e.CancellationReason).HasMaxLength(500);

				// Relacionamento com OrderItems
				entity.HasMany(e => e.KitchenOrderItems)
					  .WithOne(e => e.KitchenOrder)
					  .HasForeignKey(e => e.OrderId)
					  .OnDelete(DeleteBehavior.Cascade);
			});

			// Configuração da entidade OrderItem
			modelBuilder.Entity<KitchenOrderItem>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.MenuItemId).IsRequired();
				entity.Property(e => e.MenuItemName).IsRequired().HasMaxLength(255);
				entity.Property(e => e.Quantity).IsRequired();
				entity.Property(e => e.UnitPrice).IsRequired().HasPrecision(10, 2);
				entity.Property(e => e.TotalPrice).IsRequired().HasPrecision(10, 2);
			});
		}

	}
}

