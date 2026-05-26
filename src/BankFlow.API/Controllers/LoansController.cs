using BankFlow.Application.Commands.CreateLoan;
using BankFlow.Application.Queries.GetLoanById;
using BankFlow.Application.Queries.GetLoansByCustomer;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static BankFlow.Application.DTOs.Dtos;

namespace BankFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly IMediator _mediator;

    public LoansController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<LoanDto>> CreateLoan(
        [FromBody] CreateLoanDto dto,
        [FromServices] IValidator<CreateLoanCommand> validator)
    {
        var command = new CreateLoanCommand(
            dto.CustomerId, dto.Amount, dto.InterestRate, dto.TermMonths, dto.LoanType);

        var validationResult = await validator.ValidateAsync(command);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));

        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LoanDetailDto>> GetById(int id)
    {
        try
        {
            var result = await _mediator.Send(new GetLoanByIdQuery(id));
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet("customer/{customerId:int}")]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetByCustomer(int customerId)
    {
        try
        {
            var result = await _mediator.Send(new GetLoansByCustomerQuery(customerId));
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}