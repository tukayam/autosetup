using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestSetupGenerator.CodeAnalysis.UnitTests.Helpers
{
    static class FieldDeclarationsProvider
    {
        public static IEnumerable<SyntaxNode> GetFieldDeclarations(string fileName)
        {
            var source = TextFileReader.ReadFile(fileName);
            var document = DocumentProvider.CreateDocument(source);

            var root = document.GetSyntaxRootAsync().Result;
            return root.DescendantNodes().OfType<FieldDeclarationSyntax>();
        }
    }
}
