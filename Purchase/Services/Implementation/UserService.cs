using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Purchase.Infrastructure.Models;
using Purchase.Services.Contract;
using PurchaseShared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Purchase.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<Utilisateur> _userCollection;
        private readonly MongoContext _mongoContext;
        public UserService(IOptions<PurchaseStoreDatabaseSettings> storeSettings)
        {
            _mongoContext = new MongoContext(storeSettings);
            _userCollection = _mongoContext.GetDatabase().GetCollection<Utilisateur>(storeSettings.Value.UsersCollectionName);
        }
        public async Task AddUserAsync(Utilisateur user)
        {
             await _userCollection.InsertOneAsync(user);
        }

        public async Task DeleteUserAsync(string id)
        {
            await _userCollection.DeleteOneAsync( u => u.Id == id);
        }

        public async Task<Utilisateur> GetUserByIdAsync(string id)
        {
            return await _userCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Utilisateur> GetUserByNameAsync(string userName)
        {
            return await _userCollection.Find(u => u.UserName == userName).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Utilisateur>> GetUsersAsync()
        {
            return await _userCollection.Find(_ => true).ToListAsync();
        }

        public async Task<IEnumerable<Utilisateur>> GetUsersAsync(string query)
        {
            return await _userCollection.Find(u => u.UserName.ToLower().Contains(query.ToLower()) || u.Nom.ToLower().Contains(query.ToLower()) || u.Prenom.ToLower().Contains(query.ToLower())).ToListAsync();
        }

        public async Task UpdateUserAsync(Utilisateur user)
        {
            await _userCollection.ReplaceOneAsync(u => u.Id == user.Id, user);
        }

        public async Task<bool> AuthenticateUserAsync(string userName, string password)
        {
            var user = await _userCollection.Find(u => u.UserName == userName && u.Password == password).FirstOrDefaultAsync();
            if (user == null)
                return false;
            else
                return true;
        }
    }
}
