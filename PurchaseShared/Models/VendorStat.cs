using System;
namespace PurchaseShared.Models
{
    public class VendorStat
    {
        public VendorStat()
        {
        }
        public VendorStat(string id, string name, decimal totalAmounts, string type)
        {
            Id = id; Name = name; TotalAmounts = totalAmounts;
            Type = type;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal TotalAmounts { get; set; }

        public string ShortName => Name.Length > 5 ? Name.Substring(0,5) : Name;

    }
}
