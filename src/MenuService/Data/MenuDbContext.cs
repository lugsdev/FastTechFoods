using MenuService.Models;
using Microsoft.EntityFrameworkCore;

namespace MenuService.Data
{
    public class MenuDbContext : DbContext
    {
        public MenuDbContext(DbContextOptions<MenuDbContext> options) : base(options)
        {
        }

        public DbSet<MenuItem> MenuItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MenuItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Price).IsRequired().HasPrecision(10, 2);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<MenuItem>().HasData(
                new MenuItem
                {
                    Id = 1,
                    Name = "Big Burger",
                    Description = "Hambúrguer artesanal com carne bovina, queijo, alface, tomate e molho especial",
                    Price = 25.90m,
                    Category = "Lanche",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow
                },
                new MenuItem
                {
                    Id = 2,
                    Name = "Chicken Deluxe",
                    Description = "Sanduíche de frango grelhado com queijo, alface e maionese temperada",
                    Price = 22.50m,
                    Category = "Lanche",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow
                },
                new MenuItem
                {
                    Id = 3,
                    Name = "Batata Frita Grande",
                    Description = "Porção generosa de batatas fritas crocantes",
                    Price = 12.90m,
                    Category = "Lanche",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow
                },
                new MenuItem
                {
                    Id = 4,
                    Name = "Coca-Cola 350ml",
                    Description = "Refrigerante Coca-Cola gelado",
                    Price = 5.50m,
                    Category = "Bebida",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow
                },
                new MenuItem
                {
                    Id = 5,
                    Name = "Suco de Laranja Natural",
                    Description = "Suco de laranja natural 300ml",
                    Price = 8.90m,
                    Category = "Bebida",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow
                },
                new MenuItem
                {
                    Id = 6,
                    Name = "Sorvete de Chocolate",
                    Description = "Sorvete cremoso de chocolate com calda",
                    Price = 9.90m,
                    Category = "Sobremesa",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}

