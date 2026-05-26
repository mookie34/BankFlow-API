using AutoMapper;
using BankFlow.Domain.Entities;
using BankFlow.Domain.Enums;
using BankFlow.Domain.Interfaces;
using FluentValidation;
using MediatR;
using static BankFlow.Application.DTOs.Dtos;

namespace BankFlow.Application.Commands.CreateLoan
{
    // El Command: describe QUÉ quieres hacer
    public record CreateLoanCommand(
        int CustomerId,
        decimal Amount,
        decimal InterestRate,
        int TermMonths,
        LoanType LoanType
    ) : IRequest<LoanDto>;

    //Validator: valida los datos ANTES de ejecutar
    public class CreateLoanCommandValidator : AbstractValidator<CreateLoanCommand>
    {
        public CreateLoanCommandValidator()
        {
            RuleFor(x => x.CustomerId).GreaterThan(0).WithMessage("Customer ID is required.");
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Loan amount must be greater than zero.");
            RuleFor(x => x.InterestRate).InclusiveBetween(0, 100).WithMessage("Interest rate must be between 0% and 100%.");
            RuleFor(x => x.TermMonths).InclusiveBetween(1, 360).WithMessage("Term months must be between 1 and 360 months.");
            RuleFor(x => x.LoanType).IsInEnum().WithMessage("Invalid loan type.");
        }
    }

    //El Handler: ejecuta la lógica de negocio para cumplir el Command
    public class CreateLoanCommandHandler : IRequestHandler<CreateLoanCommand, LoanDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CreateLoanCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<LoanDto> Handle(CreateLoanCommand request, CancellationToken cancellationToken)
        {
            //1. Validar que el cliente exista
            var customer = await _unitOfWork.Customers.GetByIdAsync(request.CustomerId);
            if (customer == null)
                throw new KeyNotFoundException($"Customer with ID {request.CustomerId} not found.");

            //2. Validar que el cliente sea elegible para el préstamo
            if (!customer.IsEligibleForLoan(request.Amount))
            {
                throw new InvalidOperationException(
                    $"Customer is not eligible for a loan of {request.Amount:C} " +
                    $"Credit score: {customer.CreditScore}.");
            }

            //3. Crear la entidad Loan(prestamo)
            var loan = new Loan(
                customerId: request.CustomerId,
                amount: request.Amount,
                interestRate: request.InterestRate,
                termMonths: request.TermMonths,
                loanType: request.LoanType
                );

            //4. Guardar el préstamo
            await _unitOfWork.Loans.AddAsync(loan);
            await _unitOfWork.SaveChangesAsync();

            //5. Generar tabla de amortización
            var schedule = loan.GenerateAmortizationSchedule();
            await _unitOfWork.LoanSchedules.AddRangeAsync(schedule);

            //6. Activar el préstamo
            loan.Activate();
            _unitOfWork.Loans.Update(loan);
            await _unitOfWork.SaveChangesAsync();

            //7. Mapear a DTO y retornar
            var savedLoan = await _unitOfWork.Loans.GetByIdAsync(loan.Id);
            return _mapper.Map<LoanDto>(savedLoan);
        }
    }
}
