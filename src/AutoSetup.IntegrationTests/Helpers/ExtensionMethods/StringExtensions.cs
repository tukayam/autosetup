using System.Linq;

namespace AutoSetup.IntegrationTests.Helpers.ExtensionMethods
{
    static class StringExtensions
    {
        public static string RemoveWhitespace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());
        }
    }
}
