using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using TestSetupGenerator.CodeAnalysis.CodeAnalyzers;
using TestSetupGenerator.CodeAnalysis.Models;
using TestSetupGenerator.UnitTests.Helpers.RoslynStubProviders;
using TestSetupGenerator.XUnitMoq;
using Xunit;
using TestSetupGenerator.UnitTests.Helpers.ExtensionMethods;
using Xunit.Abstractions;

namespace TestSetupGenerator.UnitTests
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

            var iocConfig=new IoCConfig();
            var container = iocConfig.Container;
            container.RegisterInstance(_classUnderTestFinder.Object);

            _target = iocConfig.Container.GetInstance<IXUnitSetupGenerator>();
        }

        [Theory]
        [InlineData("files.TestClass_NoSetupConstructor.txt")]
        [InlineData("files.TestClass_WithSetupConstructor.txt")]
        public async Task RegeneratesSetup_When_NoAdditionalLinesMustBeKept(string filePath)
        {
            var document = DocumentProvider.CreateDocumentFromFile(filePath);
            var root = await document.GetSyntaxRootAsync();
            var testClass = root.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().First();

            var classUnderTest = SyntaxNodeProvider.GetSyntaxNodeFromFile<ClassDeclarationSyntax>("files.ClassUnderTest.txt", "ClassUnderTest");
            _classUnderTestFinder.Setup(_ => _.GetAsync(It.IsAny<Solution>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new ClassUnderTest(classUnderTest, null));

            var actual = await _target.RegenerateSetup(document, testClass, new CancellationToken());
            var actualText = await actual.GetTextAsync();
            _testOutput.WriteLine(actualText.ToString());

            var expected = await DocumentProvider.CreateDocumentFromFile("files.TestClass_WithFinalSetupConstructor.txt").GetTextAsync();

            Assert.Equal(expected.ToString().RemoveWhitespace(), actualText.ToString().RemoveWhitespace());
        }
    }
}
