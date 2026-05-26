namespace BankFlow.Domain.Entities
{
    using System;
    using System.Collections.Generic;

    public class Customer
    {
        public int Id { get; private set; }
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string DocumentNumber { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string Phone { get; private set; } = string.Empty;
        public int CreditScore { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public ICollection<Loan> Loans { get; private set; } = new List<Loan>();

        private Customer() { }

        public Customer(string firstName, string lastName, string documentNumber, string email, string phone, int creditScore)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name is required.", nameof(firstName));

            if (string.IsNullOrWhiteSpace(documentNumber))
                throw new ArgumentException("Document number is required.", nameof(documentNumber));

            if (creditScore < 0 || creditScore > 1000)
                throw new ArgumentOutOfRangeException(nameof(creditScore), "Credit score must be between 0 and 1000.");

            FirstName = firstName;
            LastName = lastName;
            DocumentNumber = documentNumber;
            Email = email;
            Phone = phone;
            CreditScore = creditScore;
            CreatedAt = DateTime.UtcNow;
        }

        public string FullName => $"{FirstName} {LastName}".Trim();
        public bool IsEligibleForLoan(decimal requestedAmount)
        {
            // Simple eligibility logic based on credit score and requested amount
            if (CreditScore < 300)
                return false;
            if (CreditScore < 500 && requestedAmount > 10_000_000m)
                return false;
            if (CreditScore < 700 && requestedAmount > 50_000_000m)
                return false;
            return true;
        }

        public void UpdateContactInfo(string email, string phone)
        {
            Email = email;
            Phone = phone;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateCreditScore(int newCreditScore)
        {
            if (newCreditScore < 0 || newCreditScore > 1000)
                throw new ArgumentOutOfRangeException(nameof(newCreditScore), "Credit score must be between 0 and 1000.");

            CreditScore = newCreditScore;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
