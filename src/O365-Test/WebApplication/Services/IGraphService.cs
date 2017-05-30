using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Services
{
    public interface IGraphService
    {
        MessageRequest BuildEmailMessage(string recipients, string subject);

        Task<string> GetMyEmailAddress(string accessToken);

        Task<string> SendEmail(string accessToken, MessageRequest email);
    }
}
