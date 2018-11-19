using System.Collections.Generic;
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

namespace TestSetupGenerator.CodeAnalysis.UnitTests.DocumentBuilderTests
{
    public class DocumentBuilderTests_WithFields
    {
        private const string FieldDeclarations = "DocumentBuilderTests.files.FieldDeclarations.txt";

        private const string EmptyTestClass = "DocumentBuilderTests.files.EmptyTestClass.txt";
        private const string TestClassWithAllFields = "DocumentBuilderTests.files.TestClassWithAllFields.txt";
        private const string TestClassWithSomeFields = "DocumentBuilderTests.files.TestClassWithSomeFields.txt";
        private const string TestClassWithConstructorAndSomeFields = "DocumentBuilderTests.files.TestClassWithConstructorAndSomeFields.txt";

        private readonly ITestOutputHelper _testOutput;
        private readonly Mock<IMemberFinder> _memberFinder;
        private readonly IFieldFinder _fieldFinder;
        private DocumentBuilder _target;

        public DocumentBuilderTests_WithFields(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
            _memberFinder = new Mock<IMemberFinder>();
            _fieldFinder = new FieldFinder();
        }

        [Theory]
        [InlineData(EmptyTestClass)]
        [InlineData(TestClassWithSomeFields)]
        [InlineData(TestClassWithConstructorAndSomeFields)]
        public async Task SetsFields(string testClassFilePath)
        {
            // Arrange
            var document = DocumentProvider.CreateDocumentFromFile(testClassFilePath);
            var root = document.GetSyntaxRootAsync().Result;
            var testClass = root.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().First();

            _target = new DocumentBuilder(_memberFinder.Object, _fieldFinder, document, testClass);

            var newFieldDeclarations =
                SyntaxNodeProvider.GetAllSyntaxNodesFromFile<FieldDeclarationSyntax>(FieldDeclarations).ToList();

            var expectedFieldDeclarations =
                SyntaxNodeProvider.GetAllSyntaxNodesFromFile<FieldDeclarationSyntax>(TestClassWithAllFields).ToList();
            var expected = GetCollectionTextAsString(expectedFieldDeclarations);

            // Act
            var actual = await _target.WithFields(newFieldDeclarations)
                                    .BuildAsync(new CancellationToken());

            // Assert
            var actualRoot = await actual.GetSyntaxRootAsync();
            var actualFields = GetCollectionTextAsString(actualRoot.DescendantNodes().OfType<FieldDeclarationSyntax>());
            _testOutput.WriteLine(actualFields);

            Assert.Equal(expected, actualFields);
        }

        private string GetCollectionTextAsString(IEnumerable<SyntaxNode> fields)
        {
            return string.Join(string.Empty, fields.Select(_ => _.GetText()));
        }
    }
}