using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestSetupGenerator.CodeAnalysis.CodeGenerators;
using TestSetupGenerator.CodeAnalysis.UnitTests.Helpers.RoslynStubProviders;
using Xunit;

namespace TestSetupGenerator.CodeAnalysis.UnitTests.CodeGenerators
{
    public class ExpressionStatementGeneratorTests_TargetObjectAssignmentExpression
    {
        private ExpressionStatementGenerator _target;
        public ExpressionStatementGeneratorTests_TargetObjectAssignmentExpression()
        {
            _target = new ExpressionStatementGenerator();
        }

        [Fact]
        public void Returns_TargetWithAllDependencies()
        {
            var filePath = "files.Class_WithUnitTestsForClassWithParameters.txt";
            var fieldDeclarations = SyntaxNodeProvider.GetAllSyntaxNodesFromFile<FieldDeclarationSyntax>(filePath);
            var syntaxGenerator = new SyntaxGeneratorProvider().GetSyntaxGenerator();

            var actual = _target.TargetObjectAssignmentExpression(fieldDeclarations, "ClassUnderTest", syntaxGenerator);

            var asText = actual.GetText().ToString();
            Assert.Equal("_target=(newClassUnderTest(_someType,_someOtherType))", asText);
        }
    }
}