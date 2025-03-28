using LOGHouseSystem.Adapters.Extensions.Discord;
using LOGHouseSystem.Models;
using LOGHouseSystem.Models.SendEmails;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Helper;
using LOGHouseSystem.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
using Newtonsoft.Json.Linq;
using System.Net.Mail;
using System.Web.Helpers;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace LOGHouseSystem.Services
{
    public class EmailService : IEmailService
    {
        private readonly ISentEmailService _sentEmailService;
        private readonly IClientsRepository _clientRepository;
        

        EmailSettings _emailSettings = null;
        public EmailService(IOptions<EmailSettings> options, ISentEmailService sentEmailService, IClientsRepository clientRepository)
        {
            _emailSettings = options.Value;
            _sentEmailService = sentEmailService;
            _clientRepository = clientRepository;
        }

        public bool ReceiveMessageAndSendEmail(string subject, string message, User? user, int? clientId)
        {

            Client client = _clientRepository.GetById(clientId);

            if(user == null)
            {
                user = client.User;
            }

            EmailData emailData = new EmailData()
            {
                EmailToId = user.Email,
                EmailToName = user.Name,
                EmailSubject = subject,
                EmailBody = $"<h1>Olá, {user.Name} </h1>" + "<br/>" +
                                       "<p> Você está recebendo esse Email por que tem uma atualização em um dos seus pedidos.\r\n </p>" +
                                       "<br/>"  + "<br/>" + $"{message}" + "<br/>" +
                                       "<br/>" + "Atenciosamente,\r\nEquipe LogHouse"
            };

            bool sendEmail = SendEmail(emailData, user, 0);

            return sendEmail;
        }

        public bool SendEmail(EmailData emailData, User? user, int? clientId, int? invoiceNumber = null)
        {
            try
            {
                if(_sentEmailService.EmailAlreadySendedToday(emailData.EmailSubject,clientId)) return true;

                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                MimeMessage emailMessage = new MimeMessage();

                MailboxAddress emailFrom = new MailboxAddress(_emailSettings.Name, _emailSettings.EmailId);
                emailMessage.From.Add(emailFrom);

                MailboxAddress emailTo = new MailboxAddress(emailData.EmailToName, emailData.EmailToId);
                emailMessage.To.Add(emailTo);

                emailMessage.Subject = emailData.EmailSubject;

                BodyBuilder emailBodyBuilder = new BodyBuilder();

                var emailLayout = BaseLayoutEmail.BaseEmailLayout;

                emailBodyBuilder.HtmlBody = emailLayout.Replace("[TITLE]", !string.IsNullOrEmpty(emailData.EmailTitle) ? emailData.EmailTitle : emailData.EmailSubject).Replace("[BODY]", emailData.EmailBody);

                emailMessage.Body = emailBodyBuilder.ToMessageBody();

                SmtpClient emailClient = new SmtpClient();
                emailClient.CheckCertificateRevocation = false;
                emailClient.Connect(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
                emailClient.Authenticate(_emailSettings.EmailId, _emailSettings.Password);
                emailClient.Send(emailMessage);
                emailClient.Disconnect(true);
                emailClient.Dispose();

                _sentEmailService.AddEmail(emailData, user, clientId, invoiceNumber);


                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"[Send Email] Falha ao enviar email. err: {ex.Message}");

                DiscordExtension discordExtension = new DiscordExtension();
                discordExtension.AddLogInDiscord(new Adapters.Extensions.Discord.Models.Embed
                {
                    color = Adapters.Extensions.Discord.Models.DiscordStatusColor.ERROR,
                    title = "<SendEmail> Envio de Email falhou",
                    description = $"Falha ao enviar email de devolução. err ```{ex.Message}``` data ```{JObject.FromObject(emailData)}```"
                });
                //TempData["ErrorMessage"] = "Não foi possível enviar o email, por favor, tente novamente. Detalhe do erro: " + ex
                return false;
            }
        }

        public bool SendEmailToDevolution(EmailData emailData, User? user, int? clientId, List<byte[]> imageBytesList = null, string? invoiceNumber = null)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                MimeMessage emailMessage = new MimeMessage();

                MailboxAddress emailFrom = new MailboxAddress(_emailSettings.Name, _emailSettings.EmailId);
                emailMessage.From.Add(emailFrom);

                MailboxAddress emailTo = new MailboxAddress(emailData.EmailToName, emailData.EmailToId);
                emailMessage.To.Add(emailTo);

                emailMessage.Subject = emailData.EmailSubject;

                var emailLayout = BaseLayoutEmail.BaseEmailLayout;

                TextPart emailBody = new TextPart("html")
                {
                    Text = emailLayout.Replace("[TITLE]", !string.IsNullOrEmpty(emailData.EmailTitle) ? emailData.EmailTitle : emailData.EmailSubject)
                                     .Replace("[BODY]", emailData.EmailBody)
                };

                Multipart multipart = new Multipart("mixed");
                multipart.Add(emailBody);

                if (imageBytesList != null)
                {
                    foreach (var imageBytes in imageBytesList)
                    {
                        var imagePart = new MimePart("image", "png")
                        {
                            Content = new MimeContent(new MemoryStream(imageBytes), ContentEncoding.Default),
                            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                            ContentTransferEncoding = ContentEncoding.Base64,
                            FileName = "image.png" // Nome do arquivo da imagem
                        };
                        multipart.Add(imagePart);
                    }
                }

                emailMessage.Body = multipart;

                using (var emailClient = new SmtpClient())
                {
                    emailClient.CheckCertificateRevocation = false;
                    emailClient.Connect(_emailSettings.Host, _emailSettings.Port,SecureSocketOptions.StartTls);
                    emailClient.Authenticate(_emailSettings.EmailId, _emailSettings.Password);
                    emailClient.Send(emailMessage);
                    emailClient.Disconnect(true);
                }


                int intInvoiceValue;

                if (!int.TryParse(invoiceNumber, out intInvoiceValue))
                {
                    intInvoiceValue = 0;
                }

                _sentEmailService.AddEmail(emailData, user, clientId, intInvoiceValue);

                return true;
            }
            catch (Exception ex)
            {
                DiscordExtension discordExtension = new DiscordExtension();
                discordExtension.AddLogInDiscord(new Adapters.Extensions.Discord.Models.Embed
                {
                    color = Adapters.Extensions.Discord.Models.DiscordStatusColor.ERROR,
                    title = "<SendEmailToDevolution> Envio de Email falhou",
                    description = $"Falha ao enviar email de devolução. err ```{ex.Message}```"
                });
                // Lide com a exceção apropriadamente
                return false;
            }
        }

    }    
}
