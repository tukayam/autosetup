using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestSetupGenerator.CodeAnalysis.CodeAnalyzers;
using TestSetupGenerator.CodeAnalysis.CodeGenerators;
using TestSetupGenerator.CodeAnalysis.UnitTests.Helpers.RoslynStubProviders;
using Xunit;

namespace TestSetupGenerator.CodeAnalysis.UnitTests.CodeAnalyzers
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
