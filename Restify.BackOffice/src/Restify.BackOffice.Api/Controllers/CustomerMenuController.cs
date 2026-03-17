using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Api.Controllers;

[ApiController]
[Route("api/customer")]
[AllowAnonymous]
public class CustomerMenuController : ControllerBase
{
    private readonly ICustomerMenuService _customerMenuService;

    public CustomerMenuController(ICustomerMenuService customerMenuService)
    {
        _customerMenuService = customerMenuService;
    }

    /// <summary>
    /// Obtiene las categorias del menu de un restaurante (publico)
    /// </summary>
    [HttpGet("{tenantId}/menu/categories")]
    public async Task<IActionResult> GetCategories(Guid tenantId, CancellationToken cancellationToken)
    {
        var result = await _customerMenuService.GetCategoriesAsync(tenantId, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtiene los productos del menu (publico)
    /// </summary>
    [HttpGet("{tenantId}/menu/products")]
    public async Task<IActionResult> GetProducts(Guid tenantId, [FromQuery] Guid? categoryId, CancellationToken cancellationToken)
    {
        var result = await _customerMenuService.GetProductsAsync(tenantId, categoryId, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtiene un producto especifico (publico)
    /// </summary>
    [HttpGet("{tenantId}/menu/products/{productId}")]
    public async Task<IActionResult> GetProduct(Guid tenantId, Guid productId, CancellationToken cancellationToken)
    {
        var result = await _customerMenuService.GetProductByIdAsync(tenantId, productId, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Data);
    }

    /// <summary>
    /// Crea un pedido desde el menu del cliente
    /// </summary>
    [HttpPost("{tenantId}/orders")]
    public async Task<IActionResult> CreateOrder(Guid tenantId, [FromBody] CustomerCreateOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await _customerMenuService.CreateOrderAsync(tenantId, request, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetOrderStatus), new { orderId = result.Data!.Id }, result.Data);
    }

    /// <summary>
    /// Obtiene el estado de un pedido (publico, para tracking)
    /// </summary>
    [HttpGet("orders/{orderId}/status")]
    public async Task<IActionResult> GetOrderStatus(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await _customerMenuService.GetOrderStatusAsync(orderId, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Data);
    }

    /// <summary>
    /// Obtiene el historial de pedidos de un cliente
    /// </summary>
    [HttpGet("orders/customer/{customerId}")]
    public async Task<IActionResult> GetCustomerOrders(Guid customerId, CancellationToken cancellationToken)
    {
        var result = await _customerMenuService.GetCustomerOrdersAsync(customerId, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Data);
    }
}
