using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace TestSetupGenerator.CodeAnalysis.CodeGenerators
{
    public class UsingDirectiveGenerator
    {
        public List<UsingDirectiveSyntax> UsingDirectives( IEnumerable<string> namespacesForTestingFrameworks, SyntaxGenerator generator)
        {
            //todo: add other usings for class under test
            var usingDirectives = new List<UsingDirectiveSyntax>();
            foreach (var ns in namespacesForTestingFrameworks)
            {
                usingDirectives.Add(generator.NamespaceImportDeclaration(ns) as UsingDirectiveSyntax);
            }

            return usingDirectives;
        }

        public async Task<SyntaxNode> AddUsingDirectivesToNewRoot(SyntaxNode existingRoot, SyntaxNode newRoot, IEnumerable<UsingDirectiveSyntax> usingDirectives)
        {
            var existingUsingDirectives = existingRoot.DescendantNodes().OfType<UsingDirectiveSyntax>().ToList();

            var firstUsingDirectiveInFile = existingUsingDirectives.FirstOrDefault();
            var nodeToInsertAfter = firstUsingDirectiveInFile ?? newRoot.DescendantNodes().First();

            foreach (var usingDirective in usingDirectives)
            {
                if (existingUsingDirectives.All(_ => _.Name != usingDirective.Name))
                {
                    if (firstUsingDirectiveInFile != null)
                    {
                        newRoot = newRoot.InsertNodesAfter(firstUsingDirectiveInFile, usingDirectives);
                    }
                    else
                    {
                        newRoot = newRoot.InsertNodesBefore(nodeToInsertAfter, usingDirectives);
                    }
                }
            }

            return newRoot;
        }
    }
}
