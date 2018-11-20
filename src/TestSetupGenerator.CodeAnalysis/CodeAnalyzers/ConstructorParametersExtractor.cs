using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoSetup.CodeAnalysis.CodeAnalyzers
{
    public interface IConstructorParametersExtractor
    {
        IEnumerable<ParameterSyntax> GetParametersOfConstructor(ClassDeclarationSyntax classDec);
    }

    public class ConstructorParametersExtractor : IConstructorParametersExtractor
    {
        public IEnumerable<ParameterSyntax> GetParametersOfConstructor(ClassDeclarationSyntax classDec)
        {
            var constructorWithParameters =
                classDec.DescendantNodes()
                    .OfType<ConstructorDeclarationSyntax>()
                    .FirstOrDefault(
                        x => x.ParameterList.Parameters.Any());
            if (constructorWithParameters != null)
            {
                var constructorParam = constructorWithParameters.ParameterList.Parameters;
                return constructorParam;
            }

            return new List<ParameterSyntax>();
        }
    }
}
