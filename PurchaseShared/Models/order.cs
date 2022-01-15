using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace PurchaseShared.Models
{
  public class Order
  {
    //[JsonProperty(PropertyName = "_id")]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    [Required(ErrorMessage = "Le champ Nom Agent est requis")]
    [JsonProperty(PropertyName = "agentname")]
    [BsonElement("agentname")]
    public string AgentName { get; set; }
    
    [JsonProperty(PropertyName = "noligne")]
    [BsonElement("noligne")]
    public int NoLigne { get; set; }

    [JsonProperty(PropertyName = "unitname")]
    [BsonElement("unitname")]
    public string UnitName { get; set; }
  
    [Required(ErrorMessage = "Le numéro de la DA est requis")]
    [JsonProperty(PropertyName = "reqnumber")]
    [BsonElement("reqnumber")]
    public int ReqNumber { get; set; }

    [JsonProperty(PropertyName = "description")]
    [BsonElement("description")]
    public string Description { get; set; }
    
    [JsonProperty(PropertyName = "localisation")]
    [BsonElement("localisation")]
    public string Localisation { get; set; }
    [JsonProperty(PropertyName = "type")]
    [BsonElement("type")]
    public string Type { get; set; }
    [JsonProperty(PropertyName = "prdate")]
    [BsonElement("prdate")]
    public DateTime PrDate { get; set; }

    [JsonProperty(PropertyName = "approveddate")]
    [BsonElement("approveddate")]
    public DateTime ApprovedDate { get; set; }

    [JsonProperty(PropertyName = "status")]
    [BsonElement("status")]
    public string Status { get; set; }

    [JsonProperty(PropertyName = "purchaseorder")]
    [BsonElement("purchaseorder")]
    public string PurchaseOrder { get; set; }

    [JsonProperty(PropertyName = "datepo")]
    [BsonElement("datepo")]
    public DateTime DatePo { get; set; }
    
    [Required(ErrorMessage = "Le champ montant est requis"), Range(1.0, Double.MaxValue, ErrorMessage = "Valeur Minimum 1 requise")]
    [JsonProperty(PropertyName = "amount")]
    [BsonElement("amount")]
    public double Amount { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("vendor_id")]
    [JsonProperty(PropertyName = "vendor_id")]
    public string VendorId { get; set; }
    [BsonIgnore]
    [JsonProperty(PropertyName = "vendor")]
    public Vendor Vendor { get; set; }
  }
}
