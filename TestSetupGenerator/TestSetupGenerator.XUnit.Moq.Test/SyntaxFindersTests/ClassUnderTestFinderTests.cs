using Microsoft.CodeAnalysis;
using NUnit.Framework;
using TestSetupGenerator.SyntaxFinders;
using TestSetupGenerator.Test.Helpers;

namespace TestSetupGenerator.Test.SyntaxFindersTests
{
    [TestFixture]
    class ClassUnderTestFinderTests
    {
        private ClassUnderTestFinder _target;
        private Solution _solution;

        [SetUp]
        public void Setup()
        {
            _target = new ClassUnderTestFinder();

            var solutionGenerator = new RoslynSolutionGenerator();
            _solution = solutionGenerator.GetTestClassGeneratorSamplesSolutionAsync();
        }

        [Test]
        public void FindsClassDeclarationSyntax_When_ThereIsOneClassWithThatNameInSolution()
        {
            var actual = _target.GetAsync("",_solution, "ClassWithOneDependency").Result;

            Assert.AreEqual("ClassWithOneDependency", actual.Identifier.Text);
        }
    }
}
