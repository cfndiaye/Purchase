using System;
namespace Purchase.Infrastructure.Models
{
    public class PurchaseStoreDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public string DatabaseName { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string VendorsCollectionName { get; set; }
        public string OrdersCollectionName { get; set; }
    }
}
