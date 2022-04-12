using System;
namespace PurchaseShared.Models
{
    public class VendorStat
    {
        public VendorStat()
        {
        }
        public VendorStat(string id, string name, double totalAmounts)
        {
            Id = id; Name = name; TotalAmounts = totalAmounts;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public double TotalAmounts { get; set; }

    }
}
