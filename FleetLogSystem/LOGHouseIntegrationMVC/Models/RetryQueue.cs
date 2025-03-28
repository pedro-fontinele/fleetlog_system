namespace LOGHouseSystem.Models
{
    public class RetryQueue
    {
        public int Id { get; set; }

        public string Note { get; set; }

        public int? HookInputId { get; set; }
        //public virtual HookInput HookInput { get; set; }

        public DateTime LastTry { get; set; }

        public int Tries { get; set; }
    }
}
