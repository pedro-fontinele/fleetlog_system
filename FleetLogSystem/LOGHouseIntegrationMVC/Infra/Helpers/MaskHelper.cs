using System.Text.RegularExpressions;

namespace LOGHouseSystem.Infra.Helpers
{
    public static class MaskHelper
    {
        public static string RemoveMask(string value)
        {
            Regex digitsOnly = new Regex(@"[^\d]");
            return digitsOnly.Replace(value, "");
        }


        public static string FormatCNPJ(string CNPJ)
        {
            return
            Convert.ToUInt64(CNPJ).ToString(
            @"00\.000\.000\/0000\-00"
            );
        }
    }
}
