using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestSetupGenerator.CodeAnalysis.UnitTests.Helpers.IO;

namespace TestSetupGenerator.CodeAnalysis.UnitTests.Helpers.RoslynStubProviders
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
