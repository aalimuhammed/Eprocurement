using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
    public interface IGraphEmailService
    {
        Task<bool>SendEmailAsync(
            string recipientEmail,
            string subject,
            string body);
    }
}
