using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Purchase.Models
{
    public class Order
    {
        //[JsonProperty(PropertyName = "_id")]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "datepo")]
        [BsonElement("datepo")]
        public DateTime DatePo { get; set; }
        [JsonProperty(PropertyName ="amount")]
        [BsonElement("amount")]
        public double Amount { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty(PropertyName = "vendor_id")]
        public string vendorId { get; set; }
        [BsonIgnore]
        [JsonProperty(PropertyName = "vendor")]
        public Vendor Vendor { get; set; }
    }
}
