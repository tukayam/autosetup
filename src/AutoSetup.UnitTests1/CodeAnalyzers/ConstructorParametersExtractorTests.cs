using System.Linq;
using AutoSetup.CodeAnalyzers;
using AutoSetup.UnitTests.Helpers.RoslynStubProviders;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace AutoSetup.UnitTests.CodeAnalyzers
{
    public class ConstructorParametersExtractorTests
    {
        private ConstructorParametersExtractor _target;
        public ConstructorParametersExtractorTests()
        {
            _target = new ConstructorParametersExtractor();
        }

        [Fact]
        public void ReturnsAllConstructorParameters()
        {
            var textFilePath = "files.Class_WithConstructorWithTwoParameters.txt";
            var classDeclarationSyntax = SyntaxNodeProvider.GetSyntaxNodeFromFile<ClassDeclarationSyntax>(textFilePath, "TestClass");

            var actual = _target.GetParametersOfConstructor(classDeclarationSyntax).ToList();

            Assert.Equal(2, actual.Count);

            var firstParameter = actual.First();
            Assert.Equal("someType",firstParameter.Identifier.Text);
            Assert.Equal("ISomeType", firstParameter.Type.ToString());

            var secondParameter = actual.Skip(1).First();
            Assert.Equal("someOtherType", secondParameter.Identifier.Text);
            Assert.Equal("ISomeOtherType", secondParameter.Type.ToString());
        }
    }
}
