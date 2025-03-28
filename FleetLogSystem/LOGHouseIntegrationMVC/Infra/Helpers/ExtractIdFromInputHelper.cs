using System.Text.RegularExpressions;

namespace LOGHouseSystem.Infra.Helpers
{
    public class ExtractIdFromInputHelper
    {
        public static int ExtractIdFromInput(string input)
        {
            //Get Id from string
            Match match = Regex.Match(input, @"(?<=Id:\s*)\d+");

            if (match.Success && int.TryParse(match.Value, out int id))
            {
                return id;
            }

            throw new ArgumentException("Não foi possível extrair o valor do Id da string.");
        }
    }
}
