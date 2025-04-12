using EStore.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EStore.Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(x => x.OrderId)
            .IsRequired()
            .HasMaxLength(36);

        builder.Property(x => x.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.Method)
            .IsRequired()
            .HasConversion<string>();

        // VNPay Payment Information
        builder.Property(x => x.VnpTransactionCode)
            .HasMaxLength(50);

        builder.Property(x => x.VnpBankCode)
            .HasMaxLength(20);

        builder.Property(x => x.VnpPayDate)
            .HasMaxLength(14);

        builder.Property(x => x.VnpResponseCode)
            .HasMaxLength(2);

        builder.Property(x => x.VnpSecureHash)
            .HasMaxLength(256);

        builder.Property(x => x.VnpTxnRef)
            .HasMaxLength(100);

        builder.Property(x => x.VnpOrderInfo)
            .HasMaxLength(255);

        builder.Property(x => x.VnpBankTransactionId)
            .HasMaxLength(50);

        builder.Property(x => x.VnpIpAddress)
            .HasMaxLength(45);

        // Indexes
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.OrderId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.Method);
        builder.HasIndex(x => x.CreatedAt);
    }
} 