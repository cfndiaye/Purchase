using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PurchaseShared.Models;
using Purchase.Services.Contract;
using Purchase.Infrastructure.Models;

namespace Purchase.Services.Implementation
{
  public class OrderService : IOrderService
  {
    private readonly IMongoCollection<Order> _ordersCollection;
    private readonly VendorService _vendorService;
        private readonly MongoContext  _mongoContext;

    public OrderService(IOptions<PurchaseStoreDatabaseSettings> storeSettings, 
        
        VendorService vendorService)
    {
            _mongoContext = new MongoContext(storeSettings);
            _ordersCollection = _mongoContext.GetDatabase().GetCollection<Order>(storeSettings.Value.OrdersCollectionName);
            _vendorService = vendorService;
    }

    public async Task AddOrderAsync(Order order) =>
        await _ordersCollection.InsertOneAsync(order);

    public async Task DeleteOrderAsync(string orderId) =>
        await _ordersCollection.DeleteOneAsync(x => x.Id == orderId);

    public async Task<Order> GetOrderByIdAsync(string orderId)
    {
      var order = await _ordersCollection.Find(x => x.Id == orderId).FirstOrDefaultAsync();
            //include vendor reference
            var vendor = await _vendorService.GetVendorByIdAsync(order.VendorId);
      order.Vendor = vendor;
      return order;
    }

    public async Task<Order> GetOrderByPrAsync(int reqNumber)
    {
      var order = await _ordersCollection.Find(x => x.ReqNumber == reqNumber).FirstOrDefaultAsync();
      //include vendor reference
      var vendor = await _vendorService.GetVendorByIdAsync(order.VendorId);
      order.Vendor = vendor;

      return order;
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync(string query) =>
        (await _ordersCollection.Find(_ => true).ToListAsync()).Where(o => o.Vendor.Name.Contains(query));

    public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status)
    {
      var orders = (await _ordersCollection.Find(o => o.Status == status).ToListAsync());
      orders.ForEach(o => o.Vendor =  _vendorService.GetVendorById(o.VendorId));
      return orders;
    }


    public async Task<IEnumerable<Order>> GetOrdersAsync()
    {
      return await _ordersCollection.Find(_ => true).ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync(bool includeVendor)
    {
      var orders = (await _ordersCollection.Find(_ => true).ToListAsync());
      orders.ForEach(async o => o.Vendor = await _vendorService.GetVendorByIdAsync(o.VendorId));
      return orders;
    }

    public async Task UpdateOrderAsync(string id, Order order) =>
        await _ordersCollection.ReplaceOneAsync(x => x.Id == id, order);

    public async Task BulkImportAsync(List<Order> orders)
    {
      await _ordersCollection.InsertManyAsync(orders);
    }

       
  }
}
