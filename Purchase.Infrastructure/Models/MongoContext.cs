using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Purchase.Infrastructure.Models
{
    public class MongoContext
    {
        public MongoContext(IOptions<PurchaseStoreDatabaseSettings> purchaseOption)
        {
           
            _credential = MongoCredential.CreateCredential( purchaseOption.Value.DatabaseName, 
                purchaseOption.Value.User,
                purchaseOption.Value.Password
                );

            ///
            _mongoConnectionString = purchaseOption.Value.ConnectionString;

            var settings = MongoClientSettings.FromConnectionString(_mongoConnectionString);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
                
            
            _mongoClient = new MongoClient(settings);
            _mongoDatabase = _mongoClient.GetDatabase(purchaseOption.Value.DatabaseName);

        }

        protected readonly MongoClient _mongoClient;
        protected readonly MongoCredential _credential;

        protected readonly string _mongoConnectionString;
        protected readonly IMongoDatabase _mongoDatabase;

        public IMongoDatabase GetDatabase()
        {
            return _mongoDatabase;
        }



    }
}
