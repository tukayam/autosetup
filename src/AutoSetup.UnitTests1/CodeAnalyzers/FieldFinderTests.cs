using AutoSetup.CodeAnalyzers;
using AutoSetup.UnitTests.Helpers.RoslynStubProviders;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace AutoSetup.UnitTests.CodeAnalyzers
{
    public class FieldFinderTests
    {
        private const string FieldNameToUse = "_someType";
        private const string FileContainingFieldToUse = "CodeAnalyzers.files.Class_WithSampleField.txt";
        private const string ClassNameToUse = "TestClass";

        private readonly FieldFinder _target;

        public FieldFinderTests()
        {
            _target = new FieldFinder();
        }

        [Theory]
        [InlineData("CodeAnalyzers.files.Class_WithSampleMethod.txt")]
        [InlineData("CodeAnalyzers.files.Class_WithSampleMethodWithLines.txt")]
        public void CannotFindSimilarField_When_NoFieldExistsWithSameType(string searchInFile)
        {
            var fieldSyntax = FieldDeclarationProvider.GetFieldDeclarationFromFile(FileContainingFieldToUse, FieldNameToUse);
            var classSyntax = SyntaxNodeProvider.GetSyntaxNodeFromFile<ClassDeclarationSyntax>(searchInFile, ClassNameToUse);

            var actual = _target.FindSimilarNode(classSyntax.Members, fieldSyntax);

            Assert.Null(actual);
        }

        [Theory]
        [InlineData("CodeAnalyzers.files.Class_WithSampleField.txt")]
        [InlineData("CodeAnalyzers.files.Class_WithPublicSampleField.txt")]
        [InlineData("CodeAnalyzers.files.Class_WithNotReadonlySampleField.txt")]
        [InlineData("CodeAnalyzers.files.Class_WithSampleFieldDifferentName.txt")]
        public void CanFindSimilarField_When_FieldExistsWithSameType(string searchInFile)
        {
            var fieldSyntax = FieldDeclarationProvider.GetFieldDeclarationFromFile(FileContainingFieldToUse, FieldNameToUse);
            var classSyntax = SyntaxNodeProvider.GetSyntaxNodeFromFile<ClassDeclarationSyntax>(searchInFile, ClassNameToUse);

            var actual = _target.FindSimilarNode(classSyntax.Members, fieldSyntax);

            Assert.NotNull(actual);
        }
    }
}