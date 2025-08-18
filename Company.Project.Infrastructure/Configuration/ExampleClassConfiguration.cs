using Company.Project.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Company.Project.Infrastructure.Configuration
{
    public class ExampleClassConfiguration : IEntityTypeConfiguration<ExampleClass>
    {
        public void Configure(EntityTypeBuilder<ExampleClass> builder)
        {
            builder.HasKey(c=>c.Id);
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
