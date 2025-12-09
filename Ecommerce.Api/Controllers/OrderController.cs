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
        try
        {
            var result = await _orderUseCase.GetOrdersAsync(request);

            return Ok(new GeneralResponse
            {
                Data = result,
                Message = "Proceso realizado con éxito."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new GeneralResponse { Message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new GeneralResponse { Message = "Error interno del servidor " });
        }
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrderById(int orderId)
    {
        try
        {
            var result = await _orderUseCase.GetOrderByIdAsync(orderId);

            return Ok(new GeneralResponse
            {
                Data = result,
                Message = "Proceso realizado con éxito."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new GeneralResponse { Message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new GeneralResponse { Message = "Error interno del servidor" });
        }
    }


    [HttpPost("")]
    public async Task<IActionResult> CreateOrder([FromBody] AddOrderRequest request)
    {
        try
        {
            var result = await _orderUseCase.AddOrderAsync(request);

            return Ok(new GeneralResponse
            {
                Data = result,
                Message = "Proceso realizado con éxito."
            });
        }
        catch (InvalidOperationException ex)
        {

            return BadRequest(new GeneralResponse { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new GeneralResponse { Message = "Error interno del servidor" + ex.Message });
        }

    }


    [HttpPut("{orderId}")]
    [ServiceFilter(typeof(PostAuthorizeRoleFilter))]
    public async Task<IActionResult> UpdateOrderStaus([FromBody] UpdateOrderRequest request, int orderId)
    {
        try
        {
            var result = await _orderUseCase.UpdateOrderStatusAsync(request, orderId);

            return Ok(new GeneralResponse
            {
                Data = result,
                Message = "Proceso realizado con éxito."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new GeneralResponse { Message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new GeneralResponse { Message = "Error interno del servidor" });
        }
    }


    [HttpDelete("{orderId}")]
    public async Task<IActionResult> CancelOrder(int orderId)
    {
        try
        {
            var result = await _orderUseCase.CancelOrderAsync(orderId);

            return Ok(new GeneralResponse
            {
                Data = result,
                Message = "Proceso realizado con éxito."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new GeneralResponse { Message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new GeneralResponse { Message = "Error interno del servidor" });
        }
    }

}
