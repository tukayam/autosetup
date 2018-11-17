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
        private const string SampleFieldDeclarations = "DocumentBuilderTests.files.SampleFieldDeclarations.txt";

        private const string EmptyTestClass = "DocumentBuilderTests.files.EmptyTestClass.txt";
        private const string TestClassWithSampleFields = "DocumentBuilderTests.files.TestClassWithSampleFields.txt";
        private const string TestClassWithSampleFieldsRegenerated = "DocumentBuilderTests.files.TestClassWithSampleFieldsAfterRegenerate.txt";

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
        [InlineData(EmptyTestClass, TestClassWithSampleFields)]
        [InlineData(TestClassWithSampleFields, TestClassWithSampleFieldsRegenerated)]
        public async Task SetsFields(string testClassFilePath,string expectedFields)
        {
            // Arrange
            var document = DocumentProvider.CreateDocumentFromFile(testClassFilePath);
            var root = document.GetSyntaxRootAsync().Result;
            var testClass = root.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().First();

            _target = new DocumentBuilder(_memberFinder.Object, _fieldFinder, document, testClass);

            var newFieldDeclarations =
                SyntaxNodeProvider.GetAllSyntaxNodesFromFile<FieldDeclarationSyntax>(SampleFieldDeclarations).ToList();

            var expectedFieldDeclarations =
                SyntaxNodeProvider.GetAllSyntaxNodesFromFile<FieldDeclarationSyntax>(expectedFields).ToList();
            var expected = GetCollectionTextAsString(expectedFieldDeclarations);

            // Act
            var actual = await _target.WithFields(newFieldDeclarations)
                                    .BuildAsync(new CancellationToken());

            // Assert
            var actualRoot = await actual.GetSyntaxRootAsync();
            var actualFields = GetCollectionTextAsString(actualRoot.DescendantNodes().OfType<FieldDeclarationSyntax>());
            
            Assert.Equal(expected, actualFields);
        }

        private string GetCollectionTextAsString(IEnumerable<SyntaxNode> fields)
        {
            return string.Join(string.Empty, fields.Select(_ => _.GetText()));
        }
    }
}