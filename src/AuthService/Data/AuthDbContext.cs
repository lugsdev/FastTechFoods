using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Cpf).HasMaxLength(14);
            });

            // Seed data - usuários iniciais
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Email = "gerente@fasttechfoods.com",
                    Name = "Gerente Sistema",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    Role = "Employee",
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = 2,
                    Email = "cozinha@fasttechfoods.com",
                    Name = "Equipe Cozinha",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    Role = "Employee",
                    CreatedAt = DateTime.UtcNow
                },
				new User
				{
					Id = 3,
					Email = "cliente@gmail.com",
					Name = "João Carlos",
					PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
					Role = "Customer",
					CreatedAt = DateTime.UtcNow
				}
			);
        }
    }
}

