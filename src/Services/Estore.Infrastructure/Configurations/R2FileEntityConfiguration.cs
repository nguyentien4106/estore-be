using EStore.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EStore.Infrastructure.Configurations;

public class R2FileEntityConfiguration : IEntityTypeConfiguration<R2FileEntity>
{
    public void Configure(EntityTypeBuilder<R2FileEntity> builder)
    {
        builder.HasKey(x => x.Id);
        
        // Create index on UserId column
        builder.HasIndex(x => x.UserId, "IX_R2FileEntity_UserId");
        
        // Set specific lengths for string properties
        builder.Property(x => x.FileName).IsRequired().HasMaxLength(255);
        builder.Property(x => x.ContentType).IsRequired().HasMaxLength(100);
        builder.Property(x => x.FileKey).IsRequired().HasMaxLength(1000);
    }
}