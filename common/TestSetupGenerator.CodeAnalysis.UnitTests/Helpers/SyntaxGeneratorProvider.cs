using Microsoft.CodeAnalysis.Editing;

namespace TestSetupGenerator.CodeAnalysis.UnitTests.Helpers
{
    class SyntaxGeneratorProvider
    {
        public SyntaxGenerator GetSyntaxGenerator()
        {
            var source = string.Empty;
            var document = DocumentProvider.CreateDocument(source);
            return SyntaxGenerator.GetGenerator(document);
        }
    }
}
