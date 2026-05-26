using AutoMapper;
using BankFlow.Domain.Entities;
using static BankFlow.Application.DTOs.Dtos;

namespace BankFlow.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Customer, CustomerDto>()
                .ForMember(d => d.ActiveLoansCount,
                opt => opt.MapFrom(s => s.Loans.Count(l =>
                l.Status == Domain.Enums.LoanStatus.Active)));

            CreateMap<Loan, LoanDto>()
                .ForMember(d => d.CustomerName,
                    opt => opt.MapFrom(s => s.Customer.FullName))
                .ForMember(d => d.LoanType,
                    opt => opt.MapFrom(s => s.LoanType.ToString()))
                .ForMember(d => d.Status,
                    opt => opt.MapFrom(s => s.Status.ToString()));

            CreateMap<Loan, LoanDetailDto>()
                .ForMember(d => d.CustomerName,
                    opt => opt.MapFrom(s => s.Customer.FullName))
                .ForMember(d => d.LoanType,
                    opt => opt.MapFrom(s => s.LoanType.ToString()))
                .ForMember(d => d.Status,
                    opt => opt.MapFrom(s => s.Status.ToString()));

            CreateMap<Payment, PaymentDto>();
            CreateMap<LoanSchedule, LoanScheduleDto>();

        }
    }
}
