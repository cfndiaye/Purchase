using System;
namespace Purchase.Models
{
    public class PurchaseStoreDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string VendorsCollectionName { get; set; }
        public string OrdersCollectionName { get; set; }
    }
}
