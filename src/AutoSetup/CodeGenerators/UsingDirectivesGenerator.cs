using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace AutoSetup.CodeGenerators
{
    public interface IUsingDirectivesGenerator
    {
        List<UsingDirectiveSyntax> UsingDirectives(Compilation compilation, IEnumerable<ParameterSyntax> parameters, IEnumerable<string> namespacesForTestingFrameworks, SyntaxGenerator generator);

        List<UsingDirectiveSyntax> TestingFrameworkUsingDirectives(IEnumerable<string> namespacesForTestingFrameworks, SyntaxGenerator generator);
    }

    public class UsingDirectivesGenerator : IUsingDirectivesGenerator
    {
        public List<UsingDirectiveSyntax> TestingFrameworkUsingDirectives(IEnumerable<string> namespacesForTestingFrameworks, SyntaxGenerator generator)
        {
            //todo: add other usings for class under test
            var usingDirectives = new List<UsingDirectiveSyntax>();
            foreach (var ns in namespacesForTestingFrameworks)
            {
                usingDirectives.Add(generator.NamespaceImportDeclaration(ns) as UsingDirectiveSyntax);
            }

            return usingDirectives;
        }

        public List<UsingDirectiveSyntax> UsingDirectives(Compilation compilation, IEnumerable<ParameterSyntax> parameters, IEnumerable<string> namespacesForTestingFrameworks, SyntaxGenerator generator)
        {
            var usingDirectives = TestingFrameworkUsingDirectives(namespacesForTestingFrameworks, generator);
            foreach (var parameter in parameters)
            {
                var type = parameter.DescendantTokens().First(t => t.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.IdentifierToken));
                var symbols = compilation.GetSymbolsWithName(s => s == type.Text);
                if (symbols.Any())
                {
                    var ns = symbols.First().ContainingNamespace.Name;
                    if (!string.IsNullOrWhiteSpace(ns))
                    {
                        usingDirectives.Add(generator.NamespaceImportDeclaration(ns) as UsingDirectiveSyntax);
                    }                    
                }
            }

            return usingDirectives;
        }
    }
}
