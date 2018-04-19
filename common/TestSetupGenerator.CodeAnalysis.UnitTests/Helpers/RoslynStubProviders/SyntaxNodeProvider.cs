using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using TestSetupGenerator.CodeAnalysis.UnitTests.Helpers.IO;

namespace TestSetupGenerator.CodeAnalysis.UnitTests.Helpers.RoslynStubProviders
{
    class SyntaxNodeProvider
    {
        public static IEnumerable<SyntaxNode> GetAllSyntaxNodesFromFile<T>(string fileName) where T : SyntaxNode
        {
            var source = TextFileReader.ReadFile(fileName);
            var document = DocumentProvider.CreateDocument(source);

            var root = document.GetSyntaxRootAsync().Result;
            return root.DescendantNodes().OfType<T>().ToList();
        }
    }
}
