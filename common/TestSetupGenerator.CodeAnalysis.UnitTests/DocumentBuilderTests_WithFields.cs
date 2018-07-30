using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using TestSetupGenerator.CodeAnalysis.CodeAnalyzers;
using TestSetupGenerator.CodeAnalysis.UnitTests.Helpers.RoslynStubProviders;
using Xunit;
using Xunit.Abstractions;

namespace TestSetupGenerator.CodeAnalysis.UnitTests
{
    public class DocumentBuilderTests_WithFields
    {
        private readonly ITestOutputHelper _testOutput;
        private readonly Mock<IMemberFinder> _memberFinder;
        private DocumentBuilder _target;

        public DocumentBuilderTests_WithFields(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
            _memberFinder = new Mock<IMemberFinder>();
        }

        [Fact]
        public async Task AddsNewFieldsToEmptyTestClass()
        {
            var filePath = "files.EmptyTestClass.txt";
            var document = DocumentProvider.CreateDocumentFromFile(filePath);
            var root = document.GetSyntaxRootAsync().Result;
            var testClass = root.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().First();

            _target = new DocumentBuilder(_memberFinder.Object, document, testClass);

            var newFieldDeclarations =
                SyntaxNodeProvider.GetAllSyntaxNodesFromFile<FieldDeclarationSyntax>("files.SampleFieldDeclarations.txt").ToList();

            _memberFinder.Setup(_ =>
                    _.FindSimilarNode(It.IsAny<SyntaxNode>(), It.IsAny<SyntaxNode>()))
                .Returns(default(SyntaxNode));

            var actual = await _target.WithFields(newFieldDeclarations)
                                 .BuildAsync(new CancellationToken());

            var actualRoot = await actual.GetSyntaxRootAsync();
            var actualFields = actualRoot.DescendantNodes().OfType<FieldDeclarationSyntax>().ToList();

            Assert.Equal(2, actualFields.Count);
            Assert.True(actualFields.All(_ => newFieldDeclarations.Any(f => f.GetText().ToString() == _.GetText().ToString())));

            _testOutput.WriteLine(actualRoot.GetText().ToString());
        }

        [Fact]
        public async Task AddsFieldsInSameOrderAsTheParameters()
        {
            var filePath = "files.EmptyTestClass.txt";
            var document = DocumentProvider.CreateDocumentFromFile(filePath);
            var root = document.GetSyntaxRootAsync().Result;
            var testClass = root.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().First();

            _target = new DocumentBuilder(_memberFinder.Object, document, testClass);

            var newFieldDeclarations =
                SyntaxNodeProvider.GetAllSyntaxNodesFromFile<FieldDeclarationSyntax>("files.SampleFieldDeclarations.txt").ToList();

            _memberFinder.Setup(_ =>
                    _.FindSimilarNode(It.IsAny<SyntaxNode>(), It.IsAny<SyntaxNode>()))
                .Returns(default(SyntaxNode));

            var actual = await _target.WithFields(newFieldDeclarations)
                .BuildAsync(new CancellationToken());


            var actualRoot = await actual.GetSyntaxRootAsync();
            _testOutput.WriteLine(actualRoot.GetText().ToString());

            var actualFields = actualRoot.DescendantNodes().OfType<FieldDeclarationSyntax>().ToList();

            var expectedFirstField = newFieldDeclarations.First();
            var expectedSecondField = newFieldDeclarations.Skip(1).First();

            var actualFirstField = actualFields.First();
            var actualSecondField = actualFields.Skip(1).First();

            Assert.Equal(expectedFirstField.GetText().ToString(), actualFirstField.GetText().ToString());
            Assert.Equal(expectedSecondField.GetText().ToString(), actualSecondField.GetText().ToString());
        }

        //[Fact]
        //public async Task ReplacesExistingFieldsWithSameTypeInClass()
        //{
        //    var filePath = "DocumentBuilderTests.files.TestClassWithSampleFields.txt";
        //    var document = DocumentProvider.CreateDocumentFromFile(filePath);
        //    var root = document.GetSyntaxRootAsync().Result;
        //    var testClass = root.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().First();

        //    _target = new DocumentBuilder(_memberFinder.Object, document, testClass);

        //    var newFieldDeclarations =
        //        SyntaxNodeProvider.GetAllSyntaxNodesFromFile<FieldDeclarationSyntax>("DocumentBuilderTests.files.SampleFieldDeclarations.txt").ToList();

        //    var existingFields = root.DescendantNodes().OfType<FieldDeclarationSyntax>().ToList();
        //    var firstExistingField = existingFields.First();
        //    var secondExistingField = existingFields.Skip(1).First();

        //    _memberFinder.Setup(_ =>
        //            _.FindSimilarNode(It.Is<SyntaxNode>(f => f == firstExistingField), It.IsAny<SyntaxNode>()))
        //        .Returns(firstExistingField);
        //    _memberFinder.Setup(_ =>
        //            _.FindSimilarNode(It.Is<SyntaxNode>(f => f == secondExistingField), It.IsAny<SyntaxNode>()))
        //        .Returns(secondExistingField);

        //    _memberFinder.Setup(_ =>
        //            _.FindSimilarNode(It.Is<SyntaxNode>(s => s == newSetupMethod),
        //                It.Is<SyntaxNode>(s => s == testClass)))
        //        .Returns(existingSetupMethod);

        //    var actual = await _target.WithSetupMethod(newSetupMethod)
        //        .BuildAsync(new CancellationToken());

        //    var actualRoot = await actual.GetSyntaxRootAsync();
        //    var actualMethods = actualRoot.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();

        //    Assert.Single(actualMethods);
        //    Assert.Equal(newSetupMethod.GetText().ToString(), actualMethods.First().GetText().ToString());
        //}

        //[Fact]
        //public async Task DoesNotReplaceSetupMethod_When_WithSetupMethodIsNotCalled()
        //{
        //    var filePath = "DocumentBuilderTests.files.EmptyTestClass.txt";
        //    var document = DocumentProvider.CreateDocumentFromFile(filePath);
        //    var root = document.GetSyntaxRootAsync().Result;
        //    var testClass = root.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().First();

        //    _target = new DocumentBuilder(_memberFinder.Object, document, testClass);

        //    var actual = await _target
        //                        .BuildAsync(new CancellationToken());

        //    var actualRoot = await actual.GetSyntaxRootAsync();

        //    Assert.Equal(root.GetText().ToString(), actualRoot.GetText().ToString());
        //}
    }
}