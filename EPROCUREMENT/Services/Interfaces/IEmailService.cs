using System.Threading;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(
                string toEmail,
                string subject, string body,
                CancellationToken cancellationToken = default);
    }
}