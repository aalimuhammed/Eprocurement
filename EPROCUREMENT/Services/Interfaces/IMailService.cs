using EPROCUREMENT.Models;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
