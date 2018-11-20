using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoSetup.CodeAnalysis.CodeGenerators
{
    public interface IFieldNameGenerator
    {
        string GetFromParameter(ParameterSyntax parameter);
    }

    public class FieldNameGenerator : IFieldNameGenerator
    {
        public string GetFromParameter(ParameterSyntax parameter)
        {
            var parameterName = parameter.Identifier.Text;
            return parameterName.TrimStart('_').Insert(0, "_");
        }
    }
}
