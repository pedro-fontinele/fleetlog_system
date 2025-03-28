using LOGHouseSystem.Infra.Enums;

namespace LOGHouseSystem.Models
{
    public class HookInput
    {
        public int Id { get; set; }

        public string? Payload { get; set; }

        public string? Note { get; set; }

        public string? Cnpj { get; set; }

        public OrderOrigin Origin { get; set; }
        public HookTypeEnum? Type { get; set; }
        public bool? Status { get; set; }
        public string Code { get; set; }

        public DateTime? Date { get; set; }

    }
}
