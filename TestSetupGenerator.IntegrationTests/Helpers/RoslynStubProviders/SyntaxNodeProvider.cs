using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using TestSetupGenerator.IntegrationTests.Helpers.IO;

namespace TestSetupGenerator.IntegrationTests.Helpers.RoslynStubProviders
{
    static class SyntaxNodeProvider
    {
        private static ConcurrentDictionary<string, IEnumerable<SyntaxNode>> _cacheAllSyntaxNodesFromFile;
        private static ConcurrentDictionary<string, SyntaxNode> _cacheSyntaxNodeFromFile;

        static SyntaxNodeProvider()
        {
            _cacheAllSyntaxNodesFromFile = new ConcurrentDictionary<string, IEnumerable<SyntaxNode>>();
            _cacheSyntaxNodeFromFile = new ConcurrentDictionary<string, SyntaxNode>();
        }

        public static IEnumerable<SyntaxNode> GetAllSyntaxNodesFromFile<T>(string filePath) where T : SyntaxNode
        {
            if (!_cacheAllSyntaxNodesFromFile.ContainsKey(filePath))
            {
                var source = TextFileReader.ReadFile(filePath);
                var document = DocumentProvider.CreateDocument(source);

                var root = document.GetSyntaxRootAsync().Result;
                var nodes = root.DescendantNodes().OfType<T>().ToList();
                _cacheAllSyntaxNodesFromFile.TryAdd(filePath, nodes);
            }

            return _cacheAllSyntaxNodesFromFile[filePath];
        }

        public static T GetSyntaxNodeFromFile<T>(string filePath, string syntaxIdentifier) where T : SyntaxNode
        {
            var key = filePath + syntaxIdentifier;
            if (!_cacheSyntaxNodeFromFile.ContainsKey(key))
            {

                var source = TextFileReader.ReadFile(filePath);
                var document = DocumentProvider.CreateDocument(source);

                var root = document.GetSyntaxRootAsync().Result;
                var node = root.DescendantNodesAndSelf().OfType<T>().First(_ => _.ChildTokens().Any(t => t.IsKind(SyntaxKind.IdentifierToken) && t.Text == syntaxIdentifier));

                _cacheSyntaxNodeFromFile.TryAdd(key, node);
            }

            return _cacheSyntaxNodeFromFile[key] as T;
        }
    }
}
