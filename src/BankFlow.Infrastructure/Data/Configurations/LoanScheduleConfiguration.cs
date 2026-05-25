using BankFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankFlow.Infrastructure.Data.Configurations;

public class LoanScheduleConfiguration : IEntityTypeConfiguration<LoanSchedule>
{
    public void Configure(EntityTypeBuilder<LoanSchedule> builder)
    {
        builder.ToTable("LoanSchedules");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.PrincipalAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.InterestAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.Balance)
            .HasColumnType("decimal(18,2)");

        // TotalAmount es calculada, no se guarda
        builder.Ignore(s => s.TotalAmount);

        // Índice compuesto: no pueden haber dos cuotas con el mismo número para un préstamo
        builder.HasIndex(s => new { s.LoanId, s.InstallmentNumber })
            .IsUnique();
    }
}