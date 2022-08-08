using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PurchaseShared.Models;
using Purchase.Services.Contract;
using Purchase.Infrastructure.Models;
using MongoDB.Bson;

namespace Purchase.Services.Implementation
{
  public class OrderService : IOrderService
  {
    private readonly IMongoCollection<Order> _ordersCollection;
    private readonly IMongoCollection<Vendor> _vendorsCollection;
    private readonly VendorService _vendorService;
    private readonly MongoContext _mongoContext;

    public OrderService(IOptions<PurchaseStoreDatabaseSettings> storeSettings, 

        VendorService vendorService)
    {
      
      _mongoContext = new MongoContext(storeSettings);
      _ordersCollection = _mongoContext.GetDatabase().GetCollection<Order>(storeSettings.Value.OrdersCollectionName);
      _vendorsCollection = _mongoContext.GetDatabase().GetCollection<Vendor>(storeSettings.Value.VendorsCollectionName);
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
      orders.ForEach(o => o.Vendor = _vendorService.GetVendorById(o.VendorId));
      return orders;
    }


    public async Task<IEnumerable<Order>> GetOrdersAsync()
    {
      return await _ordersCollection.Find(_ => true).ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync(bool includeVendor)
    {
      /*var orders = (await _ordersCollection.Find(_ => true).ToListAsync());
      if (includeVendor)
        orders.ForEach(async o => o.Vendor = await _vendorService.GetVendorByIdAsync(o.VendorId));*/

        var queryableOrder = _ordersCollection.AsQueryable();
      var queryableVendor =  _vendorsCollection.AsQueryable();
      var result = from order in queryableOrder
                   join vendor in queryableVendor on order.VendorId equals vendor.Id
                   select new Order
                   {
                     Id = order.Id,
                     AgentName = order.AgentName,
                     NoLigne = order.NoLigne,
                     UnitName = order.UnitName,
                     ReqNumber = order.ReqNumber,
                     Description = order.Description,
                     Localisation = order.Localisation,
                     Type = order.Type,
                     PrDate = order.PrDate,
                     ApprovedDate = order.ApprovedDate,
                     Status = order.Status,
                     PurchaseOrder = order.PurchaseOrder,
                     DatePo = order.DatePo,
                     DateLivraison = order.DateLivraison,
                     Amount = order.Amount,
                     VendorId = order.VendorId, 
                     Vendor = vendor,
                     Devise = order.Devise,
                     Commentaires = order.Commentaires,
                     Livree = order.Livree

                   };
      return await  Task.Run(() => result.ToList());
    }

    public async Task UpdateOrderAsync(string id, Order order) =>
        await _ordersCollection.ReplaceOneAsync(x => x.Id == id, order);

    public async Task BulkImportAsync(List<Order> orders)
    {
      await _ordersCollection.InsertManyAsync(orders);
    }

    //Get Total cost off orders
    public async Task<double> GetTotalCostAsync(string devise)
    {
      var orders = await _ordersCollection.Find(_ => true).ToListAsync();

      return orders.Where(o => o.Devise == devise).Sum(o => o.Amount);
    }
    //Get Total Cost By Vendor
    public async Task<double> GetTotalCostByVendorIdAsync(string vendorId, string devise)
    {
      var orders = await _ordersCollection.Find(_ => true).ToListAsync();

      return orders.Where(o => o.VendorId == vendorId).Where(o => o.Devise == devise).Sum(o => o.Amount);
    }

    public async Task<IEnumerable<Order>> GetOrdersByVendorIdAsync(string vendorId)
    {
      return await _ordersCollection.Find(o => o.VendorId == vendorId).ToListAsync();
    }

    public  async Task<double> GetTotalCostByVendorType(string type)
    {
      var queryableOrder = _ordersCollection.AsQueryable();
      var queryableVendor =  _vendorsCollection.AsQueryable();
      var result = from order in queryableOrder
                   join vendor in queryableVendor on order.VendorId equals vendor.Id
                   select new Order
                   {
                     Id = order.Id,
                     AgentName = order.AgentName,
                     NoLigne = order.NoLigne,
                     UnitName = order.UnitName,
                     ReqNumber = order.ReqNumber,
                     Description = order.Description,
                     Localisation = order.Localisation,
                     Type = order.Type,
                     PrDate = order.PrDate,
                     ApprovedDate = order.ApprovedDate,
                     Status = order.Status,
                     PurchaseOrder = order.PurchaseOrder,
                     DatePo = order.DatePo,
                     DateLivraison = order.DateLivraison,
                     Amount = order.Amount,
                     VendorId = order.VendorId, 
                     Vendor = vendor,
                     Devise = order.Devise,
                     Commentaires = order.Commentaires,
                     Livree = order.Livree

                   }; 

      return await Task.Run(() => result.Where(x => x.Vendor.Type == type).Sum(o => o.Amount));

    }

    

  }
}
