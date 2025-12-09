using Ecommerce.Api.Filters;
using Ecommerce.Domain.DTOs.General;
using Ecommerce.Domain.DTOs.Orders;
using Ecommerce.Application.UseCases.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;


[ApiController]
[Authorize]
[ServiceFilter(typeof(PostAuthorizeFilter))]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly OrderUseCases _orderUseCase;

    public OrderController(OrderUseCases orderUseCase)
    {
        _orderUseCase = orderUseCase;
    }


    [HttpGet("")]
    public async Task<IActionResult> GetOrders([FromQuery] GetOrdersWithFiltersRequest request)
    {

        var result = await _orderUseCase.GetOrdersAsync(request);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });

    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrderById(int orderId)
    {

        var result = await _orderUseCase.GetOrderByIdAsync(orderId);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });


    }


    [HttpPost("")]
    public async Task<IActionResult> CreateOrder([FromBody] AddOrderRequest request)
    {

        var result = await _orderUseCase.AddOrderAsync(request);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });



    }


    [HttpPut("{orderId}")]
    [ServiceFilter(typeof(PostAuthorizeRoleFilter))]
    public async Task<IActionResult> UpdateOrderStaus([FromBody] UpdateOrderRequest request, int orderId)
    {

        var result = await _orderUseCase.UpdateOrderStatusAsync(request, orderId);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });

    }


    [HttpDelete("{orderId}")]
    public async Task<IActionResult> CancelOrder(int orderId)
    {

        var result = await _orderUseCase.CancelOrderAsync(orderId);

        return Ok(new GeneralResponse
        {
            Data = result,
            Message = "Proceso realizado con éxito."
        });

    }

}
