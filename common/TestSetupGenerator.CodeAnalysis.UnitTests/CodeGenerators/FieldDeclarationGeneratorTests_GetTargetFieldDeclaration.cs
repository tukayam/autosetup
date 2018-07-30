using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using TestSetupGenerator.CodeAnalysis.CodeGenerators;
using TestSetupGenerator.CodeAnalysis.UnitTests.Helpers.RoslynStubProviders;
using Xunit;

namespace TestSetupGenerator.CodeAnalysis.UnitTests.CodeGenerators
{
    public class FieldDeclarationGeneratorTests_GetTargetFieldDeclaration
    {
        private readonly Mock<IFieldNameGenerator> _fieldNameGenerator;
        private FieldDeclarationGenerator _target;

        public FieldDeclarationGeneratorTests_GetTargetFieldDeclaration()
        {
            _fieldNameGenerator = new Mock<IFieldNameGenerator>();
            _target = new FieldDeclarationGenerator(_fieldNameGenerator.Object);
        }

        [Fact]
        public void Returns_TargetFieldDeclaration()
        {
            var syntaxGenerator = new SyntaxGeneratorProvider().GetSyntaxGenerator();

            var classundertestName = "ClassUnderTest";
            var actual = _target.GetTargetFieldDeclaration(classundertestName, syntaxGenerator) as FieldDeclarationSyntax;

            var actualTextWithoutSpaces = actual.GetText().ToString().Replace(" ", "");

            var expected = "private readonly ClassUnderTest _target;";
            var expectedTextWithoutSpaces = expected.Replace(" ", "");
            Assert.Equal(expectedTextWithoutSpaces, actualTextWithoutSpaces);
        }
    }
}
