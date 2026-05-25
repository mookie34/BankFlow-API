using BankFlow.Domain.Entities;

namespace BankFlow.Domain.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(int id);
    Task<Customer?> GetByDocumentNumberAsync(string documentNumber);
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<Customer> AddAsync(Customer customer);
    void Update(Customer customer);
}

public interface ILoanRepository
{
    Task<Loan?> GetByIdAsync(int id);
    Task<Loan?> GetByIdWithScheduleAsync(int id);
    Task<IEnumerable<Loan>> GetByCustomerIdAsync(int customerId);
    Task<IEnumerable<Loan>> GetAllAsync();
    Task<Loan> AddAsync(Loan loan);
    void Update(Loan loan);
}

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(int id);
    Task<IEnumerable<Payment>> GetByLoanIdAsync(int loanId);
    Task<Payment> AddAsync(Payment payment);
}

public interface ILoanScheduleRepository
{
    Task<IEnumerable<LoanSchedule>> GetByLoanIdAsync(int loanId);
    Task AddRangeAsync(IEnumerable<LoanSchedule> schedules);
    void Update(LoanSchedule schedule);
}

public interface IUnitOfWork : IDisposable
{
    ICustomerRepository Customers { get; }
    ILoanRepository Loans { get; }
    IPaymentRepository Payments { get; }
    ILoanScheduleRepository LoanSchedules { get; }
    Task<int> SaveChangesAsync();
}