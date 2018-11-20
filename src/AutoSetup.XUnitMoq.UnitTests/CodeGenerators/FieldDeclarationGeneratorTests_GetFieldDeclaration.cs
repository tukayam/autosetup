using AutoSetup.XUnitMoq.CodeGenerators;
using AutoSetup.XUnitMoq.UnitTests.Helpers.RoslynStubProviders;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using Xunit;

namespace AutoSetup.XUnitMoq.UnitTests.CodeGenerators
{
    public class FieldDeclarationGeneratorTests_GetFieldDeclaration
    {
        private readonly Mock<IFieldNameGenerator> _fieldNameGenerator;
        private FieldDeclarationGenerator _target;

        public FieldDeclarationGeneratorTests_GetFieldDeclaration()
        {
            _fieldNameGenerator = new Mock<IFieldNameGenerator>();
            _target = new FieldDeclarationGenerator(_fieldNameGenerator.Object);

            _fieldNameGenerator.Setup(_ => _.GetFromParameter(It.IsAny<ParameterSyntax>()))
                .Returns("_fieldName");
        }

        [Theory]
        [InlineData("files.Class_WithConstructorWithTwoParameters.txt", "someType", "private readonly ISomeType _fieldName;")]
        [InlineData("files.Class_WithConstructorWithTwoParameters.txt", "someOtherType", "private readonly ISomeOtherType _fieldName;")]
        public void Returns_FieldDeclarationWithCorrectType(string filePath, string parameterName, string expected)
        {
            var parameterSyntax = SyntaxNodeProvider.GetSyntaxNodeFromFile<ParameterSyntax>(filePath, parameterName);
            var syntaxGenerator = new SyntaxGeneratorProvider().GetSyntaxGenerator();

            var actual = _target.GetFieldDeclaration(parameterSyntax, syntaxGenerator) as FieldDeclarationSyntax;

            var actualTextWithoutSpaces = actual.GetText().ToString().Replace(" ", "");
            var expectedTextWithoutSpaces = expected.Replace(" ", "");
            Assert.Equal(expectedTextWithoutSpaces, actualTextWithoutSpaces);
        }
    }
}
