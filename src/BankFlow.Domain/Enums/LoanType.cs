namespace BankFlow.Domain.Enums
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public enum LoanType
    {
        Personal = 0,
        Mortgage = 1, // Hipotecario
        vehicle = 2, // Vehiculo
        Education = 3, // Educativo
        Business = 4 // Empresarial
    }
}
