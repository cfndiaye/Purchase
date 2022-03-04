using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Purchase.Models;
using PurchaseShared.Models;
using Purchase.Services.Contract;

namespace Purchase.Services.Implementation
{
    public class VendorService : IVendorService
    {
        private readonly IMongoCollection<Vendor> _vendorsCollection;
        private readonly MongoContext _mongoContext;


        public VendorService(IOptions<PurchaseStoreDatabaseSettings> storeSettings)
        {
            _mongoContext = new MongoContext(storeSettings);
            _vendorsCollection = _mongoContext.GetDatabase().GetCollection<Vendor>(storeSettings.Value.VendorsCollectionName);
        }

        public async Task AddVendorAsync(Vendor vendor) =>
            await _vendorsCollection.InsertOneAsync(vendor);

        public async Task DeleteVendorAsync(string vendorId) =>
            await _vendorsCollection.DeleteOneAsync(x => x.Id == vendorId);

        public async Task<Vendor> GetVendorByIdAsync(string id) =>
            await _vendorsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public Vendor GetVendorById(string id) => 
            _vendorsCollection.Find(x =>x.Id == id).FirstOrDefault();

        public async Task<IEnumerable<Vendor>> GetVendorsAsync(string query)
        {
            if(!string.IsNullOrWhiteSpace(query))
            {
                return await _vendorsCollection.Find(v => v.Name.ToLower()
                                                                .Contains(query.ToLower()))
                                                                .ToListAsync();
            }
            return null;
        }
            

        public async Task<IEnumerable<Vendor>> GetVendorsAsync() =>
            await _vendorsCollection.Find(_ => true).ToListAsync();

        public async Task UpdateVendorAsync(string id, Vendor vendor) =>
            await _vendorsCollection.ReplaceOneAsync(x => x.Id == id, vendor);

        public async Task UpdateVendorOrdersAsync(string id, string orderId)
        {
            var vendor = await _vendorsCollection.Find(v => v.Id == id).FirstOrDefaultAsync();
            if (vendor != null)
            {
                //ajouter de l'orderId dans la collection des orders
                if (vendor.Orders != null)
                {
                    vendor.Orders.Add(orderId);
                    await _vendorsCollection.ReplaceOneAsync(v => v.Id == id, vendor);
                }
                else
                {
                    vendor.Orders = new List<string>();
                    vendor.Orders.Add(orderId);
                    await _vendorsCollection.ReplaceOneAsync(v => v.Id == id, vendor);
                }
            }
        }
        

        public async Task RemoveOrderOnVendor(string orderId, Vendor vendor)
        {
            vendor.Orders.Remove(orderId);
           
            if(vendor.OrderList != null)
            { 
                var order = vendor.OrderList.First(o => o.Id == orderId);
                vendor.OrderList.Remove(order);
            }
            
            await _vendorsCollection.ReplaceOneAsync(v => v.Id == vendor.Id, vendor);

        }
    }
}
