using Ecommerce.Domain.DTOs.Orders;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Ecommerce.Application.UseCases.Orders;

public class OrderUseCases
{

    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IConfiguration _config;
    private static readonly ActivitySource _source = new("OrderUseCases");
    private readonly ILogger<OrderUseCases> _logger;


    public OrderUseCases(IOrderRepository orderRepository, IProductRepository productRepository, IConfiguration config, ILogger<OrderUseCases> logger)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _config = config;
        _logger = logger;
    }

    public async Task<string> AddOrderAsync(AddOrderRequest request)
    {
        using var activity = _source.StartActivity("AddOrderAsync");

        _logger.LogInformation(
            "Iniciando creación de orden para cliente {UserId}",
            request.UserId);

        decimal subtotal = 0;
        var orderItems = new List<OrderItem>();

        for (int i = 0; i < request.Items.Count; i++)
        {
            var findProduct = await _productRepository.GetByIdAsync(request.Items[i].ProductId);
            if (findProduct == null)
            {
                throw new InvalidOperationException($"Producto {request.Items[i].ProductName} no encontrado.");
            }
            if (findProduct.Stock < request.Items[i].Quantity)
            {
                throw new InvalidOperationException($"Stock insuficiente para el producto {request.Items[i].ProductName}.");
            }
            var newOrdenItem = OrderItem.Create(request.Items[i]);
            orderItems.Add(newOrdenItem);
            subtotal += request.Items[i].Price * request.Items[i].Quantity;
        }

        var orderAddress = OrderAddress.Create(request.ShippingAddress, request.BillingAddress);
        var orderStatus = OrderStatus.Create();

        decimal shippingCost = 0;
        decimal discount = 0;

        decimal taxRate = decimal.Parse(_config["App:taxRate"]!) / 100m;
        decimal tax = subtotal * taxRate;

        decimal total = subtotal + discount + tax + shippingCost;
        var paymentInfo = OrderPayment.Create(request.PaymentInfo, total);


        activity?.SetTag("order.userId", request.UserId);
        activity?.SetTag("order.items_count", request.Items.Count);
        activity?.SetTag("order.subtotal", subtotal.ToString());
        activity?.SetTag("order.shippingCost", shippingCost.ToString());
        activity?.SetTag("order.discount", discount.ToString());
        activity?.SetTag("order.tax", tax.ToString());
        activity?.SetTag("order.total", total.ToString());

        using var dbSpan = _source.StartActivity("SaveToDatabase", ActivityKind.Internal);
        {

            var newOrder = Order.Create(request.UserId, orderItems, orderAddress, paymentInfo, orderStatus, total, subtotal, tax, discount, shippingCost);
            await _orderRepository.AddOrderWithTransactionAsync(newOrder);

            dbSpan?.SetTag("db.operation", "insert");
            dbSpan?.SetTag("db.rows_affected", 1);

            _logger.LogInformation("Orden {OrderNumber} creada exitosamente para cliente {UserId}",
                newOrder.OrderNumber, newOrder.UserId);

            return $"Orden {newOrder.OrderNumber} creada exitosamente.";
        }
    }

    public async Task<string> CancelOrderAsync(int orderId)
    {

        var findOrden = await _orderRepository.GetByIdAsync(orderId) ??
            throw new InvalidOperationException("Orden no encontrada.");

        var canceledOrderStatus = OrderStatus.Cancel(findOrden.OrderStatus!);
        var canceledOrderPayment = OrderPayment.Cancel(findOrden.OrderPayment!);
        var cancelOrder = Order.Cancel(findOrden, canceledOrderStatus, canceledOrderPayment);
        await _orderRepository.CancelAsync(cancelOrder, findOrden.OrderItems.ToList());

        return "Orden cancelada exitosamente.";

    }

    public async Task<string> UpdateOrderStatusAsync(UpdateOrderRequest request, int orderId)
    {

        var findOrden = await _orderRepository.GetByIdAsync(orderId) ??
            throw new InvalidOperationException("Orden no encontrada.");

        var updatedOrder = OrderStatus.SetStatus(findOrden.OrderStatus!, request.Status);
        await _orderRepository.UpdateAsync(updatedOrder);

        return "Orden actualizada exitosamente.";

    }

    public async Task<object> GetOrdersAsync(GetOrdersWithFiltersRequest request)
    {
        var result = await _orderRepository.GetOrderAsync(request);

        var safeOrderResponse = new List<object>();

        result.Items.ForEach(order =>
        {
            safeOrderResponse.Add(Order.ToSafeResponse(order));
        });


        return new
        {
            Items = safeOrderResponse,
            result.TotalCount,
            result.PageNumber,
            result.PageSize,
            result.TotalPages,
            result.HasPreviousPage,
            result.HasNextPage,
        };
    }

    public async Task<object> GetOrderByIdAsync(int productId)
    {
        var findOrden = await _orderRepository.GetByIdAsync(productId) ??
            throw new InvalidOperationException("Orden no encontrada.");

        return Order.ToSafeResponseDetail(findOrden!);
    }

}
