using Microsoft.CodeAnalysis;
using TestSetupGenerator.CodeAnalysis.CodeGenerators;
using TestSetupGenerator.CodeAnalysis.UnitTests.Helpers;
using TestSetupGenerator.CodeAnalysis.UnitTests.Helpers.RoslynStubProviders;
using Xunit;

namespace TestSetupGenerator.CodeAnalysis.UnitTests.CodeGenerators
{
    public class ExpressionStatementGeneratorTests_MockRepositoryGenerateStub
    {
        private ExpressionStatementGenerator _target;
        public ExpressionStatementGeneratorTests_MockRepositoryGenerateStub()
        {
            _target = new ExpressionStatementGenerator();
        }

        [Fact]
        public void Returns_FieldAssignmentExpression_When_ParameterValuesAreValid()
        {
            var parameterType = "ISomeType";
            var fieldName = "_someType";
            var syntaxGenerator = new SyntaxGeneratorProvider().GetSyntaxGenerator();

            var actual =
                _target.MockRepositoryGenerateStubAssignmentExpression(parameterType, fieldName, syntaxGenerator);
            var asText = actual.NormalizeWhitespace().ToFullString();

            var expected = "_someType = (MockRepository.GenerateStub<ISomeType>())";
            Assert.Equal(expected,asText);
        }
    }
}
