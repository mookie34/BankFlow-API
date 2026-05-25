namespace BankFlow.Infrastructure.Data
{
    using BankFlow.Domain.Entities;
    using Microsoft.EntityFrameworkCore;

    public class BankFlowDbContext: DbContext
    {
        public BankFlowDbContext(DbContextOptions<BankFlowDbContext> options) : base(options)
        {
        }

        //Cada DBSet representa una tabla en la base de datos
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Loan> Loans => Set<Loan>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<LoanSchedule> LoanSchedules => Set<LoanSchedule>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Carga todas las configuraciones de este ensamblado automáticamente
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BankFlowDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
