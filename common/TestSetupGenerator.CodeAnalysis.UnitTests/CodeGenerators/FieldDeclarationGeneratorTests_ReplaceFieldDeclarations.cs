using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestSetupGenerator.CodeAnalysis.CodeGenerators;
using TestSetupGenerator.CodeAnalysis.UnitTests.Helpers;
using TestSetupGenerator.CodeAnalysis.UnitTests.Helpers.RoslynStubProviders;
using Xunit;

namespace TestSetupGenerator.CodeAnalysis.UnitTests.CodeGenerators
{
    public class FieldDeclarationGeneratorTests_ReplaceFieldDeclarations
    {
        private FieldDeclarationGenerator _target;
        public FieldDeclarationGeneratorTests_ReplaceFieldDeclarations()
        {
            _target = new FieldDeclarationGenerator();
        }
   
        [Fact]
        public void AddsNewField_ToTestClass_When_ClassUnderTest_Has_NewConstructorParameters()
        {
            var filePath = "files.ClassUnderTest_And_TestClass.txt";
            var classUnderTestName = "ClassUnderTest";
            var testClassName = "TestClass";
            var classUnderTestDeclarationSyntax = ClassDeclarationProvider.GetClassDeclaration(filePath, classUnderTestName);
            var testClassDeclarationSyntax = ClassDeclarationProvider.GetClassDeclaration(filePath, testClassName);
            var membersInTestClass = testClassDeclarationSyntax.Members;
            var syntaxGenerator = new SyntaxGeneratorProvider().GetSyntaxGenerator();

            var actual = _target.ReplaceFieldDeclarations(classUnderTestDeclarationSyntax, membersInTestClass, syntaxGenerator).ToList();
            var actualFieldDeclarations = actual.OfType<FieldDeclarationSyntax>().ToList();
            Assert.Equal(3, actualFieldDeclarations.Count);

            var fieldNames = actualFieldDeclarations.SelectMany(_ => _.DescendantNodes().OfType<VariableDeclaratorSyntax>().Select(v=>v.Identifier.Text)).ToList();

            Assert.Contains("_someType", fieldNames);
            Assert.Contains("_someOtherType", fieldNames);
            Assert.Contains("_newType", fieldNames);
        }
    }
}
