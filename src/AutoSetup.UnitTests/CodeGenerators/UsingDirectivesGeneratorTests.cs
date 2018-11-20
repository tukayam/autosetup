using System.Collections.Generic;
using System.Linq;
using AutoSetup.CodeGenerators;
using AutoSetup.UnitTests.Helpers.RoslynStubProviders;
using Xunit;

namespace AutoSetup.UnitTests.CodeGenerators
{
    public class UsingDirectivesGeneratorTests
    {
        private UsingDirectivesGenerator _target;

        public UsingDirectivesGeneratorTests()
        {
            _target = new UsingDirectivesGenerator();
        }

        [Fact]
        public void Adds_AllNecessaryUsings()
        {
            var syntaxGenerator = new SyntaxGeneratorProvider().GetSyntaxGenerator();

            var rhinoMocksUsing = "rhino.mocks";

            var usingDirectives = _target.UsingDirectives(new List<string> {rhinoMocksUsing}, syntaxGenerator);
            Assert.Equal("rhino.mocks", usingDirectives.First().Name.ToString());
        }
    }
}
