using AutoSetup.CodeAnalysis.CodeGenerators;
using AutoSetup.CodeAnalysis.UnitTests.Helpers.RoslynStubProviders;
using Microsoft.CodeAnalysis;
using Xunit;

namespace AutoSetup.CodeAnalysis.UnitTests.CodeGenerators
{
    public class ExpressionStatementGeneratorTests_MoqStub
    {
        private ExpressionStatementGenerator _target;
        public ExpressionStatementGeneratorTests_MoqStub()
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
                _target.MoqStubAssignmentExpression(parameterType, fieldName, syntaxGenerator);
            var asText = actual.NormalizeWhitespace().ToFullString();

            var expected = "_someType = (new Mock<ISomeType>())";
            Assert.Equal(expected,asText);
        }
    }
}