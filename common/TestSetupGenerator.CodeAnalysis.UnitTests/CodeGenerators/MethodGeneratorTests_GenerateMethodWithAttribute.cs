using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using TestSetupGenerator.CodeAnalysis.CodeGenerators;
using TestSetupGenerator.CodeAnalysis.UnitTests.Helpers;
using TestSetupGenerator.CodeAnalysis.UnitTests.Helpers.RoslynStubProviders;
using Xunit;

namespace TestSetupGenerator.CodeAnalysis.UnitTests.CodeGenerators
{
    public class MethodGeneratorTests_GenerateMethodWithAttribute
    {
        private MethodGenerator _target;
        public MethodGeneratorTests_GenerateMethodWithAttribute()
        {
            _target = new MethodGenerator();
        }

        [Fact]
        public void Returns_MethodWithAttribute()
        {
            var methodName = "method";
            var attributeName = "someAttribute";
            var methodBody = new List<SyntaxNode>();
            var syntaxGenerator = new SyntaxGeneratorProvider().GetSyntaxGenerator();

            var actual = _target.GenerateMethodWithAttribute(methodName, attributeName, methodBody, syntaxGenerator);
            var asText = actual.GetText().ToString();

            var expected = "[someAttribute]publicvoidmethod(){}";
            Assert.Equal(expected, asText);
        }
    }
}
