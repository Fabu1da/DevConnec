using Microsoft.EntityFrameworkCore;

namespace DevConnect.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<UserEntity> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Email).HasColumnName("email").IsRequired();
                entity.Property(e => e.Password).HasColumnName("password").IsRequired();
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Role).HasColumnName("role");
                entity.Property(e => e.Level).HasColumnName("level");
                entity.HasIndex(e => e.Email).IsUnique();
            });
        }
    }

    public class UserEntity
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Name { get; set; }
        public required string Role { get; set; }
        public required string Level { get; set; }
    }
}

    