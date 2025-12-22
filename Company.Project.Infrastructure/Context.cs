using Company.Project.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Company.Project.Infrastructure
{
    public class Context : IdentityDbContext<ApplicationUser>
    {
        public Context() { }
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Seeding Data
            modelBuilder.Entity<ExampleClass>().HasData(
                new ExampleClass
                {
                    Id= 1,
                    Name = "Example 1",
                },
                new ExampleClass
                {
                    Id = 2,
                    Name = "Example 2",
                }
            );
            
            #endregion
            
            #region Filters
            modelBuilder.Entity<ExampleClass>(entity =>
            {
                entity.HasQueryFilter(c => !c.IsDeleted);
            });
            modelBuilder.Entity<ImageFile>(entity =>
            {
                entity.HasQueryFilter(c => !c.IsDeleted);
            });
            modelBuilder.Entity<ChatBotMessages>(entity =>
            {
                entity.HasQueryFilter(c => !c.IsDeleted);
            });

            #endregion
            
            modelBuilder.Entity<OTP>()
                .HasOne(otp => otp.User)
                .WithMany()
                .HasForeignKey(otp => otp.UserId);
            
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasIndex(e => e.Token).IsUnique();
                entity.HasOne(e => e.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<PaymentMethod>()
                .HasOne(pm => pm.User)
                .WithMany(u => u.PaymentMethods)
                .HasForeignKey(pm => pm.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<PaymentTransaction>()
                .HasOne(pt => pt.User)
                .WithMany(u => u.PaymentTransactions)
                .HasForeignKey(pt => pt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ChatBotMessages>()
                .HasOne(msg => msg.User)
                .WithMany(u => u.ChatBotMessages)
                .HasForeignKey(msg => msg.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            base.OnModelCreating(modelBuilder);

        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
        
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = null;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
        
        #region Dbsets
        public DbSet<ExampleClass> ExClass { get; set; }
        public DbSet<OTP> OTPs { get; set; }
        public DbSet<ImageFile> ImageFiles { get; set; }
        public DbSet<ChatBotMessages> ChatBotMessages { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }= null!;
        public DbSet<PaymentMethod> PaymentMethods { get; set; }= null!;
        

        #endregion
        
    }
}
