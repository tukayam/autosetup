using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestSetupGenerator.CodeAnalysis.UnitTests.Helpers.IO;

namespace TestSetupGenerator.CodeAnalysis.UnitTests.Helpers.RoslynStubProviders
{
    class TypeSyntaxProvider
    {
        public static TypeSyntax GetTypeSyntax(string fileName, string typeName)
        {
            var source = TextFileReader.ReadFile(fileName);
            var document = DocumentProvider.CreateDocument(source);

            var root = document.GetSyntaxRootAsync().Result;
            return root.DescendantNodes().OfType<TypeSyntax>().First(_ => _.ToString() == typeName);
        }
    }
}
