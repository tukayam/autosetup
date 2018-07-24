using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using TestSetupGenerator.CodeAnalysis.CodeAnalyzers;
using TestSetupGenerator.CodeAnalysis.CodeGenerators;

namespace TestSetupGenerator.CodeAnalysis
{
    public interface IFieldDeclarationsBuilder
    {
        IEnumerable<SyntaxNode> GetFieldDeclarations(ClassDeclarationSyntax classUnderTestDeclarationSyntax, SyntaxGenerator syntaxGenerator);
        IEnumerable<SyntaxNode> GetFieldDeclarationsAsGeneric(ClassDeclarationSyntax classUnderTestDeclarationSyntax, string genericSymbol, SyntaxGenerator syntaxGenerator);
    }

    public class FieldDeclarationsBuilder : IFieldDeclarationsBuilder
    {
        private readonly IConstructorParametersExtractor _constructorParametersExtractor;
        private readonly IFieldDeclarationGenerator _fieldDeclarationGenerator;


        public FieldDeclarationsBuilder(IFieldDeclarationGenerator fieldDeclarationGenerator, IConstructorParametersExtractor constructorParametersExtractor)
        {
            _fieldDeclarationGenerator = fieldDeclarationGenerator;
            _constructorParametersExtractor = constructorParametersExtractor;
        }

        public IEnumerable<SyntaxNode> GetFieldDeclarations(ClassDeclarationSyntax classUnderTestDeclarationSyntax, SyntaxGenerator syntaxGenerator)
        {
            var constructorParameters = _constructorParametersExtractor.GetParametersOfConstructor(classUnderTestDeclarationSyntax).ToList();
            if (constructorParameters.Any())
            {
                var fieldDeclarations = new List<SyntaxNode>();

                foreach (var parameter in constructorParameters)
                {
                    var fieldDec = _fieldDeclarationGenerator.GetFieldDeclaration(parameter, syntaxGenerator);
                    fieldDeclarations.Add(fieldDec);
                }

                return fieldDeclarations;
            }

            return new List<SyntaxNode>();
        }

        public IEnumerable<SyntaxNode> GetFieldDeclarationsAsGeneric(ClassDeclarationSyntax classUnderTestDeclarationSyntax, string genericSymbol,
            SyntaxGenerator syntaxGenerator)
        {
            var constructorParameters = _constructorParametersExtractor.GetParametersOfConstructor(classUnderTestDeclarationSyntax).ToList();
            if (constructorParameters.Any())
            {
                var fieldDeclarations = new List<SyntaxNode>();

                foreach (var parameter in constructorParameters)
                {
                    var fieldDec = _fieldDeclarationGenerator.GetGenericFieldDeclaration(parameter, genericSymbol, syntaxGenerator);
                    fieldDeclarations.Add(fieldDec);
                }

                return fieldDeclarations;
            }

            return new List<SyntaxNode>();
        }
    }
}