namespace LOGHouseSystem.Infra.Helpers
{
    public static class DateTimeHelper
    {

        public static DateTime GetCurrentDateTime()
        {
            DateTime serverTime = DateTime.Now;
            DateTime _localTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(serverTime, TimeZoneInfo.Local.Id, "E. South America Standard Time");
            return _localTime;
        }
    }
}
