using BankFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankFlow.Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.PrincipalAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.InterestAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Reference)
            .HasMaxLength(50);
    }
}