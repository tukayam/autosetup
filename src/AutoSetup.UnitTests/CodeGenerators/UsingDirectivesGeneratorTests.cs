using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoSetup.CodeGenerators;
using AutoSetup.UnitTests.Helpers.IO;
using AutoSetup.UnitTests.Helpers.RoslynStubProviders;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace AutoSetup.UnitTests.CodeGenerators
{
    public class UsingDirectivesGeneratorTests
    {
        private UsingDirectivesGenerator _target;

        public UsingDirectivesGeneratorTests()
        {
            _target = new UsingDirectivesGenerator();
        }

        [Fact]
        public void Adds_Using_For_MockingFramework()
        {
            var syntaxGenerator = new SyntaxGeneratorProvider().GetSyntaxGenerator();

            var rhinoMocksUsing = "rhino.mocks";

            var usingDirectives = _target.TestingFrameworkUsingDirectives(new List<string> { rhinoMocksUsing }, syntaxGenerator);
            Assert.Equal("rhino.mocks", usingDirectives.First().Name.ToString());
        }

        [Theory]
        [InlineData("files.Class_WithConstructorWithOneParameterUnderNamespace.txt", "someType", "someNamespace")]
        [InlineData("files.Class_WithConstructorWithOneParameterUnderAnotherNamespace.txt", "someType", "someNamespace")]
        [InlineData("files.Class_WithConstructorWithMultipleParametersUnderNamespaces.txt", "someType", "someNamespace")]
        [InlineData("files.Class_WithConstructorWithMultipleParametersUnderNamespaces.txt", "anotherType", "anotherNamespace")]
        public async Task Adds_Usings_For_All_Parameters(string filePath, string parameterName, string expectedNamespaces)
        {
            var source = TextFileReader.ReadFile(filePath);
            var document = DocumentProvider.CreateDocument(source);

            var parameterSyntax = SyntaxNodeProvider.GetSyntaxNodeFromDocument<ParameterSyntax>(document, parameterName);
            var syntaxGenerator = new SyntaxGeneratorProvider().GetSyntaxGenerator();

            var semanticModel = await document.GetSemanticModelAsync();

            var usingDirectives = _target.UsingDirectives(semanticModel.Compilation, new[] { parameterSyntax }, new List<string>(), syntaxGenerator);

            Assert.Equal(expectedNamespaces, usingDirectives.First().Name.ToString());
        }
    }
}
