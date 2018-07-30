using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace TestSetupGenerator.CodeAnalysis.CodeGenerators
{
    public interface IFieldDeclarationGenerator
    {
        SyntaxNode GetFieldDeclaration(ParameterSyntax parameter, SyntaxGenerator generator);
        SyntaxNode GetGenericFieldDeclaration(ParameterSyntax parameter, string genericSymbol, SyntaxGenerator generator);
    }

    public class FieldDeclarationGenerator : IFieldDeclarationGenerator
    {
        private readonly IFieldNameGenerator _fieldNameGenerator;

        public FieldDeclarationGenerator(IFieldNameGenerator fieldNameGenerator)
        {
            _fieldNameGenerator = fieldNameGenerator;
        }
        
        public SyntaxNode GetFieldDeclaration(ParameterSyntax parameter, SyntaxGenerator generator)
        {
            var fieldName = _fieldNameGenerator.GetFromParameter(parameter);
            return generator.FieldDeclaration(fieldName
                , parameter.Type
                , Accessibility.Private
                , DeclarationModifiers.ReadOnly);
        }

        public SyntaxNode GetGenericFieldDeclaration(ParameterSyntax parameter, string genericSymbol, SyntaxGenerator generator)
        {
            var fieldName = _fieldNameGenerator.GetFromParameter(parameter);

            var parameterTypeIdentifier = generator.IdentifierName(parameter.Type.ToString());
            var type = generator.GenericName(genericSymbol, parameterTypeIdentifier);

            return generator.FieldDeclaration(fieldName
                , type
                , Accessibility.Private
                , DeclarationModifiers.ReadOnly);
        }
    }
}
