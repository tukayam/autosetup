using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoSetup.XUnitMoq.CodeAnalyzers;
using AutoSetup.XUnitMoq.UnitTests.Helpers.RoslynStubProviders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using Xunit;

namespace AutoSetup.XUnitMoq.UnitTests.DocumentBuilderTests
{
    public class DocumentBuilderTests_WithSetupMethod
    {
        private const string EmptyTestClass = "DocumentBuilderTests.files.EmptyTestClass.txt";
        private const string Constructor = "DocumentBuilderTests.files.Constructor.txt";
        private const string TestClassWithEmptyConstructor = "DocumentBuilderTests.files.TestClassWithEmptyConstructor.txt";

        private readonly Mock<IMemberFinder> _memberFinder;
        private readonly Mock<IFieldFinder> _fieldFinder;
        private DocumentBuilder _target;

        public DocumentBuilderTests_WithSetupMethod()
        {
            _memberFinder = new Mock<IMemberFinder>();
            _fieldFinder = new Mock<IFieldFinder>();
        }

        [Fact]
        public async Task AddsNewSetupMethodToClass()
        {
            var document = DocumentProvider.CreateDocumentFromFile(EmptyTestClass);
            var root = document.GetSyntaxRootAsync().Result;
            var testClass = root.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().First();

            _target = new DocumentBuilder(_memberFinder.Object, _fieldFinder.Object, document, testClass);

            var newSetupMethod =
                SyntaxNodeProvider.GetSyntaxNodeFromFile<ConstructorDeclarationSyntax>(Constructor, "TestClass");

            _memberFinder.Setup(_ =>
                    _.FindSimilarNode(It.IsAny<SyntaxList<MemberDeclarationSyntax>>(),
                        It.Is<SyntaxNode>(s => s == newSetupMethod)))
                .Returns(default(SyntaxNode));

            var actual = await _target.WithSetupMethod(newSetupMethod)
                                 .BuildAsync(new CancellationToken());

            var actualRoot = await actual.GetSyntaxRootAsync();
            var actualSetupMethod = actualRoot.DescendantNodes().OfType<ConstructorDeclarationSyntax>().First();
            Assert.Equal(newSetupMethod.GetText().ToString(), actualSetupMethod.GetText().ToString());
        }

        [Fact]
        public async Task ReplacesSetupMethodInClass()
        {
            var document = DocumentProvider.CreateDocumentFromFile(TestClassWithEmptyConstructor);
            var root = document.GetSyntaxRootAsync().Result;
            var testClass = root.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().First();

            _target = new DocumentBuilder(_memberFinder.Object, _fieldFinder.Object, document, testClass);

            var newSetupMethod =
                SyntaxNodeProvider.GetSyntaxNodeFromFile<ConstructorDeclarationSyntax>(Constructor, "TestClass");

            var existingSetupMethod = root.DescendantNodes().OfType<ConstructorDeclarationSyntax>().First();
            _memberFinder.Setup(_ =>
                    _.FindSimilarNode(It.IsAny<SyntaxList<MemberDeclarationSyntax>>(),
                        It.Is<SyntaxNode>(s => s == newSetupMethod)))
                .Returns(existingSetupMethod);

            var actual = await _target.WithSetupMethod(newSetupMethod)
                .BuildAsync(new CancellationToken());

            var actualRoot = await actual.GetSyntaxRootAsync();
            var actualMethods = actualRoot.DescendantNodes().OfType<ConstructorDeclarationSyntax>().ToList();

            Assert.Single((IEnumerable) actualMethods);
            Assert.Equal(newSetupMethod.GetText().ToString(), actualMethods.First().GetText().ToString());
        }

        [Fact]
        public async Task DoesNotReplaceSetupMethod_When_WithSetupMethodIsNotCalled()
        {
            var document = DocumentProvider.CreateDocumentFromFile(EmptyTestClass);
            var root = document.GetSyntaxRootAsync().Result;
            var testClass = root.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().First();

            _target = new DocumentBuilder(_memberFinder.Object, _fieldFinder.Object, document, testClass);

            var actual = await _target
                                .BuildAsync(new CancellationToken());

            var actualRoot = await actual.GetSyntaxRootAsync();

            Assert.Equal(root.GetText().ToString(), actualRoot.GetText().ToString());
        }
    }
}