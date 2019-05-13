﻿using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace AutoSetup.CodeGenerators
{
    public interface IUsingDirectivesGenerator
    {
        List<UsingDirectiveSyntax> UsingDirectives(SemanticModel model, IEnumerable<ParameterSyntax> parameters, IEnumerable<string> namespacesForTestingFrameworks, SyntaxGenerator generator);

        List<UsingDirectiveSyntax> UsingDirectives(IEnumerable<string> namespacesForTestingFrameworks, SyntaxGenerator generator);
    }

    public class UsingDirectivesGenerator : IUsingDirectivesGenerator
    {
        public List<UsingDirectiveSyntax> UsingDirectives(IEnumerable<string> namespacesForTestingFrameworks, SyntaxGenerator generator)
        {
            //todo: add other usings for class under test
            var usingDirectives = new List<UsingDirectiveSyntax>();
            foreach (var ns in namespacesForTestingFrameworks)
            {
                usingDirectives.Add(generator.NamespaceImportDeclaration(ns) as UsingDirectiveSyntax);
            }

            return usingDirectives;
        }

        public List<UsingDirectiveSyntax> UsingDirectives(SemanticModel model, IEnumerable<ParameterSyntax> parameters, IEnumerable<string> namespacesForTestingFrameworks, SyntaxGenerator generator)
        {
            var usingDirectives = UsingDirectives(namespacesForTestingFrameworks, generator);
            foreach (var parameter in parameters)
            {
                var ns = model.GetTypeInfo(parameter).Type.Name;
                usingDirectives.Add(generator.NamespaceImportDeclaration(ns) as UsingDirectiveSyntax);
            }

            return usingDirectives;
        }
    }
}
