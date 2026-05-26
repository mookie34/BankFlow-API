using BankFlow.Application.Commands.ProcessPayment;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static BankFlow.Application.DTOs.Dtos;

namespace BankFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<PaymentDto>> ProcessPayment(
        [FromBody] CreatePaymentDto dto,
        [FromServices] IValidator<ProcessPaymentCommand> validator)
    {
        var command = new ProcessPaymentCommand(dto.LoanId, dto.Amount);

        var validationResult = await validator.ValidateAsync(command);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));

        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(null, result);
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
}