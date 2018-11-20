using AutoSetup.CodeGenerators;
using AutoSetup.UnitTests.Helpers.RoslynStubProviders;
using Microsoft.CodeAnalysis;
using Xunit;

namespace AutoSetup.UnitTests.CodeGenerators
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
                _target.RhinoMocksStubAssignmentExpression(parameterType, fieldName, syntaxGenerator);
            var asText = actual.NormalizeWhitespace().ToFullString();

            var expected = "_someType = (MockRepository.GenerateStub<ISomeType>())";
            Assert.Equal(expected,asText);
        }
    }
}
