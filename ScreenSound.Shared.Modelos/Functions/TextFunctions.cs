using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScreenSound.Shared.Modelos.Functions
{
    public class TextFunctions
    {
        public static string RemoveSpacesSpecialCharactersAndAccents(string input)
        {
            // Normaliza a string para remover acentos
            string normalizedString = NormalizeString(input);

            // Define uma expressão regular que mantém apenas letras e números
            string pattern = "[^a-zA-Z0-9]";
            string replacement = "";

            // Cria a instância do Regex
            Regex regex = new Regex(pattern);

            // Substitui os caracteres que correspondem ao padrão definido por uma string vazia
            string result = regex.Replace(normalizedString, replacement);

            return result;
        }

        public static string NormalizeString(string input)
        {
            // Normaliza a string para FormD (separando caracteres base de seus acentos)
            string normalizedString = input.Normalize(NormalizationForm.FormD);

            // Cria um StringBuilder para armazenar a string sem acentos
            StringBuilder stringBuilder = new StringBuilder();

            // Itera sobre cada caractere na string normalizada
            foreach (char c in normalizedString)
            {
                // Verifica se o caractere é um caractere não espacial
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            // Retorna a string sem acentos
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
