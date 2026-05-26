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

        public record LoanDto(
            int Id,
            int CustomerId,
            string CustomerName,
            decimal Amount,
            decimal InterestRate,
            int TermMonths,
            string LoanType,
            string Status,
            decimal MonthlyPayment,
            decimal TotalPaid,
            decimal OutstandingBalance,
            DateTime StartDate,
            DateTime EndDate
        );

        public record LoanDetailDto(
            int Id,
            int CustomerId,
            string CustomerName,
            decimal Amount,
            decimal InterestRate,
            int TermMonths,
            string LoanType,
            string Status,
            decimal MonthlyPayment,
            decimal TotalPaid,
            decimal OutstandingBalance,
            DateTime StartDate,
            DateTime EndDate,
            List<PaymentDto> Payments,
            List<LoanScheduleDto> Schedule
        );

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