using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace TestSetupGenerator.CodeAnalysis.CodeGenerators
{
    public class FieldDeclarationGenerator
    {
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