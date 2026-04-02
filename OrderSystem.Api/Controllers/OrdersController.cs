using Microsoft.AspNetCore.Mvc;
using OrderSystem.Api.Contracts.Requests;
using OrderSystem.Api.Contracts.Responses;
using OrderSystem.Application.Ports;
using OrderSystem.Application.Services;

namespace OrderSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;
    private readonly IOrderRepository _orderRepository;

    public OrdersController(OrderService orderService, IOrderRepository orderRepository)
    {
        _orderService = orderService;
        _orderRepository = orderRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderResponse>>> GetAll()
    {
        var orders = await _orderRepository.GetAllAsync();
        return Ok(orders.Select(MapToResponse));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderResponse>> GetById(Guid id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null)
            return NotFound();

        return Ok(MapToResponse(order));
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponse>> Create(CreateOrderRequest request)
    {
        var order = await _orderService.CreateOrderAsync(request.CustomerId);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, MapToResponse(order));
    }

    [HttpPost("{id:guid}/items")]
    public async Task<ActionResult> AddItem(Guid id, AddItemRequest request)
    {
        await _orderService.AddItemToOrderAsync(id, request.ProductId, request.Quantity);
        return NoContent();
    }

    [HttpDelete("{id:guid}/items/{productId:guid}")]
    public async Task<ActionResult> RemoveItem(Guid id, Guid productId)
    {
        await _orderService.RemoveItemFromOrderAsync(id, productId);
        return NoContent();
    }

    [HttpPost("{id:guid}/coupon")]
    public async Task<ActionResult> ApplyCoupon(Guid id, ApplyCouponRequest request)
    {
        await _orderService.ApplyCouponAsync(id, request.CouponCode);
        return NoContent();
    }

    [HttpPost("{id:guid}/confirm")]
    public async Task<ActionResult> Confirm(Guid id)
    {
        await _orderService.ConfirmOrderAsync(id);
        return NoContent();
    }

    [HttpPost("{id:guid}/pay")]
    public async Task<ActionResult> Pay(Guid id)
    {
        await _orderService.PayOrderAsync(id);
        return NoContent();
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult> Cancel(Guid id)
    {
        await _orderService.CancelOrderAsync(id);
        return NoContent();
    }

    private static OrderResponse MapToResponse(Domain.Entities.Order order)
    {
        return new OrderResponse(
            order.Id,
            order.CustomerId,
            order.Status.ToString(),
            order.CreatedAt,
            order.PaidAt,
            order.Subtotal.Amount,
            order.Discount?.Amount,
            order.Total.Amount,
            order.Items.Select(i => new OrderItemResponse(
                i.Id,
                i.ProductId,
                i.ProductName,
                i.UnitPrice.Amount,
                i.Quantity,
                i.TotalPrice.Amount
            )).ToList()
        );
    }
}