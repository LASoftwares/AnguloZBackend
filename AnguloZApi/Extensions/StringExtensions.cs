using System.Globalization;
using System.Text;

namespace AnguloZApi.Extensions
{
    public static class StringExtensions
    {
        public static string RemoverAcentos(this string palavra)
        {
            var normalizedString = palavra.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

            for (int i = 0; i < normalizedString.Length; i++)
            {
                char c = normalizedString[i];
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder
                .ToString()
                .Normalize(NormalizationForm.FormC).Replace(" ", "-");
        }
    }
}
