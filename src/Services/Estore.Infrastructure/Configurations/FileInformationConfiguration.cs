using EStore.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EStore.Infrastructure.Configurations;

public class FileInformationConfiguration : IEntityTypeConfiguration<FileInformation>
{
    public void Configure(EntityTypeBuilder<FileInformation> builder)
    {
        builder.Property(x => x.FileName).HasMaxLength(512).IsRequired();
        builder.Property(x => x.StorageFileName).HasMaxLength(1024).IsRequired();
        builder.Property(x => x.Url).HasMaxLength(512);
    }
}
