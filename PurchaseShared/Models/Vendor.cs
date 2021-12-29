using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace PurchaseShared.Models
{
    public class Vendor
    {
        
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        //[JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }
        
        [BsonElement("name")]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [BsonElement("emailid")]
        [JsonProperty(PropertyName = "emailid")]
        public string EmailId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> Orders { get; set; }

        [BsonIgnore]
        public List<Order> OrderList { get; set; }
    }
}
