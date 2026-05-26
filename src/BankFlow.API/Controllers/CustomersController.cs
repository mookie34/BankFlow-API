using AutoMapper;
using BankFlow.Domain.Entities;
using BankFlow.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static BankFlow.Application.DTOs.Dtos;

namespace BankFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CustomersController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll()
    {
        var customers = await _unitOfWork.Customers.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<CustomerDto>>(customers));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerDto>> GetById(int id)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(id);
        if (customer is null)
            return NotFound(new { error = $"Customer with ID {id} not found." });

        return Ok(_mapper.Map<CustomerDto>(customer));
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create([FromBody] CreateCustomerDto dto)
    {
        var existing = await _unitOfWork.Customers.GetByDocumentNumberAsync(dto.DocumentNumber);
        if (existing is not null)
            return Conflict(new { error = $"A customer with document {dto.DocumentNumber} already exists." });

        try
        {
            var customer = new Customer(
                dto.FirstName, dto.LastName, dto.DocumentNumber,
                dto.Email, dto.Phone, dto.CreditScore);

            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<CustomerDto>(customer);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}