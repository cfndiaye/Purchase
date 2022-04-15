using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseShared.Models
{
    public class Utilisateur
    {
        public Utilisateur(string id, string userName, string prenom, string nom)
        {
            Id = id;
            UserName = userName;
            Prenom = prenom;
            Nom = nom;

        }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("username")]
        public string UserName { get; set; }
        [BsonElement("password")]
        public string Password { get; set; }
        [BsonElement("prenom")]
        public string Prenom { get; set; }
        [BsonElement("nom")]
        public string Nom { get; set; }
    }
}
