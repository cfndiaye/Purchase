using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Purchase.Models
{
    public class MongoContext
    {
        public MongoContext(IOptions<PurchaseStoreDatabaseSettings> purchaseOption)
        {
            _serverAddress = new MongoServerAddress(purchaseOption.Value.Server, purchaseOption.Value.Port);
            _credential = MongoCredential.CreateCredential( purchaseOption.Value.DatabaseName, 
                purchaseOption.Value.User,
                purchaseOption.Value.Password
                );
            
            _mongoClientSettings = new MongoClientSettings
            {
                Server = _serverAddress,
                Credential = _credential,
                MaxConnectionPoolSize = 1000,
                WaitQueueSize = 20000
            };
            _mongoClient = new MongoClient(_mongoClientSettings);
            _mongoDatabase = _mongoClient.GetDatabase(purchaseOption.Value.DatabaseName);

        }

        protected readonly MongoClient _mongoClient;
        protected readonly MongoCredential _credential;
        protected readonly MongoServerAddress _serverAddress;
        protected readonly MongoClientSettings _mongoClientSettings;
        protected readonly IMongoDatabase _mongoDatabase;

        public IMongoDatabase GetDatabase()
        {
            return _mongoDatabase;
        }



    }
}
