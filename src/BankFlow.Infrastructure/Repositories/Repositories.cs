using BankFlow.Domain.Entities;
using BankFlow.Domain.Interfaces;
using BankFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankFlow.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly BankFlowDbContext _context;

    public CustomerRepository(BankFlowDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(int id)
        => await _context.Customers
            .Include(c => c.Loans)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Customer?> GetByDocumentNumberAsync(string documentNumber)
        => await _context.Customers
            .FirstOrDefaultAsync(c => c.DocumentNumber == documentNumber);

    public async Task<IEnumerable<Customer>> GetAllAsync()
        => await _context.Customers.ToListAsync();

    public async Task<Customer> AddAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
        return customer;
    }

    public void Update(Customer customer)
        => _context.Customers.Update(customer);
}

public class LoanRepository : ILoanRepository
{
    private readonly BankFlowDbContext _context;

    public LoanRepository(BankFlowDbContext context)
    {
        _context = context;
    }

    public async Task<Loan?> GetByIdAsync(int id)
        => await _context.Loans
            .Include(l => l.Customer)
            .Include(l => l.Payments)
            .FirstOrDefaultAsync(l => l.Id == id);

    public async Task<Loan?> GetByIdWithScheduleAsync(int id)
        => await _context.Loans
            .Include(l => l.Customer)
            .Include(l => l.Payments)
            .Include(l => l.Schedule.OrderBy(s => s.InstallmentNumber))
            .FirstOrDefaultAsync(l => l.Id == id);

    public async Task<IEnumerable<Loan>> GetByCustomerIdAsync(int customerId)
        => await _context.Loans
            .Include(l => l.Payments)
            .Where(l => l.CustomerId == customerId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<Loan>> GetAllAsync()
        => await _context.Loans
            .Include(l => l.Customer)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

    public async Task<Loan> AddAsync(Loan loan)
    {
        await _context.Loans.AddAsync(loan);
        return loan;
    }

    public void Update(Loan loan)
        => _context.Loans.Update(loan);
}

public class PaymentRepository : IPaymentRepository
{
    private readonly BankFlowDbContext _context;

    public PaymentRepository(BankFlowDbContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByIdAsync(int id)
        => await _context.Payments
            .Include(p => p.Loan)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IEnumerable<Payment>> GetByLoanIdAsync(int loanId)
        => await _context.Payments
            .Where(p => p.LoanId == loanId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();

    public async Task<Payment> AddAsync(Payment payment)
    {
        await _context.Payments.AddAsync(payment);
        return payment;
    }
}

public class LoanScheduleRepository : ILoanScheduleRepository
{
    private readonly BankFlowDbContext _context;

    public LoanScheduleRepository(BankFlowDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LoanSchedule>> GetByLoanIdAsync(int loanId)
        => await _context.LoanSchedules
            .Where(s => s.LoanId == loanId)
            .OrderBy(s => s.InstallmentNumber)
            .ToListAsync();

    public async Task AddRangeAsync(IEnumerable<LoanSchedule> schedules)
        => await _context.LoanSchedules.AddRangeAsync(schedules);

    public void Update(LoanSchedule schedule)
        => _context.LoanSchedules.Update(schedule);
}