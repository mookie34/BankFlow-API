using BankFlow.Domain.Entities;
using FluentAssertions;

namespace BankFlow.UnitTests.Domain
{
    public class CustomerTests
    {
        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateCustomer()
        {
            //Arrange - prepara los datos

            //Act - ejecuta la acción
            var customer = new Customer("Santiago", "Mazo", "1234567890",
                "santiago@email.com", "+57319685742", 750);

            //Assert - verifica el resultado
            customer.FirstName.Should().Be("Santiago");
            customer.LastName.Should().Be("Mazo");
            customer.DocumentNumber.Should().Be("1234567890");
            customer.CreditScore.Should().Be(750);
            customer.FullName.Should().Be("Santiago Mazo");
        }

        [Fact]
        public void Constructor_WithEmptyFirstName_ShouldThrow()
        {
            //Arrange - prepara los datos
            //Act - ejecuta la acción
            Action act = () => new Customer("", "Mazo", "1234567890", "email@test.com", "123", 500);

            //Assert - verifica el resultado
            act.Should().Throw<ArgumentException>().WithMessage("*First name*");
        }

        [Fact]
        public void Constructor_WithEmptyDocumentNumber_ShouldThrow()
        {
            var act = () => new Customer("Santiago", "Mazo", "", "email@test.com", "123", 500);
            act.Should().Throw<ArgumentException>().WithMessage("*Document number*");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1001)]
        public void Constructor_WithInvalidCreditScore_ShouldThrow(int score)
        {
            var act = () => new Customer("Santiago", "Mazo", "123", "email@test.com", "123", score);
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData(200, 5_000_000, false)]    // Score muy bajo, no califica
        [InlineData(400, 5_000_000, true)]     // Score bajo, monto bajo, OK
        [InlineData(400, 15_000_000, false)]   // Score bajo, monto alto, NO
        [InlineData(600, 15_000_000, true)]    // Score medio, monto medio, OK
        [InlineData(600, 55_000_000, false)]   // Score medio, monto alto, NO
        [InlineData(800, 100_000_000, true)]   // Score alto, monto alto, OK
        public void IsEligibleForLoan_ShouldEvaluateCorrectly(int score, decimal amount, bool expected)
        {
            var customer = new Customer("Test", "User", "999", "test@test.com", "123", score);

            var result = customer.IsEligibleForLoan(amount);

            result.Should().Be(expected);
        }

        [Fact]
        public void UpdateContactInfo_ShouldUpdateFieldsAndTimestamp()
        {
            var customer = new Customer("Santiago", "Mazo", "123", "old@email.com", "000", 500);

            customer.UpdateContactInfo("new@email.com", "+573001234567");

            customer.Email.Should().Be("new@email.com");
            customer.Phone.Should().Be("+573001234567");
            customer.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void UpdateCreditScore_WithValidScore_ShouldUpdate()
        {
            var customer = new Customer("Santiago", "Mazo", "123", "email@test.com", "123", 500);

            customer.UpdateCreditScore(750);

            customer.CreditScore.Should().Be(750);
            customer.UpdatedAt.Should().NotBeNull();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1001)]
        public void UpdateCreditScore_WithInvalidScore_ShouldThrow(int newScore)
        {
            var customer = new Customer("Santiago", "Mazo", "123", "email@test.com", "123", 500);

            var act = () => customer.UpdateCreditScore(newScore);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

    }
}
