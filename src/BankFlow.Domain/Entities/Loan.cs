namespace BankFlow.Domain.Entities
{
    using BankFlow.Domain.Enums;

    public class Loan
    {
        public int Id { get; private set; }
        public int CustomerId { get; private set; }
        public decimal Amount { get; private set; }
        public decimal InterestRate { get; private set; }
        public int TermMonths { get; private set; }
        public LoanType LoanType { get; private set; }
        public LoanStatus Status { get; private set; }
        public decimal MonthlyPayment { get; private set; }
        public decimal TotalPaid { get; private set; }
        public decimal OutstandingBalance { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public Customer Customer { get; private set; } = null!;
        public ICollection<Payment> Payments { get; private set; } = new List<Payment>();
        public ICollection<LoanSchedule> Schedule { get; private set; } = new List<LoanSchedule>();

        //Constructor for EF
        private Loan() { }

        //Constructor publico
        public Loan(int customerId, decimal amount, decimal interestRate, int termMonths, LoanType loanType)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
            if (interestRate < 0 || interestRate > 100)
                throw new ArgumentOutOfRangeException(nameof(interestRate), "Interest rate must be between 0 and 100.");
            if (termMonths <= 0 || termMonths > 360)
                throw new ArgumentOutOfRangeException(nameof(termMonths), "Term must be between 1 and 360 months.");

            CustomerId = customerId;
            Amount = amount;
            InterestRate = interestRate;
            TermMonths = termMonths;
            LoanType = loanType;
            Status = LoanStatus.Pending;
            OutstandingBalance = amount;
            TotalPaid = 0;
            StartDate = DateTime.UtcNow;
            EndDate = StartDate.AddMonths(termMonths);
            CreatedAt = DateTime.UtcNow;

            MonthlyPayment = CalculateMonthlyPayment();
        }

        public decimal CalculateMonthlyPayment()
        {
            // Si la tasa es 0, simplemente divide el monto entre los meses
            if (InterestRate == 0)
                return Math.Round(Amount / TermMonths, 2);

            // Convertir tasa anual a mensual: 12% anual → 0.01 mensual
            var monthlyRate = InterestRate / 100m / 12m;

            // (1 + r)^n
            var factor = (decimal)Math.Pow((double)(1 + monthlyRate), TermMonths);

            // M = P * [r * (1+r)^n] / [(1+r)^n - 1]
            var payment = Amount * (monthlyRate * factor) / (factor - 1);

            return Math.Round(payment, 2);
        }

        public List<LoanSchedule> GenerateAmortizationSchedule()
        {
            var schedule = new List<LoanSchedule>();
            var balance = Amount;
            var monthlyRate = InterestRate / 100m / 12m;
            var payment = MonthlyPayment;

            for (int i = 1; i <= TermMonths; i++)
            {
                // Los intereses se calculan sobre el saldo vigente
                var interestAmount = Math.Round(balance * monthlyRate, 2);

                // Lo que queda después de pagar intereses va a capital
                var principalAmount = payment - interestAmount;

                // Última cuota: ajustar para cubrir exactamente el saldo restante
                if (i == TermMonths)
                {
                    principalAmount = balance;
                    payment = principalAmount + interestAmount;
                }

                balance -= principalAmount;

                var installment = new LoanSchedule(
                    loanId: Id,
                    installmentNumber: i,
                    dueDate: StartDate.AddMonths(i),
                    principalAmount: principalAmount,
                    interestAmount: interestAmount,
                    balance: Math.Max(balance, 0)
                );

                schedule.Add(installment);
            }

            return schedule;
        }

        public void Activate()
        {
            if (Status != LoanStatus.Pending)
                throw new InvalidOperationException(
                    $"Cannot activate a loan with status {Status}.");

            Status = LoanStatus.Active;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ApplyPayment(decimal amount)
        {
            if (Status != LoanStatus.Active)
                throw new InvalidOperationException(
                    $"Cannot apply payment to a loan with status {Status}.");

            if (amount <= 0)
                throw new ArgumentException("Payment amount must be greater than zero.", nameof(amount));

            if (amount > OutstandingBalance)
                throw new ArgumentException("Payment amount exceeds outstanding balance.", nameof(amount));

            TotalPaid += amount;
            OutstandingBalance -= amount;

            if (OutstandingBalance <= 0)
            {
                OutstandingBalance = 0;
                Status = LoanStatus.PaidOff;
            }

            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if (Status == LoanStatus.PaidOff)
                throw new InvalidOperationException("Cannot cancel a paid-off loan.");

            Status = LoanStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
        }

    }
}
