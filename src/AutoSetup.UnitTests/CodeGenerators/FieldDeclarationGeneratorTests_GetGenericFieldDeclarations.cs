using AutoSetup.CodeGenerators;
using AutoSetup.UnitTests.Helpers.RoslynStubProviders;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using Xunit;

namespace AutoSetup.UnitTests.CodeGenerators
{
    public class FieldDeclarationGeneratorTests_GetGenericFieldDeclarations
    {
        private readonly Mock<IFieldNameGenerator> _fieldNameGenerator;
        private FieldDeclarationGenerator _target;

        public FieldDeclarationGeneratorTests_GetGenericFieldDeclarations()
        {
            _fieldNameGenerator = new Mock<IFieldNameGenerator>();
            _target = new FieldDeclarationGenerator(_fieldNameGenerator.Object);

            _fieldNameGenerator.Setup(_ => _.GetFromParameter(It.IsAny<ParameterSyntax>()))
                .Returns("_fieldName");
        }

        [Theory]
        [InlineData("files.Class_WithConstructorWithTwoParameters.txt", "someType", "private readonly Mock<ISomeType> _fieldName;")]
        [InlineData("files.Class_WithConstructorWithTwoParameters.txt", "someOtherType", "private readonly Mock<ISomeOtherType> _fieldName;")]
        public void Returns_FieldDeclarationsWithCorrectName_ForEach_Parameter(string filePath, string parameterName, string expected)
        {
            var parameterSyntax = SyntaxNodeProvider.GetSyntaxNodeFromFile<ParameterSyntax>(filePath, parameterName);
            var syntaxGenerator = new SyntaxGeneratorProvider().GetSyntaxGenerator();

            var actual = _target.GetGenericFieldDeclaration(parameterSyntax, "Mock", syntaxGenerator);

            var actualTextWithoutSpaces = actual.GetText().ToString().Replace(" ", "");
            var expectedTextWithoutSpaces = expected.Replace(" ", "");
            Assert.Equal(expectedTextWithoutSpaces, actualTextWithoutSpaces);
        }
    }
}
