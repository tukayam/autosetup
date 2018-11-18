using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace TestSetupGenerator.CodeAnalysis.CodeGenerators
{
    public interface IConstructorGenerator
    {
        ConstructorDeclarationSyntax Constructor(string containingTypeName, IEnumerable<SyntaxNode> methodBodySyntaxNodes, SyntaxGenerator generator);
    }

    public class ConstructorGenerator : IConstructorGenerator
    {
        public ConstructorDeclarationSyntax Constructor(string containingTypeName, IEnumerable<SyntaxNode> methodBodySyntaxNodes, SyntaxGenerator generator)
        {
            IEnumerable<SyntaxNode> parameters = null;
            Accessibility accessibility = Accessibility.Public;
            DeclarationModifiers declarationModifiers = default(DeclarationModifiers);
            IEnumerable<SyntaxNode> baseConstructorArguments = null;
            return generator.ConstructorDeclaration(containingTypeName, parameters,
                accessibility, declarationModifiers, baseConstructorArguments, methodBodySyntaxNodes) 
                as ConstructorDeclarationSyntax;
        }
    }
}
