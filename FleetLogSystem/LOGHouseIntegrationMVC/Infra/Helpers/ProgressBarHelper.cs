namespace LOGHouseSystem.Infra.Helpers
{
    public class ProgressBarHelper
    {
        public static decimal GetPercentage(decimal itemsToPercent, decimal totalItems)
        {
            return itemsToPercent * 100 / totalItems;
        }
    }
}
