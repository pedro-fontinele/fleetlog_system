using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LOGHouseSystem.Controllers
{
    public class EmailController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly IExpeditionOrderRepository _expeditionOrderRepository;

        public EmailController(IEmailService emailService, IExpeditionOrderRepository expeditionOrderRepository)
        {
            _emailService = emailService;
            _expeditionOrderRepository = expeditionOrderRepository;
        }


        public IActionResult SendEmailWithMessage(string subject, string message,  int orderId)
        {
            try
            {
                ExpeditionOrder order = _expeditionOrderRepository.GetById(orderId);

                message = $"Número da Nota: {order.InvoiceNumber} <br/> <br/>" + message;

                bool result = _emailService.ReceiveMessageAndSendEmail(subject,  message, null, order.ClientId);
                if (result)
                {
                    TempData["SuccessMessage"] = "O Email com as informações do erro foi enviado com sucesso para o cliente!";
                    return RedirectToAction("Index", "ExpeditionOrder");
                }
                else
                {
                    return BadRequest();
                }
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult SendEmailWithMessage(SendEmailAboutOrderViewModel data)
        {
            try
            {
                ExpeditionOrder order = _expeditionOrderRepository.GetById(data.orderId);

                data.message = $"Número da Nota: {order.InvoiceNumber} <br/> <br/>" + data.message;

                bool result = _emailService.ReceiveMessageAndSendEmail(data.subject, data.message, null, order.ClientId);
                if (result)
                {
                    return Json(new
                    {
                        status = "success",
                        message = "O Email com as informações do erro foi enviado com sucesso para o cliente!"
                    });
                    
                }
                else
                {
                    return Json(new
                    {
                        status = "error",
                        message = "Ocorreu uma falha no envio do email, verifique os logs para detalhes [Send Email]"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = "error",
                    message = $"Ocorreu uma falha no envio do email. Err: {ex.Message}"
                });
            }
        }
    }
}
