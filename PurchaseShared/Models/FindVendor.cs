using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Linq;

namespace PurchaseShared.Models
{
    public static class FindVendor
    {
        //find vendor for Autocomplete
        public static async Task<IEnumerable<string>> FindVendorsAsyncDelegate(string searchVendor, HttpClient client)
        {
            if (string.IsNullOrWhiteSpace(searchVendor))
                return Array.Empty<string>();

            var vendors = await client.GetFromJsonAsync<IEnumerable<Vendor>>($"api/Vendors/GetByName/{searchVendor}");
            return vendors.Select(v => new string(v.Id + " " + v.Name)).ToList();
        }

        public static string GetDescription(string query)
        {
            if (query.Length > 24)
            {
                var selected = query.Substring(25);
                return selected;

            }

            return null;
        }
    }
}
