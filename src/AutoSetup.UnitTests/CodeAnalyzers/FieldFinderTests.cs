﻿using AutoSetup.CodeAnalyzers;
using AutoSetup.UnitTests.Helpers.RoslynStubProviders;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace AutoSetup.UnitTests.CodeAnalyzers
{
    public class FieldFinderTests
    {
        private readonly FieldFinder _target;

        public FieldFinderTests()
        {
            _target = new FieldFinder();
        }

        [Theory]
        [InlineData("CodeAnalyzers.files.Class_WithSampleField.txt", "CodeAnalyzers.files.Class_WithSampleMethod.txt", "_someType", false)]
        [InlineData("CodeAnalyzers.files.Class_WithSampleField.txt", "CodeAnalyzers.files.Class_WithSampleField.txt", "_someType", true)]
        public void Returns_SimilarFieldSyntaxNode_IfExists(string filePath, string filePathCompared, string fieldName, bool expected)
        {
            var fieldSyntax = FieldDeclarationProvider.GetFieldDeclarationFromFile(filePath, fieldName);
            var classSyntax = SyntaxNodeProvider.GetSyntaxNodeFromFile<ClassDeclarationSyntax>(filePathCompared, "TestClass");

            var actual = _target.FindSimilarNode(classSyntax.Members, fieldSyntax);

            Assert.Equal(expected, actual != null);
        }
    }
}