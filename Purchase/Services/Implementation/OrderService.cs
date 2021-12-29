using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PurchaseShared.Models;
using Purchase.Services.Contract;
using Purchase.Models;

namespace Purchase.Services.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IMongoCollection<Order> _ordersCollection;
        private readonly VendorService _vendorService;
        public OrderService(IOptions<PurchaseStoreDatabaseSettings> purchaseStoreDatabaseSettings, VendorService vendorService)
        {
            var mongoClient = new MongoClient(purchaseStoreDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(purchaseStoreDatabaseSettings.Value.DatabaseName);
            _ordersCollection = mongoDatabase.GetCollection<Order>(purchaseStoreDatabaseSettings.Value.OrdersCollectionName);
            _vendorService = vendorService;
        }

        public async Task AddOrderAsync(Order order) =>
            await _ordersCollection.InsertOneAsync(order);

        public async Task DeleteOrderAsync(string orderId) =>
            await _ordersCollection.DeleteOneAsync(x => x.Id == orderId);

        public async Task<Order> GetOrderByIdAsync(string orderId)
        {
           var order = await _ordersCollection.Find(x => x.Id == orderId).FirstOrDefaultAsync();
            var vendor = await _vendorService.GetVendorByIdAsync(order.vendorId);
            order.Vendor = vendor;
            return order;
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(string query) =>
            (await _ordersCollection.Find(_ => true).ToListAsync()).Where(o => o.Vendor.Name.Contains(query));

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            var orders = await _ordersCollection.Find(_ => true).ToListAsync();
            orders.ForEach(o => o.Vendor = _vendorService.GetVendorById(o.vendorId));
            return orders;
        }

        public async Task UpdateOrderAsync(string id, Order order) =>
            await _ordersCollection.ReplaceOneAsync(x => x.Id == id, order);
    }
}
