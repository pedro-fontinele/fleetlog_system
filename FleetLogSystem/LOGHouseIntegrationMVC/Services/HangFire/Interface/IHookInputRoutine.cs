namespace LOGHouseSystem.Services.HangFire.Interface
{
    public interface IHookInputRoutine
    {
        Task HookRoutine();
        Task HookInvoiceRoutine();

        void DeleteOldHooks();

        Task ForceAllIntegration();
    }
}
