using System.Threading.Tasks;

namespace WebApplication.Services
{
    public interface IAuthProvider
    {
        Task<string> GetUserAccessTokenAsync();
    }
}
