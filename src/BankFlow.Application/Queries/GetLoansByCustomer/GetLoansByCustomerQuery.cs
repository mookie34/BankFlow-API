using AutoMapper;
using BankFlow.Domain.Interfaces;
using MediatR;
using static BankFlow.Application.DTOs.Dtos;

namespace BankFlow.Application.Queries.GetLoansByCustomer;

public record GetLoansByCustomerQuery(int CustomerId) : IRequest<IEnumerable<LoanDto>>;

public class GetLoansByCustomerQueryHandler : IRequestHandler<GetLoansByCustomerQuery, IEnumerable<LoanDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetLoansByCustomerQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LoanDto>> Handle(GetLoansByCustomerQuery request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(request.CustomerId);
        if (customer is null)
            throw new KeyNotFoundException($"Customer with ID {request.CustomerId} not found.");

        var loans = await _unitOfWork.Loans.GetByCustomerIdAsync(request.CustomerId);
        return _mapper.Map<IEnumerable<LoanDto>>(loans);
    }
}