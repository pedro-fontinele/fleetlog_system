using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Text;

namespace LOGHouseSystem.Infra.Helpers
{
    public static class StringHelper
    {
        public static int ConvertToInt(this string str)
        {
			try
			{
				return Convert.ToInt32(str);
			}
			catch
			{
				return default;
			}
        }

        public static string AddMaskToCnpj(this string cnpj)
        {
            return Regex.Replace(cnpj, @"(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})", "$1.$2.$3/$4-$5");
        }

        public static string AddMaskToCpf(this string cpf)
        {
            return Regex.Replace(cpf, @"(\d{3})(\d{3})(\d{3})(\d{2})", "$1.$2.$3-$4");
        }

        public static string AddMaskToEmail(this string value, int length = 5)
        {
            var pattern = @"(?<=[\w]{1})[\w-\._\+%]*(?=[\w]{1}@)"; // Regex to hide email. ex. w*****6@gmail.com

            return Regex.Replace(value, pattern, m => new string('*', m.Length > length ? length : m.Length));
        }

        public static string FormatName(this string value)
        {
            return value
                .TrimStart()
                .TrimEnd()
                .ToUpper()
                .Replace("\"", " ")
                .Replace("\'", " ")
                .Replace("&", "E");
        }

        public static bool IsValidEmail(this string email)
        {
            try
            {
                if (email.Length > 50) return false;

                var pattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";

                if (!Regex.IsMatch(email, pattern)) return false;

                // Instead of using a regular expression to validate an email address, you can use the System.Net.Mail.MailAddress class. 
                // To determine whether an email address is valid, pass the email address to the MailAddress.MailAddress(String) class constructor.
                // 
                // Link: https://docs.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format?redirectedfrom=MSDN

                var mail = new MailAddress(email);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string NormalizeString(this string value)
        {
            return Encoding.UTF8.GetString(Encoding.GetEncoding("ISO-8859-8").GetBytes(value));
        }

        public static string NullIfWhiteSpace(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            return value;
        }

        public static string RomanNumeralFrom(this int number)
        {
            return
                new string('I', number)
                    .Replace(new string('I', 1000), "M")
                    .Replace(new string('I', 900), "CM")
                    .Replace(new string('I', 500), "D")
                    .Replace(new string('I', 400), "CD")
                    .Replace(new string('I', 100), "C")
                    .Replace(new string('I', 90), "XC")
                    .Replace(new string('I', 50), "L")
                    .Replace(new string('I', 40), "XL")
                    .Replace(new string('I', 10), "X")
                    .Replace(new string('I', 9), "IX")
                    .Replace(new string('I', 5), "V")
                    .Replace(new string('I', 4), "IV");
        }

        public static string ToCamelCase(this string value)
        {
            var ret = value;

            if (!string.IsNullOrEmpty(value) && value.Length > 1)
            {
                ret = value.Substring(0, 1).ToUpper() + value.Substring(1);
            }

            return ret;
        }

        public static string ToUtf8(this string value)
        {
            var bytes = Encoding.Default.GetBytes(value);

            return Encoding.UTF8.GetString(bytes);
        }

        public static string UnmaskOnlyNumbers(this string value)
        {
            Regex digitsOnly = new Regex(@"[^\d]");

            return digitsOnly.Replace(value, "");
        }

        public static string ReplaceWithComma(this decimal value)
        {
            return value.ToString("0.00").Replace(".", ",");
        }
    }
}
