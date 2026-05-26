using AutoMapper;
using BankFlow.Domain.Entities;
using BankFlow.Domain.Enums;
using BankFlow.Domain.Interfaces;
using FluentValidation;
using MediatR;
using static BankFlow.Application.DTOs.Dtos;

namespace BankFlow.Application.Commands.ProcessPayment;

// Command
public record ProcessPaymentCommand(
    int LoanId,
    decimal Amount
) : IRequest<PaymentDto>;

// Validator
public class ProcessPaymentCommandValidator : AbstractValidator<ProcessPaymentCommand>
{
    public ProcessPaymentCommandValidator()
    {
        RuleFor(x => x.LoanId)
            .GreaterThan(0).WithMessage("Loan ID is required.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Payment amount must be greater than zero.");
    }
}

// Handler
public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, PaymentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProcessPaymentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaymentDto> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        // 1. Buscar el préstamo con su tabla de amortización
        var loan = await _unitOfWork.Loans.GetByIdWithScheduleAsync(request.LoanId);
        if (loan is null)
            throw new KeyNotFoundException($"Loan with ID {request.LoanId} not found.");

        if (loan.Status != LoanStatus.Active)
            throw new InvalidOperationException(
                $"Cannot process payment for a loan with status {loan.Status}.");

        if (request.Amount > loan.OutstandingBalance)
            throw new InvalidOperationException(
                $"Payment amount ({request.Amount:C}) exceeds outstanding balance ({loan.OutstandingBalance:C}).");

        // 2. Calcular distribución entre capital e intereses
        var monthlyRate = loan.InterestRate / 100m / 12m;
        var interestAmount = Math.Round(loan.OutstandingBalance * monthlyRate, 2);
        var principalAmount = request.Amount - interestAmount;

        // Si el pago es menor que los intereses, todo va a intereses
        if (principalAmount < 0)
        {
            interestAmount = request.Amount;
            principalAmount = 0;
        }

        // 3. Crear el registro de pago
        var payment = new Payment(
            loanId: request.LoanId,
            amount: request.Amount,
            principalAmount: principalAmount,
            interestAmount: interestAmount,
            reference: $"PAY-{DateTime.UtcNow:yyyyMMddHHmmss}"
        );

        await _unitOfWork.Payments.AddAsync(payment);

        // 4. Aplicar el pago al préstamo (actualiza saldo y estado)
        loan.ApplyPayment(principalAmount);
        _unitOfWork.Loans.Update(loan);

        // 5. Marcar la siguiente cuota como pagada
        var nextInstallment = loan.Schedule
            .Where(s => !s.IsPaid)
            .OrderBy(s => s.InstallmentNumber)
            .FirstOrDefault();

        if (nextInstallment is not null)
        {
            nextInstallment.MarkAsPaid();
            _unitOfWork.LoanSchedules.Update(nextInstallment);
        }

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PaymentDto>(payment);
    }
}