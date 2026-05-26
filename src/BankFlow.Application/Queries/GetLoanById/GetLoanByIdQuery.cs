using AutoMapper;
using BankFlow.Domain.Interfaces;
using MediatR;
using static BankFlow.Application.DTOs.Dtos;

namespace BankFlow.Application.Queries.GetLoanById
{


    public record GetLoanByIdQuery(int LoanId) : IRequest<LoanDetailDto>;
    public class GetLoanByIdQueryHandler : IRequestHandler<GetLoanByIdQuery, LoanDetailDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetLoanByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<LoanDetailDto> Handle(GetLoanByIdQuery request, CancellationToken cancellationToken)
        {
            var loan = await _unitOfWork.Loans.GetByIdWithScheduleAsync(request.LoanId);
            if (loan is null)
                throw new KeyNotFoundException($"Loan with ID {request.LoanId} not found.");
            return _mapper.Map<LoanDetailDto>(loan);
        }
    }

}
