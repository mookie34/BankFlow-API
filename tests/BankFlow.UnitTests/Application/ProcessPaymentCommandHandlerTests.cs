using AutoMapper;
using BankFlow.Application.Commands.ProcessPayment;
using BankFlow.Application.Mappings;
using BankFlow.Domain.Entities;
using BankFlow.Domain.Enums;
using BankFlow.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BankFlow.UnitTests.Application;

public class ProcessPaymentCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IMapper _mapper;
    private readonly ProcessPaymentCommandHandler _handler;

    public ProcessPaymentCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        var mapperConfig = new MapperConfiguration(cfg =>
            cfg.AddProfile<MappingProfile>());
        _mapper = mapperConfig.CreateMapper();

        _handler = new ProcessPaymentCommandHandler(_unitOfWorkMock.Object, _mapper);
    }

    private Loan CreateActiveLoan(decimal amount = 10_000_000m)
    {
        var loan = new Loan(1, amount, 12.0m, 12, LoanType.Personal);
        loan.Activate();
        return loan;
    }

    [Fact]
    public async Task Handle_WithValidPayment_ShouldProcessSuccessfully()
    {
        var loan = CreateActiveLoan();
        var command = new ProcessPaymentCommand(1, 888_489m);

        _unitOfWorkMock.Setup(u => u.Loans.GetByIdWithScheduleAsync(1))
            .ReturnsAsync(loan);

        _unitOfWorkMock.Setup(u => u.Payments.AddAsync(It.IsAny<Payment>()))
            .ReturnsAsync((Payment p) => p);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Amount.Should().Be(888_489m);
        _unitOfWorkMock.Verify(u => u.Payments.AddAsync(It.IsAny<Payment>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentLoan_ShouldThrowKeyNotFoundException()
    {
        var command = new ProcessPaymentCommand(999, 500_000m);

        _unitOfWorkMock.Setup(u => u.Loans.GetByIdWithScheduleAsync(999))
            .ReturnsAsync((Loan?)null);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*999*");
    }

    [Fact]
    public async Task Handle_WithInactiveLoan_ShouldThrowInvalidOperationException()
    {
        // Loan en Pending, no Active
        var loan = new Loan(1, 10_000_000m, 12.0m, 12, LoanType.Personal);
        var command = new ProcessPaymentCommand(1, 500_000m);

        _unitOfWorkMock.Setup(u => u.Loans.GetByIdWithScheduleAsync(1))
            .ReturnsAsync(loan);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Pending*");
    }

    [Fact]
    public async Task Handle_WithAmountExceedingBalance_ShouldThrow()
    {
        var loan = CreateActiveLoan(amount: 1_000_000m);
        var command = new ProcessPaymentCommand(1, 2_000_000m);

        _unitOfWorkMock.Setup(u => u.Loans.GetByIdWithScheduleAsync(1))
            .ReturnsAsync(loan);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*exceeds outstanding balance*");
    }

    [Fact]
    public async Task Handle_ShouldDistributePaymentBetweenPrincipalAndInterest()
    {
        // 10M al 12% anual = 1% mensual = 100,000 de intereses
        var loan = CreateActiveLoan();
        var command = new ProcessPaymentCommand(1, 888_489m);

        _unitOfWorkMock.Setup(u => u.Loans.GetByIdWithScheduleAsync(1))
            .ReturnsAsync(loan);

        Payment? capturedPayment = null;
        _unitOfWorkMock.Setup(u => u.Payments.AddAsync(It.IsAny<Payment>()))
            .Callback<Payment>(p => capturedPayment = p)
            .ReturnsAsync((Payment p) => p);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _handler.Handle(command, CancellationToken.None);

        capturedPayment.Should().NotBeNull();
        capturedPayment!.InterestAmount.Should().Be(100_000m);
        capturedPayment.PrincipalAmount.Should().Be(788_489m);
    }

    [Fact]
    public async Task Handle_WithPaymentLessThanInterest_ShouldApplyAllToInterest()
    {
        // Intereses son 100,000 pero el pago es solo 50,000
        var loan = CreateActiveLoan();
        var command = new ProcessPaymentCommand(1, 50_000m);

        _unitOfWorkMock.Setup(u => u.Loans.GetByIdWithScheduleAsync(1))
            .ReturnsAsync(loan);

        Payment? capturedPayment = null;
        _unitOfWorkMock.Setup(u => u.Payments.AddAsync(It.IsAny<Payment>()))
            .Callback<Payment>(p => capturedPayment = p)
            .ReturnsAsync((Payment p) => p);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _handler.Handle(command, CancellationToken.None);

        capturedPayment.Should().NotBeNull();
        capturedPayment!.InterestAmount.Should().Be(50_000m);
        capturedPayment.PrincipalAmount.Should().Be(0m);
    }
}