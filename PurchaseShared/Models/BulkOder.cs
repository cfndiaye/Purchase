using System;
using System.Collections.Generic;

namespace PurchaseShared.Models
{
    public class BulkOder
    {
        public BulkOder(List<Order> orders)
        {
            Orders = orders;
        }

        public List<Order> Orders { get; set; }
    }
}
