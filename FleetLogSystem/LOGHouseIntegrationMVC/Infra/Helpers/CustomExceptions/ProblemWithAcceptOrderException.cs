namespace LOGHouseSystem.Infra.Helpers.CustomExceptions
{
    public class ProblemWithAcceptOrderException : Exception
    {
        public ProblemWithAcceptOrderException(string message, int orderId)
        {
            Message = message;
            OrderId = orderId;
        }

        public string Message { get; set; }

        public int OrderId { get; set; }
    }
}
