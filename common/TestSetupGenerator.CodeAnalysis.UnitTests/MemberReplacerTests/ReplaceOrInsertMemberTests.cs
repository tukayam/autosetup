using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using TestSetupGenerator.CodeAnalysis.CodeAnalyzers;
using TestSetupGenerator.CodeAnalysis.CodeGenerators;
using TestSetupGenerator.CodeAnalysis.UnitTests.Helpers.RoslynStubProviders;
using Xunit;

namespace TestSetupGenerator.CodeAnalysis.UnitTests.MemberReplacerTests
{
    public class ReplaceOrInsertMemberTests
    {
        private readonly MemberReplacer _target;
        private readonly Mock<IMemberFinder> _memberFinder;
        public ReplaceOrInsertMemberTests()
        {
            _memberFinder = new Mock<IMemberFinder>();
            _target = new MemberReplacer(_memberFinder.Object);
        }

        [Theory]
        [InlineData("MemberReplacerTests.files.EmptyTestClass.txt")]
        public void Inserts_Method_When_NoSimilarMemberExists(string filePath)
        {
            var testClassName = "TestClass";
            var testClassDeclarationSyntax = SyntaxNodeProvider.GetSyntaxNodeFromFile<ClassDeclarationSyntax>(filePath, testClassName);

            var filePathSampleMember = "MemberReplacerTests.files.SampleMethod.txt";
            var sampleMethodName = "SomeMethod";
            var sampleMethod = SyntaxNodeProvider.GetSyntaxNodeFromFile<MethodDeclarationSyntax>(filePathSampleMember, sampleMethodName);

            _memberFinder.Setup(_ => _.FindSimilarNode(It.IsAny<SyntaxNode>(), It.IsAny<SyntaxNode>()))
                .Returns(default(SyntaxNode));
            var actual = _target.ReplaceOrInsert(testClassDeclarationSyntax, sampleMethod);

            var actualMethodDeclarations = actual.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
            Assert.Single(actualMethodDeclarations);
            Assert.Equal(actualMethodDeclarations.First().GetText().ToString(), sampleMethod.GetText().ToString());
        }
        
        [Theory]
        [InlineData("MemberReplacerTests.files.TestClassWithSampleMethod.txt")]
        public void ReplacesSampleMethod_When_ItAlreadyExists(string filePath)
        {
            var testClassName = "TestClass";
            var testClassDeclarationSyntax = SyntaxNodeProvider.GetSyntaxNodeFromFile<ClassDeclarationSyntax>(filePath, testClassName);

            var filePathSampleMember = "MemberReplacerTests.files.SampleMethod.txt";
            var sampleMethodName = "SomeMethod";
            var sampleMethod = SyntaxNodeProvider.GetSyntaxNodeFromFile<MethodDeclarationSyntax>(filePathSampleMember, sampleMethodName);

            var sampleMethodInTestClass= testClassDeclarationSyntax.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
            _memberFinder.Setup(_ => _.FindSimilarNode(It.IsAny<SyntaxNode>(), It.IsAny<SyntaxNode>()))
                .Returns(sampleMethodInTestClass);
            var actual = _target.ReplaceOrInsert(testClassDeclarationSyntax, sampleMethod);

            var actualMethodDeclarations = actual.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
            Assert.Single(actualMethodDeclarations);
            Assert.Equal(actualMethodDeclarations.First().GetText().ToString(), sampleMethod.GetText().ToString());
        }


        [Theory]
        [InlineData("MemberReplacerTests.files.EmptyTestClass.txt")]
        public void Inserts_Constructor(string filePath)
        {
            var testClassName = "TestClass";
            var testClassDeclarationSyntax = SyntaxNodeProvider.GetSyntaxNodeFromFile<ClassDeclarationSyntax>(filePath, testClassName);

            var filePathSampleMember = "MemberReplacerTests.files.Constructor.txt";
            var constructor = SyntaxNodeProvider.GetSyntaxNodeFromFile<ConstructorDeclarationSyntax>(filePathSampleMember, "TestClass");

            _memberFinder.Setup(_ => _.FindSimilarNode(It.IsAny<SyntaxNode>(), It.IsAny<SyntaxNode>()))
                .Returns(default(SyntaxNode));

            var actual = _target.ReplaceOrInsert(testClassDeclarationSyntax, constructor);

            var actualConstructor = actual.DescendantNodes().OfType<ConstructorDeclarationSyntax>().ToList();
            Assert.Single(actualConstructor);
            Assert.Equal(actualConstructor.First().GetText().ToString(), constructor.GetText().ToString());
        }

        [Theory]
        [InlineData("MemberReplacerTests.files.TestClassWithConstructor.txt")]
        public void Replaces_Constructor(string filePath)
        {
            var testClassName = "TestClass";
            var testClassDeclarationSyntax = SyntaxNodeProvider.GetSyntaxNodeFromFile<ClassDeclarationSyntax>(filePath, testClassName);

            var filePathSampleMember = "MemberReplacerTests.files.Constructor.txt";
            var constructor = SyntaxNodeProvider.GetSyntaxNodeFromFile<ConstructorDeclarationSyntax>(filePathSampleMember, "TestClass");

            var constructorInTestClass = testClassDeclarationSyntax.DescendantNodes()
                .OfType<ConstructorDeclarationSyntax>().First();
            _memberFinder.Setup(_ => _.FindSimilarNode(It.IsAny<SyntaxNode>(), It.IsAny<SyntaxNode>()))
                .Returns(constructorInTestClass);

            var actual = _target.ReplaceOrInsert(testClassDeclarationSyntax, constructor);

            var actualConstructor = actual.DescendantNodes().OfType<ConstructorDeclarationSyntax>().ToList();
            Assert.Single(actualConstructor);
            Assert.Equal(actualConstructor.First().GetText().ToString(), constructor.GetText().ToString());
        }
    }
}
