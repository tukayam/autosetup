using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestSetupGenerator.CodeAnalysis.CodeAnalyzers;
using TestSetupGenerator.CodeAnalysis.UnitTests.Helpers.RoslynStubProviders;
using Xunit;
using Xunit.Abstractions;

namespace TestSetupGenerator.CodeAnalysis.UnitTests.DocumentBuilderTests
{
    public class DocumentBuilderTests_WithSetupMethodAndFields
    {
        private const string FieldDeclarations = "DocumentBuilderTests.files.FieldDeclarations.txt";

        private const string EmptyTestClass = "DocumentBuilderTests.files.EmptyTestClass.txt";
        private const string TestClassWithSomeFields = "DocumentBuilderTests.files.TestClassWithSomeFields.txt";
        private const string Constructor = "DocumentBuilderTests.files.Constructor.txt";
        private const string TestClassWithEmptyConstructor = "DocumentBuilderTests.files.TestClassWithEmptyConstructor.txt";
        private const string TestClassWithConstructor = "DocumentBuilderTests.files.TestClassWithConstructor.txt";
        private const string TestClassWithConstructorAndSomeFields = "DocumentBuilderTests.files.TestClassWithConstructorAndSomeFields.txt";

        private const string TestClassWithConstructorAndFields = "DocumentBuilderTests.files.TestClassWithAllFields.txt";

        private readonly ITestOutputHelper _testOutput;
        private readonly IMemberFinder _memberFinder;
        private readonly IFieldFinder _fieldFinder;
        private DocumentBuilder _target;

        public DocumentBuilderTests_WithSetupMethodAndFields(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
            _memberFinder = new MemberFinder();
            _fieldFinder = new FieldFinder();
        }

        [Theory]
        [InlineData(EmptyTestClass)]
        [InlineData(TestClassWithSomeFields)]
        [InlineData(TestClassWithEmptyConstructor)]
        [InlineData(TestClassWithConstructor)]
        [InlineData(TestClassWithConstructorAndSomeFields)]
        public async Task SetsConstructorAndFields(string testClassFilePath)
        {
            // Arrange
            var document = DocumentProvider.CreateDocumentFromFile(testClassFilePath);
            var root = document.GetSyntaxRootAsync().Result;
            var testClass = root.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().First();

            _target = new DocumentBuilder(_memberFinder, _fieldFinder, document, testClass);

            var newFieldDeclarations =
                SyntaxNodeProvider.GetAllSyntaxNodesFromFile<FieldDeclarationSyntax>(FieldDeclarations).ToList();

            var expectedFieldDeclarations =
                SyntaxNodeProvider.GetAllSyntaxNodesFromFile<FieldDeclarationSyntax>(TestClassWithConstructorAndFields).ToList();
            var expectedFields = GetCollectionTextAsString(expectedFieldDeclarations);

            var newSetupMethod =
                SyntaxNodeProvider.GetSyntaxNodeFromFile<ConstructorDeclarationSyntax>(Constructor, "TestClass");

            // Act
            var actual = await _target.WithSetupMethod(newSetupMethod)
                                        .WithFields(newFieldDeclarations)
                                        .BuildAsync(new CancellationToken());

            // Assert
            var actualRoot = await actual.GetSyntaxRootAsync();
            _testOutput.WriteLine(actualRoot.GetText().ToString());

            var actualFields = GetCollectionTextAsString(actualRoot.DescendantNodes().OfType<FieldDeclarationSyntax>());
            Assert.Equal(expectedFields, actualFields);

            var actualMethods = actualRoot.DescendantNodes().OfType<ConstructorDeclarationSyntax>().ToList();
            Assert.Single(actualMethods);
            Assert.Equal(newSetupMethod.GetText().ToString(), actualMethods.First().GetText().ToString());
        }

        private string GetCollectionTextAsString(IEnumerable<SyntaxNode> fields)
        {
            return string.Join(string.Empty, fields.Select(_ => _.GetText()));
        }
    }
}