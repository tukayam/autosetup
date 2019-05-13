using System.IO;
using System.Reflection;

namespace AutoSetup.UnitTests.Helpers.IO
{
    static class TextFileReader
    {
        public static string ReadFile(string filePath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"AutoSetup.UnitTests.{filePath}";
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
