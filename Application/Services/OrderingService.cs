﻿using System.Security.Cryptography.X509Certificates;
using Application.DTOs;
using Application.DTOs.Mappers;
using Domain.Models;
using Domain.StaticData;
using Infrastructure.DataAccess;

namespace Application.Services;

public class OrderingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly CartService _cartService;
    public OrderingService(IUnitOfWork unitOfWork, CartService cartService)
    {
        _cartService = cartService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult> CreateOrderFromCart(int userId, string paymentMethod = StaticData.CashPayment)
    {
        var result = new ServiceResult()
        {
            Success = false
        };
        var main = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);
        var cartDto = main!.ToCartDTO();
        var cartItems = main!.CartItems.ToList();
        if (cartItems.Count == 0)
        {
            result.Errors.Add("Cart is Empty!");
            return result;
        }
        var userAddress = await _unitOfWork.Addresses.GetAsync(a=> a.UserId == userId);
        var order = new Order()
        {
            OrderDate = DateTime.Now,
            TotalPrice = cartDto.TotalPrice,
            UserId = cartDto.UserId,
            AddressId = userAddress?.Id ?? 0,
            PaymentMethod = paymentMethod
        };
        await _unitOfWork.Orders.AddAsync(order);
        await _unitOfWork.CommitAsync();
        var orderItems = new List<OrderItem>();
        for (var i = 0; i < cartItems.Count; i++)
        {
            orderItems.Add(new OrderItem()
            {
                OrderId = order.Id,
                ProductId = cartItems[i].ProductId,
                Quantity = cartItems[i].Quantity,
                Price = ((await _unitOfWork.Products.GetAsync(product => product.Id == cartItems[i].ProductId))!).Price
            });
        }
        await _unitOfWork.OrderItems.AddBulkAsync(orderItems);
        if (await _unitOfWork.CommitAsync() > 0)
        {
            result.Success = true;
            return result;
        }

        result.Errors.Add("Error Creating order");
        return result;
    }

    public async Task<IEnumerable<Order>> ViewPastOrders(int userId)
    {
        var orders = (await _unitOfWork.Orders
            .GetAllAsync(o => o.UserId == userId, o => o.OrderItems));
        return orders;
    }

    public async Task<IEnumerable<Order>?> GetAllOrders(int pageIndex, int pageSize)
    {
        var orders = new List<Order>();
        orders = (await _unitOfWork.Orders.GetPaginatedAsync(pageIndex, pageSize))?.Items;
        return orders;
    }
    public async Task<IEnumerable<Order>?> GetAllOrders()
    {
        var orders = await _unitOfWork.Orders.GetAllAsync();
        return orders;
    }

    public async Task<Order?> GetOrderDetailsById(int? orderId)
    {
        var order = await _unitOfWork.Orders.GetAsync(o=> o.Id == orderId, o => o.OrderItems);
        return order;
    }
    
}