using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestSetupGenerator.CodeAnalysis.CodeGenerators;
using TestSetupGenerator.CodeAnalysis.UnitTests.Helpers;
using Xunit;

namespace TestSetupGenerator.CodeAnalysis.UnitTests.CodeGenerators
{
    public class FieldDeclarationGeneratorTests
    {
        private FieldDeclarationGenerator _target;
        public FieldDeclarationGeneratorTests()
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
    }
}
