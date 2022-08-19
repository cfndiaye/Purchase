using System;
namespace PurchaseShared.Models
{
    public class TypeVendorStat
    {
        public TypeVendorStat(string vendorType, decimal value)
        {
            VendorType = vendorType;
            Value = value;
        }
        public string VendorType { get; set; }
        public decimal Value { get; set; }
    }
}

