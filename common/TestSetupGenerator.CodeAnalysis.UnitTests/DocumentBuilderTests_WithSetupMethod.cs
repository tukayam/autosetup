using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using TestSetupGenerator.CodeAnalysis.CodeAnalyzers;
using TestSetupGenerator.CodeAnalysis.UnitTests.Helpers.RoslynStubProviders;
using Xunit;

namespace TestSetupGenerator.CodeAnalysis.UnitTests
{
    public class DocumentBuilderTests_WithSetupMethod
    {
        private readonly Mock<IMemberFinder> _memberFinder;
        private DocumentBuilder _target;
        public DocumentBuilderTests_WithSetupMethod()
        {
            _memberFinder = new Mock<IMemberFinder>();
        }

        [Fact]
        public async Task AddsNewSetupMethodToClass()
        {
            var filePath = "files.EmptyTestClass.txt";
            var document = DocumentProvider.CreateDocumentFromFile(filePath);
            var root = document.GetSyntaxRootAsync().Result;
            var testClass = root.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().First();

            _target = new DocumentBuilder(_memberFinder.Object, document, testClass);

            var newSetupMethod =
                SyntaxNodeProvider.GetSyntaxNodeFromFile<MethodDeclarationSyntax>(
                    "files.SampleMethod.txt", "SampleMethod");

            _memberFinder.Setup(_ =>
                    _.FindSimilarNode(It.Is<SyntaxNode>(s => s == newSetupMethod),
                        It.Is<SyntaxNode>(s => s == testClass)))
                .Returns(default(SyntaxNode));

            var actual = await _target.WithSetupMethod(newSetupMethod)
                                 .BuildAsync(new CancellationToken());

            var actualRoot = await actual.GetSyntaxRootAsync();
            var actualSetupMethod = actualRoot.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
            Assert.Equal(newSetupMethod.GetText().ToString(), actualSetupMethod.GetText().ToString());
        }

        [Fact]
        public async Task ReplacesSetupMethodInClass()
        {
            var filePath = "files.TestClassWithSampleMethod.txt";
            var document = DocumentProvider.CreateDocumentFromFile(filePath);
            var root = document.GetSyntaxRootAsync().Result;
            var testClass = root.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().First();

            _target = new DocumentBuilder(_memberFinder.Object, document, testClass);

            var newSetupMethod =
                SyntaxNodeProvider.GetSyntaxNodeFromFile<MethodDeclarationSyntax>(
                    "files.SampleMethod.txt", "SampleMethod");

            var existingSetupMethod = root.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
            _memberFinder.Setup(_ =>
                    _.FindSimilarNode(It.Is<SyntaxNode>(s => s == newSetupMethod),
                        It.Is<SyntaxNode>(s => s == testClass)))
                .Returns(existingSetupMethod);

            var actual = await _target.WithSetupMethod(newSetupMethod)
                .BuildAsync(new CancellationToken());

            var actualRoot = await actual.GetSyntaxRootAsync();
            var actualMethods = actualRoot.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();

            Assert.Single(actualMethods);
            Assert.Equal(newSetupMethod.GetText().ToString(), actualMethods.First().GetText().ToString());
        }

        [Fact]
        public async Task DoesNotReplaceSetupMethod_When_WithSetupMethodIsNotCalled()
        {
            var filePath = "files.EmptyTestClass.txt";
            var document = DocumentProvider.CreateDocumentFromFile(filePath);
            var root = document.GetSyntaxRootAsync().Result;
            var testClass = root.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().First();

            _target = new DocumentBuilder(_memberFinder.Object, document, testClass);

            var actual = await _target
                                .BuildAsync(new CancellationToken());

            var actualRoot = await actual.GetSyntaxRootAsync();
            
            Assert.Equal(root.GetText().ToString(), actualRoot.GetText().ToString());
        }
    }
}