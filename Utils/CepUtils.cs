
using System.Text.RegularExpressions;

namespace ConsultaCEP.Utils
{
    public static class CepUtils
    {
        public static bool IsValidCep(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep))
                return false;

            return Regex.IsMatch(cep, @"^\d{8}$");
        }
    }
}