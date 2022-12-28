using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PurchaseShared.Models
{
    public class PurchaseRequestLine
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public int RequestNumber { get; set; }
        public int LineNumber { get; set; }
        public string ItemCode { get; set; }
        public string ItemCoswinCode { get; set; }
        public string ITemDescription { get; set; }
        public string ITemEnglishDescription { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime RequestApprovedDate { get; set; }
        public DateTime RequestValidationMMDDate { get; set; }
        public string Requestor { get; set; }
        public string RequestDescription { get; set; }
        public string LastPurchaseOrder { get; set; }
        public DateTime LastPurchaseOrderDate { get; set; }
        public string LastPoVendor { get; set; }
        public string LastPurchaseCurrency { get; set; }
        public decimal LastPurchaseUnitePrice { get; set; }
        public bool Closed { get; set; }



    }
}
