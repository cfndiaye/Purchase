using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Purchase.Infrastructure.Models;
using PurchaseShared.Models;
using Purchase.Services.Contract;

namespace Purchase.Services.Implementation
{
    public class VendorService : IVendorService
    {
        private readonly IMongoCollection<Vendor> _vendorsCollection;
        private readonly IMongoCollection<Order> _ordersCollection;
        private readonly MongoContext _mongoContext;


        public VendorService(IOptions<PurchaseStoreDatabaseSettings> storeSettings)
        {
            _mongoContext = new MongoContext(storeSettings);
            _vendorsCollection = _mongoContext.GetDatabase().GetCollection<Vendor>(storeSettings.Value.VendorsCollectionName);
            _ordersCollection = _mongoContext.GetDatabase().GetCollection<Order>(storeSettings.Value.OrdersCollectionName);
        }

        /// <summary>
        /// Ajouter un vendor
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        public async Task AddVendorAsync(Vendor vendor) =>
            await _vendorsCollection.InsertOneAsync(vendor);

        /// <summary>
        /// Supprimer un vendor
        /// </summary>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        public async Task DeleteVendorAsync(string vendorId) =>
            await _vendorsCollection.DeleteOneAsync(x => x.Id == vendorId);

        /// <summary>
        /// Retourne le vendor suivant son Id méthode Asynchrone
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Vendor> GetVendorByIdAsync(string id) =>
            await _vendorsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        /// <summary>
        /// Retourne le vendor suivant son Id méthode synchrone
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Vendor GetVendorById(string id) => 
            _vendorsCollection.Find(x =>x.Id == id).FirstOrDefault();

        /// <summary>
        /// Retourne une liste de vendors en fonction du parametre query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
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
            
        /// <summary>
        /// Renvoit la liste des vendors sans leurs orders
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Vendor>> GetVendorsAsync() =>
            await _vendorsCollection.Find(_ => true).ToListAsync();

        /// <summary>
        /// Renvoit la liste des vendors avec leurs orders
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Vendor>> GetVendorsWithOrdersAsync()
        {
           var vendors = await _vendorsCollection.Find(_ => true).ToListAsync();
           vendors.ForEach(async (v) => v.OrderList = await _ordersCollection.Find(o => o.VendorId == v.Id).ToListAsync());
           return vendors;
        }
            
        /// <summary>
        /// Update vendors
        /// </summary>
        /// <param name="id"></param>
        /// <param name="vendor"></param>
        /// <returns></returns>
        public async Task UpdateVendorAsync(string id, Vendor vendor) =>
            await _vendorsCollection.ReplaceOneAsync(x => x.Id == id, vendor);

        /// <summary>
        /// Met a jour la list des orders du vendor
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task UpdateVendorOrdersAsync(string vendorId, string orderId)
        {
            var vendor = await _vendorsCollection.Find(v => v.Id == vendorId).FirstOrDefaultAsync();
            if (vendor != null)
            {
                //ajouter de l'orderId dans la collection des orders
                if (vendor.Orders != null)
                {
                    vendor.Orders.Add(orderId);
                    await _vendorsCollection.ReplaceOneAsync(v => v.Id == vendorId, vendor);
                }
                else
                {
                    vendor.Orders = new List<string>();
                    vendor.Orders.Add(orderId);
                    await _vendorsCollection.ReplaceOneAsync(v => v.Id == vendorId, vendor);
                }
            }
        }
        
        /// <summary>
        /// Supprime la order du vendor
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="vendor"></param>
        /// <returns></returns>
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
