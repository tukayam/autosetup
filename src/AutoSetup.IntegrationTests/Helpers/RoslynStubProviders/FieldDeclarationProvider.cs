using System.Collections.Concurrent;
using System.Linq;
using AutoSetup.IntegrationTests.Helpers.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoSetup.IntegrationTests.Helpers.RoslynStubProviders
{
    static class FieldDeclarationProvider
    {
        private static ConcurrentDictionary<string, FieldDeclarationSyntax> _cacheSyntaxNodeFromFile;

        static FieldDeclarationProvider()
        {
            _cacheSyntaxNodeFromFile = new ConcurrentDictionary<string, FieldDeclarationSyntax>();
        }

        public static FieldDeclarationSyntax GetFieldDeclarationFromFile(string filePath, string syntaxIdentifier)
        {
            var key = filePath + syntaxIdentifier;
            if (!_cacheSyntaxNodeFromFile.ContainsKey(key))
            {

                var source = TextFileReader.ReadFile(filePath);
                var document = DocumentProvider.CreateDocument(source);

                var root = document.GetSyntaxRootAsync().Result;
                var node = root.DescendantNodesAndSelf().OfType<FieldDeclarationSyntax>()
                    .First(_ => _.DescendantNodes().OfType<VariableDeclaratorSyntax>()
                        .First().ChildTokens().Any(t => t.IsKind(SyntaxKind.IdentifierToken) && t.Text == syntaxIdentifier));

                _cacheSyntaxNodeFromFile.TryAdd(key, node);
            }

            return _cacheSyntaxNodeFromFile[key];
        }
    }
}