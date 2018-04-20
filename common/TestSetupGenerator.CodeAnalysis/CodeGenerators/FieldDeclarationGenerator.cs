using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using TestSetupGenerator.CodeAnalysis.CodeAnalyzers;

namespace TestSetupGenerator.CodeAnalysis.CodeGenerators
{
    public class FieldDeclarationGenerator
    {
        private readonly FieldNameGenerator _fieldNameGenerator;

        public FieldDeclarationGenerator()
        {
            _fieldNameGenerator = new FieldNameGenerator();
        }

        public IEnumerable<SyntaxNode> GetFieldDeclarations(ClassDeclarationSyntax classUnderTestDeclarationSyntax, SyntaxGenerator syntaxGenerator)
        {
            var constructorWithParameters =
                classUnderTestDeclarationSyntax.DescendantNodes()
                    .OfType<ConstructorDeclarationSyntax>()
                    .FirstOrDefault(
                        x => x.ParameterList.Parameters.Any());
            if (constructorWithParameters != null)
            {
                var constructorParameters = constructorWithParameters.ParameterList.Parameters;
                return GetFieldDeclarations(constructorParameters, syntaxGenerator);
            }

            return new List<SyntaxNode>();
        }

        public IEnumerable<SyntaxNode> GetFieldDeclarations(IEnumerable<ParameterSyntax> parameters, SyntaxGenerator syntaxGenerator)
        {
            var fieldDeclarations = new List<SyntaxNode>();

            foreach (var parameter in parameters)
            {
                var fieldName = _fieldNameGenerator.GetFromParameter(parameter);
                var fieldDec = syntaxGenerator.FieldDeclaration(fieldName
                    , parameter.Type
                    , Accessibility.Private);
                fieldDeclarations.Add(fieldDec);
            }

            return fieldDeclarations;
        }

        public IEnumerable<SyntaxNode> ReplaceFieldDeclarations(ClassDeclarationSyntax classUnderTestDeclarationSyntax, SyntaxList<MemberDeclarationSyntax> members, SyntaxGenerator generator)
        {
            IEnumerable<SyntaxNode> fieldDeclarations = GetFieldDeclarations(classUnderTestDeclarationSyntax, generator);

            var existingFieldDeclarationVariables = members.OfType<FieldDeclarationSyntax>()
                .SelectMany(_ => _.Declaration.Variables).Select(_ => _.Identifier.Text).ToList();

            var index = 0;
            //foreach field replace or add
            foreach (FieldDeclarationSyntax fieldDeclaration in fieldDeclarations)
            {
                var fieldDeclarationText = fieldDeclaration.Declaration.Variables.Select(_ => _.Identifier.Text);
                if (!existingFieldDeclarationVariables.Any(_ => fieldDeclarationText.Contains(_)))
                {
                    members = members.Insert(index++, fieldDeclaration);
                }
            }

            return members;
        }
    }
}