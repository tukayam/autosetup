using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoSetup.CodeAnalysis.Models
{
    public class ClassUnderTest
    {
        public ClassDeclarationSyntax ClassDeclarationSyntax { get; }
        public SemanticModel SemanticModel { get; }

        public ClassUnderTest(ClassDeclarationSyntax classDeclarationSyntax, SemanticModel semanticModel)
        {
            ClassDeclarationSyntax = classDeclarationSyntax;
            SemanticModel = semanticModel;
        }
    }
}
