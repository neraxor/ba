using Microsoft.AspNetCore.Mvc;
using OrderSystem.Api.Contracts.Requests;
using OrderSystem.Api.Contracts.Responses;
using OrderSystem.Application.Services;

namespace OrderSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductsController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductResponse>>> GetAll()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products.Select(MapToResponse));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> GetById(Guid id)
    {
        var product = await _productService.GetProductAsync(id);
        if (product == null)
            return NotFound();

        return Ok(MapToResponse(product));
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create(CreateProductRequest request)
    {
        var product = await _productService.CreateProductAsync(
            request.Name,
            request.Description,
            request.Price,
            request.Stock
        );
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, MapToResponse(product));
    }

    [HttpPut("{id:guid}/price")]
    public async Task<ActionResult> UpdatePrice(Guid id, [FromBody] decimal newPrice)
    {
        await _productService.UpdatePriceAsync(id, newPrice);
        return NoContent();
    }

    [HttpPost("{id:guid}/stock")]
    public async Task<ActionResult> AddStock(Guid id, [FromBody] int quantity)
    {
        await _productService.AddStockAsync(id, quantity);
        return NoContent();
    }

    private static ProductResponse MapToResponse(Domain.Entities.Product product)
    {
        return new ProductResponse(
            product.Id,
            product.Name,
            product.Description,
            product.Price.Amount,
            product.Price.Currency,
            product.Stock
        );
    }
}