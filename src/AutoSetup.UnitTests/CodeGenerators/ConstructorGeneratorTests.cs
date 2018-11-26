using AutoSetup.CodeGenerators;
using AutoSetup.UnitTests.Helpers.ExtensionMethods;
using AutoSetup.UnitTests.Helpers.IO;
using AutoSetup.UnitTests.Helpers.RoslynStubProviders;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;
using System.Linq;

namespace AutoSetup.UnitTests.CodeGenerators
{
    public class ConstructorGeneratorTests
    {
        private readonly ConstructorGenerator _target;
        public ConstructorGeneratorTests()
        {
            _target = new ConstructorGenerator();
        }

        [Theory]
        [InlineData("files.ConstructorGenerator_DocumentWithTwoClasses.txt", "ClassA", "public ClassA(){}")]
        [InlineData("files.ConstructorGenerator_DocumentWithTwoClasses.txt", "ClassB", "public ClassB(){}")]
        public void ReturnsEmptyConstructor_When_MethodBodyIsNull(string filePath, string className, string expected)
        {
            var source = TextFileReader.ReadFile(filePath);
            var syntaxGenerator = new SyntaxGeneratorProvider().GetSyntaxGenerator(source);

            var actual = _target.Constructor(className, null, syntaxGenerator);

            Assert.Equal(expected.RemoveWhitespace(), actual.GetText().ToString());
        }

        [Fact]
        public void ReturnsConstructorWithBody()
        {
            var filePath = "files.ConstructorGenerator_DocumentWithOneClassAndVariables.txt";
            var source = TextFileReader.ReadFile(filePath);
            var methodBody = SyntaxNodeProvider.GetAllSyntaxNodesFromFile<LocalDeclarationStatementSyntax>(filePath);
            var syntaxGenerator = new SyntaxGeneratorProvider().GetSyntaxGenerator(source);
            var className = "ClassA";

            var actual = _target.Constructor(className, methodBody, syntaxGenerator);

            Assert.Single(actual.DescendantNodes().OfType<LocalDeclarationStatementSyntax>());
        }
    }
}
