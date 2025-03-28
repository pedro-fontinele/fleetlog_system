using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebSocketSharp;
using System.Text;
using System.Security.Policy;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Models;
using LOGHouseSystem.Services;
using LOGHouseSystem.ViewModels;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Controllers.API.Utils.Requests;

namespace LOGHouseSystem.Controllers.API
{


    [Route("api/[controller]")]
    [ApiController]
    public class UtilsController : ControllerBase
    {
        private readonly IEmailService _email;
        private readonly IUserRepository _userRepository;
        private readonly IBlingService _blingService;
        private readonly IIntegrationRepository _integrationRepository;

        public UtilsController(IEmailService email, IUserRepository userRepository, IBlingService blingService, IIntegrationRepository integrationRepository)
        {
            _email = email;
            _userRepository = userRepository;
            _blingService = blingService;
            _integrationRepository = integrationRepository;
        }


        [HttpPost]
        [Route("ForceAllIntegrations")]
        public async Task<ActionResult> ForceAllIntegrations(ForceAllCustomersOrdersIntegrationRequest data)
        {
            var integrations = await _integrationRepository.GetAllIntegrationsByName(new List<string> { "BLING" });

            var setup = integrations.Select(i => new OrderIntegrationViewModel
            {
                ClientId = i.ClientId,
                EntryDateStart = data.startDate,
                EntryDateEnd = data.endDate,
                OrderOrigin = OrderOrigin.Bling
            });

            foreach (var orders in setup)
            {
                try
                {
                    switch (orders.OrderOrigin)
                    {
                        case OrderOrigin.Bling:
                            Hangfire.BackgroundJob.Schedule(
                                () => _blingService.IntegrateOrders(orders), TimeSpan.FromSeconds(2));

                            break;
                        default:
                            throw new Exception("Integração não implemetado");
                    }

                }
                catch (Exception ex)
                {
                }
            }

            return Ok();
           
        }



        [HttpGet]
        public ActionResult Get()
        {
            User user = _userRepository.GetById(68);
            _email.SendEmail(new Models.SendEmails.EmailData
            {
                EmailBody = "Testing gmail send email",
                EmailSubject = "Log house Email",
                EmailToId = Environment.LogOrderEmail,
                EmailToName = Environment.LogOrderEmail
            }, user, null);

            return Ok();
        }


        [Route("websocket")]
        [HttpGet]
        public ActionResult testAdminFunc(string message)
        {
            using (var ws = new WebSocket("wss://socket.loghouse.com.br/"))
            {
                ws.Connect();
                ws.Send(message);
                ws.Close();
            }

            return Ok();
        }
    }
}
