using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestSetupGenerator.CodeAnalysis.CodeGenerators;
using TestSetupGenerator.CodeAnalysis.UnitTests.Helpers.RoslynStubProviders;
using Xunit;

namespace TestSetupGenerator.CodeAnalysis.UnitTests.CodeGenerators.MemberReplacerTests
{
    public class MemberReplacerTests
    {
        private readonly MemberReplacer _target;
        public MemberReplacerTests()
        {
            _target = new MemberReplacer();
        }

        [Fact]
        public void AddsNewFields_ToTestClass_When_ClassUnderTest_Has_NewConstructorParameters()
        {
            var filePath = "files.ClassUnderTest_And_TestClass.txt";
            var classUnderTestName = "ClassUnderTest";
            var testClassName = "TestClass";
            var classUnderTestDeclarationSyntax = SyntaxNodeProvider.GetSyntaxNodeFromFile<ClassDeclarationSyntax>(filePath, classUnderTestName);
            var testClassDeclarationSyntax = SyntaxNodeProvider.GetSyntaxNodeFromFile<ClassDeclarationSyntax>(filePath, testClassName);
            var membersInTestClass = testClassDeclarationSyntax.Members;

            var newFields = new List<SyntaxNode>();

            var actual = _target.ReplaceFieldDeclarations(classUnderTestDeclarationSyntax, membersInTestClass, newFields).ToList();
            var actualFieldDeclarations = actual.OfType<FieldDeclarationSyntax>().ToList();
            Assert.Equal(3, actualFieldDeclarations.Count);

            var fieldNames = actualFieldDeclarations.SelectMany(_ => _.DescendantNodes().OfType<VariableDeclaratorSyntax>().Select(v => v.Identifier.Text)).ToList();

            Assert.Contains("_someType", fieldNames);
            Assert.Contains("_someOtherType", fieldNames);
            Assert.Contains("_newType", fieldNames);
        }
    }
}
