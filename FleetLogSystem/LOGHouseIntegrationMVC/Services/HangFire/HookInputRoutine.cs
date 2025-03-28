using Hangfire;
using LOGHouseSystem.Adapters.Extensions.BlingExtension.Dto.Hook.Request;
using LOGHouseSystem.Adapters.Extensions.TinyExtension.Dto.Hook.Request;
using LOGHouseSystem.Adapters.Extensions.TinyExtension.Dto.Hook.Response;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.HangFire.Interface;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Helpers;

namespace LOGHouseSystem.Services.HangFire
{
    public class HookInputRoutine : IHookInputRoutine
    {
        private IBlingCallbackService _blingCallbackService;
        private ITinyHookService _tinyHookService;
        private readonly IHookInputService _hookInputService;
        private IHangfireExecutionService _hangfireExecutionService;
        private IIntegrationRepository _integrationRepository;
        private IBlingService _blingService;


        public HookInputRoutine(IHookInputService hookInputService, IBlingCallbackService blingCallbackService, ITinyHookService tinyHookService, IHangfireExecutionService hangfireExecutionService, 
            IIntegrationRepository integrationRepository, IBlingService blingService)
        {
            _hookInputService = hookInputService;
            _blingCallbackService = blingCallbackService;
            _tinyHookService = tinyHookService;
            _hangfireExecutionService = hangfireExecutionService;
            _integrationRepository = integrationRepository;
            _blingService = blingService;
        }

        public void DeleteOldHooks()
        {
            _hookInputService.DeleteOldHooks();
        }

        public async Task HookRoutine()
        {
            if (Environment.EnvironmentName == "Development") return;

            if (!_hangfireExecutionService.StartTask(HangfireTask.HOOK_ROUTINE)) return;

            List<HookInput> hookInputList = await _hookInputService.GetAllByType(HookTypeEnum.Order);

            if (hookInputList != null)
            {
                foreach (HookInput item in hookInputList)
                {
                    if (item.Origin == OrderOrigin.Bling)
                    {
                        try
                        {
                            await _blingCallbackService.ProcessBlingOrder(item);
                        }
                        catch (Exception ex)
                        {
                            await _hookInputService.AddNoteById(item.Id, ex.ToString());
                            await _hookInputService.InativateHook(item);
                        }
                    }                    
                }
            }

            _hangfireExecutionService.EndTask(HangfireTask.HOOK_ROUTINE);

            return;
        }

        public async Task HookInvoiceRoutine()
        {
            if (Environment.EnvironmentName == "Development") return;

            if (!_hangfireExecutionService.StartTask(HangfireTask.HOOK_INVOICE_ROUTINE)) return;

            List<HookInput> hookInputList = await _hookInputService.GetAllByType(HookTypeEnum.Invoice);

            if (hookInputList != null)
            {
                foreach (HookInput item in hookInputList)
                {
                    if (item.Origin == OrderOrigin.Bling)
                    {
                        try
                        {
                            await _blingCallbackService.ProcessBlingInvoice(item);
                        }

                        catch (Exception ex)
                        {
                            await _hookInputService.AddNoteById(item.Id, ex.ToString());
                            await _hookInputService.InativateHook(item);
                        }
                    }
                    else if (item.Origin == OrderOrigin.Tiny)
                    {
                        try
                        {
                            await _tinyHookService.ProcessTinyOrder(item);
                        }
                        catch (Exception ex)
                        {
                            string message = string.Format("{0} - {1}", ex.Message, ex.StackTrace);

                            await _hookInputService.AddNoteById(item.Id, message);
                        }
                    }
                }
            }

            _hangfireExecutionService.EndTask(HangfireTask.HOOK_INVOICE_ROUTINE);
        }

        public async Task ForceAllIntegration()
        {
            if (Environment.EnvironmentName != "Production") return;

            var integrations = await _integrationRepository.GetAllIntegrationsByName(new List<string> { "BLING" });

            var integrationsToRun = integrations.Select(i => new OrderIntegrationViewModel
            {
                ClientId = i.ClientId,
                EntryDateStart = DateTimeHelper.GetCurrentDateTime().AddDays(-3).Date,
                EntryDateEnd = DateTimeHelper.GetCurrentDateTime(),
                OrderOrigin = OrderOrigin.Bling
            });

            foreach (var integration in integrationsToRun)
            {
                try
                {
                    switch (integration.OrderOrigin)
                    {
                        case OrderOrigin.Bling:
                            Hangfire.BackgroundJob.Schedule(
                                () => _blingService.IntegrateOrders(integration), TimeSpan.FromSeconds(2));

                            break;
                        default:
                            throw new Exception("Integração não implemetado");
                    }

                }
                catch (Exception ex)
                {
                }
            }
        }
    }

}
