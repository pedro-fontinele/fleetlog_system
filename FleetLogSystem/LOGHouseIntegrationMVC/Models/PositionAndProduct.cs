namespace LOGHouseSystem.Models
{
    public class PositionAndProduct
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        public int AddressingPositionId { get; set; }

        public virtual AddressingPosition AddressingPosition { get; set; }
    }
}
