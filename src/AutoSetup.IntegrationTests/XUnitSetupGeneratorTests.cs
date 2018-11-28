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
        [InlineData("files.TestClass_WithExistingField.txt", Skip = "Works in unit tests but not here for some reason. Must check the whole integration test setup.")]
        public async Task RewritesDefaultConstructor(string filePath)
        {
            var document = DocumentProvider.CreateDocumentFromFile(filePath);
            var root = await document.GetSyntaxRootAsync();
            var testClass = root.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().First();

            var classUnderTest = SyntaxNodeProvider.GetSyntaxNodeFromFile<ClassDeclarationSyntax>("files.ClassUnderTest.txt", "ClassUnderTest");
            _classUnderTestFinder.Setup(_ => _.GetAsync(It.IsAny<Solution>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new ClassUnderTest(classUnderTest, null));

            var actual = await _target.RegenerateSetup(document, testClass, new CancellationToken());
            var actualText = await actual.GetTextAsync();
            _testOutput.WriteLine(actualText.ToString().Replace(";", ";\n"));

            var expected = await DocumentProvider.CreateDocumentFromFile("files.TestClass_WithFinalSetupConstructor.txt").GetTextAsync();

            Assert.Equal(expected.ToString().RemoveWhitespace(), actualText.ToString().RemoveWhitespace());
        }
    }
}
