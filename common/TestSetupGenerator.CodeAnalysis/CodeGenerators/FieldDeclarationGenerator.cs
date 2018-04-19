using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace TestSetupGenerator.CodeAnalysis.CodeGenerators
{
    public class FieldDeclarationGenerator
    {
        public IEnumerable<SyntaxNode> GetFieldDeclarations(ClassDeclarationSyntax classUnderTestDeclarationSyntax, SyntaxGenerator syntaxGenerator)
        {
            var constructorWithParameters =
                classUnderTestDeclarationSyntax.DescendantNodes()
                    .OfType<ConstructorDeclarationSyntax>()
                    .FirstOrDefault(
                        x => x.ParameterList.Parameters.Any());
            if (constructorWithParameters != null)
            {
                var constructorParameterTypes = constructorWithParameters.ParameterList.Parameters.Select(_ => _.Type);
                return GetFieldDeclarations(constructorParameterTypes, syntaxGenerator);
            }

            return new List<SyntaxNode>();
        }

        public IEnumerable<SyntaxNode> GetFieldDeclarations(IEnumerable<TypeSyntax> parameterTypes, SyntaxGenerator syntaxGenerator)
        {
            var fieldDeclarations = new List<SyntaxNode>();

            foreach (var parameterType in parameterTypes)
            {
                var fieldName = GetParameterFieldName(parameterType);
                var fieldDec = syntaxGenerator.FieldDeclaration(fieldName
                    , parameterType
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

        private string GetParameterFieldName(TypeSyntax parameterType)
        {
            var parameterTypeName = parameterType.ToString();
            var isInterface = parameterTypeName.Substring(0, 1) == "I" &&
                              parameterTypeName.Substring(1, 2).ToLower() != parameterTypeName.Substring(1, 2);
            parameterTypeName = isInterface ? parameterTypeName.Replace("I", string.Empty) : parameterTypeName;
            return string.Format("_{0}", parameterTypeName.Substring(0, 1).ToLowerInvariant() + parameterTypeName.Substring(1));
        }
    }
}