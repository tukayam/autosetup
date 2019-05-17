using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoSetup.CodeAnalyzers;
using AutoSetup.IntegrationTests.Helpers.ExtensionMethods;
using AutoSetup.IntegrationTests.Helpers.RoslynStubProviders;
using AutoSetup.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace AutoSetup.IntegrationTests
{
    public class XUnitSetupGeneratorTests
    {
        private readonly ITestOutputHelper _testOutput;
        private readonly Mock<IClassUnderTestFinder> _classUnderTestFinder;
        private readonly IXUnitSetupGenerator _target;

        public XUnitSetupGeneratorTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;

            _classUnderTestFinder = new Mock<IClassUnderTestFinder>();
            _target = new IoCConfig().GetInstance(_classUnderTestFinder.Object);
        }

        [Theory]
        [InlineData("files.TestClass_WithSetupConstructor.txt")]
        [InlineData("files.TestClass_DifferentConstructor.txt")]
        [InlineData("files.TestClass_NoConstructor.txt")]
        [InlineData("files.TestClass_WithExistingField.txt")]
        public async Task RewritesDefaultConstructor(string filePath)
        {
            // Arrange
            var filePathClassUnderTest = "files.ClassUnderTest.txt";
            var filePaths = new[] { filePath, filePathClassUnderTest };
            var documents = DocumentProvider.CreateCompilationAndReturnDocuments(filePaths);

            var documentWithTestClass = documents[filePath];
            var root = await documentWithTestClass.GetSyntaxRootAsync();
            var testClass = root.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().First();

            var documentWithClassUnderTest = documents[filePathClassUnderTest];
            var classUnderTest = SyntaxNodeProvider.GetSyntaxNodeFromDocument<ClassDeclarationSyntax>(documentWithClassUnderTest, "ClassUnderTest");
            var semanticModel = await documentWithClassUnderTest.GetSemanticModelAsync();
            _classUnderTestFinder.Setup(_ => _.GetAsync(It.IsAny<Solution>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new ClassUnderTest(classUnderTest, semanticModel));

            // Act
            var actual = await _target.RegenerateSetup(documentWithTestClass, testClass, new CancellationToken());

            // Assert
            var actualText = await actual.GetTextAsync();
            _testOutput.WriteLine(actualText.ToString().Replace(";", ";\n"));

            var expected = await DocumentProvider.CreateDocumentFromFile("files.TestClass_WithFinalSetupConstructor.txt").GetTextAsync();

            Assert.Equal(expected.ToString().RemoveWhitespace(), actualText.ToString().RemoveWhitespace());
        }
    }
}
