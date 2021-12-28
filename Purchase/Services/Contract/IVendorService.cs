using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Purchase.Models;

namespace Purchase.Services.Contract
{
    public interface IVendorService
    {
        Task<IEnumerable<Vendor>> GetVendorsAsync(string query);
        Task<IEnumerable<Vendor>> GetVendorsAsync();
        Task<Vendor> GetVendorByIdAsync(string id);
        Task AddVendorAsync(Vendor vendor);
        Task UpdateVendorAsync(string id, Vendor vendor);
        Task UpdateVendorOrdersAsync(string id, string orderId);
        Task DeleteVendorAsync(string vendorId);
        Task RemoveOrderOnVendor(string id, Vendor vendor);
    }
}
