using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace AutoSetup.CodeAnalysis.CodeGenerators
{
    public interface IFieldDeclarationGenerator
    {
        FieldDeclarationSyntax GetTargetFieldDeclaration(string classUnderTestName, SyntaxGenerator generator);
        FieldDeclarationSyntax GetFieldDeclaration(ParameterSyntax parameter, SyntaxGenerator generator);
        FieldDeclarationSyntax GetGenericFieldDeclaration(ParameterSyntax parameter, string genericSymbol, SyntaxGenerator generator);
    }

    public class FieldDeclarationGenerator : IFieldDeclarationGenerator
    {
        private readonly IFieldNameGenerator _fieldNameGenerator;

        public FieldDeclarationGenerator(IFieldNameGenerator fieldNameGenerator)
        {
            _fieldNameGenerator = fieldNameGenerator;
        }

        public FieldDeclarationSyntax GetTargetFieldDeclaration(string classUnderTestName, SyntaxGenerator generator)
        {
            return generator.FieldDeclaration("_target"
                , generator.IdentifierName(classUnderTestName)
                , Accessibility.Private
                , DeclarationModifiers.ReadOnly) as FieldDeclarationSyntax;
        }

        public FieldDeclarationSyntax GetFieldDeclaration(ParameterSyntax parameter, SyntaxGenerator generator)
        {
            var fieldName = _fieldNameGenerator.GetFromParameter(parameter);
            return generator.FieldDeclaration(fieldName
                , parameter.Type
                , Accessibility.Private
                , DeclarationModifiers.ReadOnly) as FieldDeclarationSyntax;
        }

        public FieldDeclarationSyntax GetGenericFieldDeclaration(ParameterSyntax parameter, string genericSymbol, SyntaxGenerator generator)
        {
            var fieldName = _fieldNameGenerator.GetFromParameter(parameter);

            var parameterTypeIdentifier = generator.IdentifierName(parameter.Type.ToString());
            var type = generator.GenericName(genericSymbol, parameterTypeIdentifier);

            return generator.FieldDeclaration(fieldName
                , type
                , Accessibility.Private
                , DeclarationModifiers.ReadOnly) as FieldDeclarationSyntax;
        }
    }
}
