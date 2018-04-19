using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestSetupGenerator.CodeAnalysis.CodeGenerators;
using TestSetupGenerator.CodeAnalysis.UnitTests.Helpers;
using TestSetupGenerator.CodeAnalysis.UnitTests.Helpers.RoslynStubProviders;
using Xunit;

namespace TestSetupGenerator.CodeAnalysis.UnitTests.CodeGenerators
{
    public class FieldDeclarationGeneratorTests_GetFieldDeclarations
    {
        private FieldDeclarationGenerator _target;
        public FieldDeclarationGeneratorTests_GetFieldDeclarations()
        {
            _target = new FieldDeclarationGenerator();
        }

        [Theory]
        [InlineData("files.Class_WithConstructorWithTwoParameters.txt", "ISomeType", "_someType")]
        [InlineData("files.Class_WithConstructorWithTwoParameters.txt", "ISomeOtherType", "_someOtherType")]
        public void Returns_FieldDeclarationsWithCorrectName_ForEach_Parameter(string filePath, string typeName, string expectedVariableName)
        {
            var typeSyntax = TypeSyntaxProvider.GetTypeSyntax(filePath, typeName);
            var syntaxGenerator = new SyntaxGeneratorProvider().GetSyntaxGenerator();

            var actual = _target.GetFieldDeclarations(new List<TypeSyntax> { typeSyntax }, syntaxGenerator).ToList();

            Assert.Single(actual);

            var firstDeclaration = actual.First();
            var firstDeclarationName = firstDeclaration.DescendantNodes().OfType<VariableDeclaratorSyntax>().First()
                .Identifier.Text;
            Assert.Equal(expectedVariableName, firstDeclarationName);
        }

        [Fact]
        public void Returns_FieldDeclarations_ForEach_ConstructorParameterInClassUnderTest()
        {
            var filePath = "files.Class_WithConstructorWithTwoParameters.txt";
            var className = "TestClass";
            var classDeclarationSyntax = ClassDeclarationProvider.GetClassDeclaration(filePath, className);
            var syntaxGenerator = new SyntaxGeneratorProvider().GetSyntaxGenerator();

            var actual = _target.GetFieldDeclarations(classDeclarationSyntax, syntaxGenerator).ToList();

            Assert.Equal(2, actual.Count);

            var firstDeclaration = actual.First();
            var firstDeclarationName = firstDeclaration.DescendantNodes().OfType<VariableDeclaratorSyntax>().First()
                .Identifier.Text;
            Assert.Equal("_someType", firstDeclarationName);

            var secondDeclaration = actual.Skip(1).First();
            var secondDeclarationName = secondDeclaration.DescendantNodes().OfType<VariableDeclaratorSyntax>().First()
                .Identifier.Text;
            Assert.Equal("_someOtherType", secondDeclarationName);
        }
    }
}
