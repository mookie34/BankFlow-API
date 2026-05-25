using BankFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankFlow.Infrastructure.Data.Configurations;

public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.ToTable("Loans");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(l => l.InterestRate)
            .IsRequired()
            .HasColumnType("decimal(5,2)");

        builder.Property(l => l.MonthlyPayment)
            .HasColumnType("decimal(18,2)");

        builder.Property(l => l.TotalPaid)
            .HasColumnType("decimal(18,2)");

        builder.Property(l => l.OutstandingBalance)
            .HasColumnType("decimal(18,2)");

        // Guardar el enum como texto legible, no como número
        builder.Property(l => l.LoanType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(l => l.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        // Un préstamo tiene muchos pagos
        builder.HasMany(l => l.Payments)
            .WithOne(p => p.Loan)
            .HasForeignKey(p => p.LoanId)
            .OnDelete(DeleteBehavior.Restrict);

        // Un préstamo tiene muchas cuotas (si borras el préstamo, se borran las cuotas)
        builder.HasMany(l => l.Schedule)
            .WithOne(s => s.Loan)
            .HasForeignKey(s => s.LoanId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}