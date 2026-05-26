namespace BankFlow.Application.DTOs
{
    using BankFlow.Domain.Enums;

    public class Dtos
    {
        public record CustomerDto(
            int Id,
            string FirstName,
            string LastName,
            string FullName,
            string DocumentNumber,
            string Email,
            string Phone,
            int CreditScore,
            int ActiveLoansCount
        );

        public record LoanDto
        {
            public int Id { get; init; }
            public int CustomerId { get; init; }
            public string CustomerName { get; init; } = string.Empty;
            public decimal Amount { get; init; }
            public decimal InterestRate { get; init; }
            public int TermMonths { get; init; }
            public string LoanType { get; init; } = string.Empty;
            public string Status { get; init; } = string.Empty;
            public decimal MonthlyPayment { get; init; }
            public decimal TotalPaid { get; init; }
            public decimal OutstandingBalance { get; init; }
            public DateTime StartDate { get; init; }
            public DateTime EndDate { get; init; }
        }

        public record LoanDetailDto
        {
            public int Id { get; init; }
            public int CustomerId { get; init; }
            public string CustomerName { get; init; } = string.Empty;
            public decimal Amount { get; init; }
            public decimal InterestRate { get; init; }
            public int TermMonths { get; init; }
            public string LoanType { get; init; } = string.Empty;
            public string Status { get; init; } = string.Empty;
            public decimal MonthlyPayment { get; init; }
            public decimal TotalPaid { get; init; }
            public decimal OutstandingBalance { get; init; }
            public DateTime StartDate { get; init; }
            public DateTime EndDate { get; init; }
            public List<PaymentDto> Payments { get; init; } = new();
            public List<LoanScheduleDto> Schedule { get; init; } = new();
        }

        public record PaymentDto(
            int Id,
            int LoanId,
            decimal Amount,
            decimal PrincipalAmount,
            decimal InterestAmount,
            DateTime PaymentDate,
            string? Reference
        );

        public record LoanScheduleDto(
            int InstallmentNumber,
            DateTime DueDate,
            decimal PrincipalAmount,
            decimal InterestAmount,
            decimal TotalAmount,
            decimal Balance,
            bool IsPaid
        );

        public record CreateCustomerDto(
            string FirstName,
            string LastName,
            string DocumentNumber,
            string Email,
            string Phone,
            int CreditScore
        );

        public record CreateLoanDto(
            int CustomerId,
            decimal Amount,
            decimal InterestRate,
            int TermMonths,
            LoanType LoanType
        );

        public record CreatePaymentDto(
            int LoanId,
            decimal Amount
        );
    }
}