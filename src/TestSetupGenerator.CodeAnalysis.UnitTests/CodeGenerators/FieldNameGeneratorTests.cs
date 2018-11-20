using AutoSetup.CodeAnalysis.CodeGenerators;
using AutoSetup.CodeAnalysis.UnitTests.Helpers.RoslynStubProviders;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace AutoSetup.CodeAnalysis.UnitTests.CodeGenerators
{
    public class FieldNameGeneratorTests
    {
        private readonly FieldNameGenerator _target;
        public FieldNameGeneratorTests()
        {
            _target = new FieldNameGenerator();
        }

        [Theory]
        [InlineData("files.Class_WithConstructorWithTwoParameters.txt", "someType", "_someType")]
        [InlineData("files.Class_WithConstructorWithTwoParameters.txt", "someOtherType", "_someOtherType")]
        public void Returns_CorrectName_For_Parameter(string filePath, string parameterName, string expected)
        {
            var parameterSyntax = SyntaxNodeProvider.GetSyntaxNodeFromFile<ParameterSyntax>(filePath, parameterName);

            var actual = _target.GetFromParameter(parameterSyntax);

            Assert.Equal(expected, actual);
        }
    }
}
