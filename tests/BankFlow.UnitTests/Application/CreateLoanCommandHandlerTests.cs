using AutoMapper;
using BankFlow.Application.Commands.CreateLoan;
using BankFlow.Application.Mappings;
using BankFlow.Domain.Entities;
using BankFlow.Domain.Enums;
using BankFlow.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BankFlow.UnitTests.Application;

public class CreateLoanCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IMapper _mapper;
    private readonly CreateLoanCommandHandler _handler;

    public CreateLoanCommandHandlerTests()
    {
        // Crear el mock de IUnitOfWork
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        // AutoMapper real (no necesita mock, es solo mapeo)
        var mapperConfig = new MapperConfiguration(cfg =>
            cfg.AddProfile<MappingProfile>());
        _mapper = mapperConfig.CreateMapper();

        // El handler recibe el mock y el mapper real
        _handler = new CreateLoanCommandHandler(_unitOfWorkMock.Object, _mapper);
    }

    private Customer CreateValidCustomer(int creditScore = 750)
    {
        return new Customer("Santiago", "Mazo", "1234567890",
            "santiago@email.com", "+573196760512", creditScore);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateLoan()
    {
        // Arrange - configurar el mock
        var customer = CreateValidCustomer();
        var command = new CreateLoanCommand(1, 10_000_000m, 12.0m, 12, LoanType.Personal);

        // Cuando el handler pida el cliente con ID 1, devuelve nuestro customer
        _unitOfWorkMock.Setup(u => u.Customers.GetByIdAsync(1))
            .ReturnsAsync(customer);

        // Cuando guarde un loan, devuélvelo tal cual
        _unitOfWorkMock.Setup(u => u.Loans.AddAsync(It.IsAny<Loan>()))
            .ReturnsAsync((Loan l) => l);

        // Cuando guarde las cuotas, no hagas nada
        _unitOfWorkMock.Setup(u => u.LoanSchedules.AddRangeAsync(It.IsAny<IEnumerable<LoanSchedule>>()))
            .Returns(Task.CompletedTask);

        // Cuando recargue el loan, devuelve uno nuevo
        _unitOfWorkMock.Setup(u => u.Loans.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) =>
            {
                return new Loan(1, 10_000_000m, 12.0m, 12, LoanType.Personal);
            });

        // SaveChangesAsync devuelve 1 (una fila afectada)
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Amount.Should().Be(10_000_000m);

        // Verificar que SÍ se llamaron los métodos esperados
        _unitOfWorkMock.Verify(u => u.Loans.AddAsync(It.IsAny<Loan>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.LoanSchedules.AddRangeAsync(
            It.IsAny<IEnumerable<LoanSchedule>>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_WithNonExistentCustomer_ShouldThrowKeyNotFoundException()
    {
        var command = new CreateLoanCommand(999, 10_000_000m, 12.0m, 12, LoanType.Personal);

        // Cuando busque el cliente 999, devuelve null (no existe)
        _unitOfWorkMock.Setup(u => u.Customers.GetByIdAsync(999))
            .ReturnsAsync((Customer?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*999*");
    }

    [Fact]
    public async Task Handle_WithLowCreditScore_ShouldThrowInvalidOperationException()
    {
        var customer = CreateValidCustomer(creditScore: 200);
        var command = new CreateLoanCommand(1, 10_000_000m, 12.0m, 12, LoanType.Personal);

        _unitOfWorkMock.Setup(u => u.Customers.GetByIdAsync(1))
            .ReturnsAsync(customer);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*not eligible*");
    }

    [Fact]
    public async Task Handle_WithMidCreditScoreAndHighAmount_ShouldThrow()
    {
        // Score 400 no puede pedir más de 10 millones
        var customer = CreateValidCustomer(creditScore: 400);
        var command = new CreateLoanCommand(1, 15_000_000m, 12.0m, 12, LoanType.Personal);

        _unitOfWorkMock.Setup(u => u.Customers.GetByIdAsync(1))
            .ReturnsAsync(customer);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*not eligible*");
    }
}