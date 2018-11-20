using AutoSetup.XUnitMoq.CodeGenerators;
using AutoSetup.XUnitMoq.UnitTests.Helpers.RoslynStubProviders;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace AutoSetup.XUnitMoq.UnitTests.CodeAnalyzers
{
    public class FieldNameGeneratorTests
    {
        private readonly FieldNameGenerator _target;
        public FieldNameGeneratorTests()
        {
            _target = new FieldNameGenerator();
        }

        [Theory]
        [InlineData("files.FieldNameGenerator_MultipleParameters.txt", "someType", "_someType")]
        [InlineData("files.FieldNameGenerator_MultipleParameters.txt", "someOtherType", "_someOtherType")]
        [InlineData("files.FieldNameGenerator_MultipleParameters.txt", "stringType", "_stringType")]
        public void GeneratesFieldNameWithUnderscoreAndLowerCase(string filePath, string parameterType, string expected)
        {
            var parameterSyntax = SyntaxNodeProvider.GetSyntaxNodeFromFile<ParameterSyntax>(filePath, parameterType);

            var actual = _target.GetFromParameter(parameterSyntax);

            Assert.Equal(expected, actual);
        }
    }
}
