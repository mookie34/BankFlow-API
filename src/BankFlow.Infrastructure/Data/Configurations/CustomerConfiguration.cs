using BankFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankFlow.Infrastructure.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.DocumentNumber)
            .IsRequired()
            .HasMaxLength(20);

        // Índice único: no pueden haber dos clientes con la misma cédula
        builder.HasIndex(c => c.DocumentNumber)
            .IsUnique();

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Phone)
            .HasMaxLength(20);

        builder.Property(c => c.CreditScore)
            .IsRequired();

        // Relación: un cliente tiene muchos préstamos
        builder.HasMany(c => c.Loans)
            .WithOne(l => l.Customer)
            .HasForeignKey(l => l.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // FullName es calculada, no se guarda en BD
        builder.Ignore(c => c.FullName);
    }
}