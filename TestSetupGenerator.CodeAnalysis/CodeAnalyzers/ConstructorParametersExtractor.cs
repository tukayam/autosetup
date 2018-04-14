using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestSetupGenerator.CodeAnalysis.CodeAnalyzers
{
    public class ConstructorParametersExtractor
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
