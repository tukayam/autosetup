using System.IO;
using System.Reflection;

namespace TestSetupGenerator.CodeAnalysis.UnitTests.Helpers.IO
{
    static class TextFileReader
    {
        public static string ReadFile(string filePath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"TestSetupGenerator.CodeAnalysis.UnitTests.{filePath}";
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
