using Hangfire;
using LOGHouseSystem.Services.HangFire;
using LOGHouseSystem.Services.HangFire.Interface;
using LOGHouseSystem.Services.Interfaces;

namespace LOGHouseSystem.Extensions
{
    public static class ServiceExtensionHookReturnInvoiceConfiguration
    {
        public static void AddReturnInvoiceHook()
        {
            RecurringJob.AddOrUpdate<IReceptNoteLotsRoutine>(x => x.CreateReceiptLots(), "* * * * *");
            /*RecurringJob.AddOrUpdate<INFeRoutine>(x => x.Routine(), "* * * * *");
            RecurringJob.AddOrUpdate<IReturnInvoiceRoutine>(x => x.ReturnInvoiceRoutineMethod(), "* * * * *");*/
        }
    }
}
