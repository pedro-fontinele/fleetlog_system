using LOGHouseSystem.Infra.Enums;

namespace LOGHouseSystem.Infra.Helpers
{
    public class ColorHelper
    {
        public static string PriorityColor (PriorityEnum? priority)
        {
            switch (priority)
            {
                case PriorityEnum.Baixa: return "#1cc88a";
                case PriorityEnum.Media: return "#f6c23e";
                case PriorityEnum.Alta: return "#e74a3b";
                default: return "#000";
            }
        }
    }
}
