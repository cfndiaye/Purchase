using PurchaseShared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Purchase.Services.Contract
{
    public interface IUserService
    {
        public Task<IEnumerable<Utilisateur>> GetUsersAsync();
        public Task<IEnumerable<Utilisateur>> GetUsersAsync(string query);
        public Task<Utilisateur> GetUserByIdAsync(string id);
        public Task<Utilisateur> GetUserByNameAsync(string name);
        public Task AddUserAsync(Utilisateur user);
        public Task UpdateUserAsync(Utilisateur user);
        public Task DeleteUserAsync(string id);
        public Task<bool> AuthenticateUserAsync(string username, string password);
    }
}
