namespace LOGHouseSystem.Adapters.Extensions.ShopeeExtension
{
    public class UnAuthorizedRequestShopeeException : Exception
    {
        public UnAuthorizedRequestShopeeException(string? message) : base(message)
        {
        }
    }
}
