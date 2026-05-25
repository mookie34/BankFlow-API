using BankFlow.Domain.Interfaces;
using BankFlow.Infrastructure.Data;

namespace BankFlow.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly BankFlowDbContext _context;
    private ICustomerRepository? _customers;
    private ILoanRepository? _loans;
    private IPaymentRepository? _payments;
    private ILoanScheduleRepository? _loanSchedules;

    public UnitOfWork(BankFlowDbContext context)
    {
        _context = context;
    }

    // Lazy initialization: solo se crea el repositorio cuando lo pides
    public ICustomerRepository Customers
        => _customers ??= new CustomerRepository(_context);

    public ILoanRepository Loans
        => _loans ??= new LoanRepository(_context);

    public IPaymentRepository Payments
        => _payments ??= new PaymentRepository(_context);

    public ILoanScheduleRepository LoanSchedules
        => _loanSchedules ??= new LoanScheduleRepository(_context);

    public async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();

    public void Dispose()
        => _context.Dispose();
}