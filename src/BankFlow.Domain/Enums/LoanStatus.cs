namespace BankFlow.Domain.Enums
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public enum LoanStatus
    {
        Pending = 0, // Préstamo creado pero no aprobado
        Active = 1, // Desembolso y en curso
        PaidOff = 2, // Pagado completamente
        Defaulted = 3, //En mora
        Cancelled = 4 //Cancelado antes de desembolso
    }
}
