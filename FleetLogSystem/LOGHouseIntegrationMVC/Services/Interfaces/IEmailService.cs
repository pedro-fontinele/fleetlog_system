using LOGHouseSystem.Models;
using LOGHouseSystem.Models.SendEmails;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface IEmailService
    {
        bool SendEmail(EmailData emailData, User? user = null, int? clientId = null, int? invoiceNumber = null);
        bool ReceiveMessageAndSendEmail(string subject, string message, User? user, int? clientId);
        bool SendEmailToDevolution(EmailData emailData, User? user, int? clientId, List<byte[]> imageBytesList = null, string? invoiceNumber = null);

    }
}
