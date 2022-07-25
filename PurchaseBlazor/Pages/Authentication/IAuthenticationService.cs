using System.Threading.Tasks;
using PurchaseShared.Models;

namespace PurchaseBlazor.Pages.Authentication
{
    public interface IAuthenticationService
    {
        Task<AuthenticatedUserModel> Login(AuthenticationUserModel userForAuthentication);
        Task Logout();
    }
}