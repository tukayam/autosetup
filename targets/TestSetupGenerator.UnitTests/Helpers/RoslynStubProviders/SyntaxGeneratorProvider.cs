using Microsoft.CodeAnalysis.Editing;

namespace TestSetupGenerator.UnitTests.Helpers.RoslynStubProviders
{
    class SyntaxGeneratorProvider
    {
        public SyntaxGenerator GetSyntaxGenerator()
        {
            var source = string.Empty;
            return GetSyntaxGenerator(source);
        }

        public SyntaxGenerator GetSyntaxGenerator(string source)
        {
            var document = DocumentProvider.CreateDocument(source);
            return SyntaxGenerator.GetGenerator(document);
        }
    }
}
