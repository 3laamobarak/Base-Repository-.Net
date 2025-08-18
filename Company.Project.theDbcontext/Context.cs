using Company.Project.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Project.theDbcontext
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

            #endregion
            
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
        


        #endregion
        
    }
}
