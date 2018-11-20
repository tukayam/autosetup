using AutoSetup.XUnitMoq.CodeAnalyzers;
using AutoSetup.XUnitMoq.UnitTests.Helpers.RoslynStubProviders;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace AutoSetup.XUnitMoq.UnitTests.CodeAnalyzers
{
    public class MemberFinderTests
    {
        private MemberFinder _target;
        public MemberFinderTests()
        {
            _target = new MemberFinder();
        }

        [Theory]
        [InlineData("CodeAnalyzers.files.Class_WithConstructorWithTwoParameters.txt", "CodeAnalyzers.files.Class_WithConstructorWithTwoParameters.txt", "TestClass", true)]
        [InlineData("CodeAnalyzers.files.Class_WithConstructorWithTwoParameters.txt", "CodeAnalyzers.files.Class_WithSampleMethod.txt", "TestClass", false)]
        [InlineData("CodeAnalyzers.files.Class_WithSampleMethod.txt", "CodeAnalyzers.files.Class_WithSampleMethod.txt", "TestClass", false)]
        [InlineData("CodeAnalyzers.files.Class_WithSampleMethod.txt", "CodeAnalyzers.files.Class_WithSampleMethod.txt", "SampleMethod", true)]
        public void Returns_SimilarClassSyntaxNode_IfExists(string filePath, string filePathCompared, string constructorOrMethodName, bool expected)
        {
            var memberSyntax = SyntaxNodeProvider.GetSyntaxNodeFromFile<MemberDeclarationSyntax>(filePath, constructorOrMethodName);
            var classSyntax = SyntaxNodeProvider.GetSyntaxNodeFromFile<ClassDeclarationSyntax>(filePathCompared, "TestClass");

            var actual = _target.FindSimilarNode(classSyntax.Members, memberSyntax);

            Assert.Equal(expected, actual != null);
        }
    }
}