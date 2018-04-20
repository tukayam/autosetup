using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestSetupGenerator.CodeAnalysis.CodeAnalyzers
{
    public class FieldNameGenerator
    {
        public string GetFromParameter(ParameterSyntax parameter)
        {
            var parameterName = parameter.Identifier.Text;
            return parameterName.TrimStart('_').Insert(0, "_");
        }
    }
}
