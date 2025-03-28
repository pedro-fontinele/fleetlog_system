namespace LOGHouseSystem.Adapters.Extensions.BaseOAUTH2Extension
{
    public class AuthenticationNotFoundException : Exception
    {
        public AuthenticationNotFoundException(string? message) : base(message)
        {
        }
    }
}
