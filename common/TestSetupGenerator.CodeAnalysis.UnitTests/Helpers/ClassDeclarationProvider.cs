using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestSetupGenerator.CodeAnalysis.UnitTests.Helpers
{
    static class ClassDeclarationProvider
    {
        public static ClassDeclarationSyntax GetClassDeclaration(string fileName, string className)
        {
            var source = TextFileReader.ReadFile(fileName);
            var document = DocumentProvider.CreateDocument(source);

            var root = document.GetSyntaxRootAsync().Result;
            return root.DescendantNodes().OfType<ClassDeclarationSyntax>().First(_ => _.Identifier.Text == className);
        }
    }
}
