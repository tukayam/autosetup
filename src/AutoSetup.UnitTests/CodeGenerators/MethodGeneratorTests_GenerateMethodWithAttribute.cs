using System.Collections.Generic;
using AutoSetup.CodeGenerators;
using AutoSetup.UnitTests.Helpers.RoslynStubProviders;
using Microsoft.CodeAnalysis;
using Xunit;

namespace AutoSetup.UnitTests.CodeGenerators
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
