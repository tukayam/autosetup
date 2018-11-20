namespace AutoSetup.CodeAnalysis.CodeAnalyzers
{
    public class TestClassDetector
    {
        public static bool IsTestClass(string className)
        {
            return className.EndsWith("Tests");
        }
    }
}
