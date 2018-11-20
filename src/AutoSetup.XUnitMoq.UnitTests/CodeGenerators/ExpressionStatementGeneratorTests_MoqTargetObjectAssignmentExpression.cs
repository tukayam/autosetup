using AutoSetup.XUnitMoq.CodeGenerators;
using AutoSetup.XUnitMoq.UnitTests.Helpers.RoslynStubProviders;
using Xunit;

namespace AutoSetup.XUnitMoq.UnitTests.CodeGenerators
{
    public class ExpressionStatementGeneratorTests_MoqTargetObjectAssignmentExpression
    {
        private ExpressionStatementGenerator _target;
        public ExpressionStatementGeneratorTests_MoqTargetObjectAssignmentExpression()
        {
            _target = new ExpressionStatementGenerator();
        }

        [Fact]
        public void Returns_TargetWithAllDependencies()
        {
            var fieldDeclarationNames = new[] { "_someType" , "_someOtherType" };
            var syntaxGenerator = new SyntaxGeneratorProvider().GetSyntaxGenerator();

            var actual = _target.MoqTargetObjectAssignmentExpression(fieldDeclarationNames, "ClassUnderTest", syntaxGenerator);

            var asText = actual.GetText().ToString();
            Assert.Equal("_target=(newClassUnderTest(_someType.Object,_someOtherType.Object))", asText);
        }
    }
}