using Microsoft.AspNetCore.Mvc;
using OrderSystem.Api.Contracts.Requests;
using OrderSystem.Api.Contracts.Responses;
using OrderSystem.Application.Ports;
using OrderSystem.Domain.Entities;

namespace OrderSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;

    public CustomersController(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<CustomerResponse>>> GetAll()
    {
        var customers = await _customerRepository.GetAllAsync();
        return Ok(customers.Select(MapToResponse));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerResponse>> GetById(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
            return NotFound();

        return Ok(MapToResponse(customer));
    }

    [HttpPost]
    public async Task<ActionResult<CustomerResponse>> Create(CreateCustomerRequest request)
    {
        var customer = new Customer(request.Name, request.Email);
        await _customerRepository.AddAsync(customer);
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, MapToResponse(customer));
    }

    [HttpPut("{id:guid}/email")]
    public async Task<ActionResult> UpdateEmail(Guid id, [FromBody] string newEmail)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
            return NotFound();

        customer.UpdateEmail(newEmail);
        await _customerRepository.UpdateAsync(customer);
        return NoContent();
    }

    private static CustomerResponse MapToResponse(Customer customer)
    {
        return new CustomerResponse(
            customer.Id,
            customer.Name,
            customer.Email
        );
    }
}