using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using TestSetupGenerator.CodeAnalysis.CodeGenerators;
using TestSetupGenerator.CodeAnalysis.UnitTests.Helpers.RoslynStubProviders;
using Xunit;

namespace TestSetupGenerator.CodeAnalysis.UnitTests.DocumentBuilderTests
{
    public class DocumentBuilderTests_WithSetupMethod
    {
        private readonly Mock<IMemberReplacer> _memberReplacer;
        private DocumentBuilder _target;
        public DocumentBuilderTests_WithSetupMethod()
        {
            _memberReplacer = new Mock<IMemberReplacer>();

           
        }

        [Fact]
        public async Task AddsNewSetupMethodToClass()
        {
            var filePath = "DocumentBuilderTests.files.EmptyTestClass.txt";
            var document = DocumentProvider.CreateDocumentFromFile(filePath);
            var root = document.GetSyntaxRootAsync().Result;
            var testClass = root.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().First();

            _target = new DocumentBuilder(_memberReplacer.Object, document, testClass);

            var newSetupMethod =
                SyntaxNodeProvider.GetSyntaxNodeFromFile<MethodDeclarationSyntax>(
                    "DocumentBuilderTests.files.SampleMethod.txt", "SampleMethod");
            _memberReplacer.Setup(_ => _.ReplaceOrInsert(It.Is<ClassDeclarationSyntax>(t => t == testClass),
                It.Is<MemberDeclarationSyntax>(s => s == newSetupMethod)));
       
            var actual = await _target.WithSetupMethodAsync(newSetupMethod)
                                 .Build();

            var actualRoot = await actual.GetSyntaxRootAsync();
            var actualSetupMethod = actualRoot.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
            Assert.Equal(newSetupMethod.GetText().ToString(), actualSetupMethod.GetText().ToString());
        }
    }
}
