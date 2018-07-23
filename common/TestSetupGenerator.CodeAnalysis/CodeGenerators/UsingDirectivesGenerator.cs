using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace TestSetupGenerator.CodeAnalysis.CodeGenerators
{
    public interface IUsingDirectivesGenerator
    {
        List<UsingDirectiveSyntax> UsingDirectives( IEnumerable<string> namespacesForTestingFrameworks, SyntaxGenerator generator);
    }

    public class UsingDirectivesGenerator : IUsingDirectivesGenerator
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
    }
}
