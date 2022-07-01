﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using PurchaseShared.Models;

namespace Purchase.Services.Contract
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetOrdersAsync(string query);
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task<Order> GetOrderByIdAsync(string id);
        Task<Order> GetOrderByPrAsync(int pr);

        Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status);
        Task<IEnumerable<Order>> GetOrdersByVendorIdAsync(string vendorId);
        Task AddOrderAsync(Order order);
        Task UpdateOrderAsync(string id, Order order);
        Task DeleteOrderAsync(string orderId);
        Task BulkImportAsync(List<Order> orders);
    }
}
