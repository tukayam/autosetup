using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace AutoSetup.CodeGenerators
{
    public interface IMethodGenerator
    {
        MemberDeclarationSyntax GenerateMethodWithAttribute(string methodName, string attributeName, IList<SyntaxNode> methodBodySyntaxNodes, SyntaxGenerator generator);
    }

    public class MethodGenerator : IMethodGenerator
    {
        /// <summary>
        /// Generates a method such as following 
        /// [SetUp]
        /// public void Setup()
        /// {
        /// ... 
        /// ...
        /// ... (provided syntax nodes here)
        /// }
        /// </summary>
        /// <param name="methodName">e.g. Setup</param>
        /// <param name="attributeName">e.g. SetUp</param>
        /// <param name="methodBodySyntaxNodes">Syntax Node instances to add to method body</param>
        /// <param name="generator">SyntaxGenerator instance for containing syntax</param>
        /// <returns></returns>
        public MemberDeclarationSyntax GenerateMethodWithAttribute(string methodName, string attributeName, IList<SyntaxNode> methodBodySyntaxNodes, SyntaxGenerator generator)
        {
            var setupMethodDeclaration = generator.MethodDeclaration(methodName, null,
              null, null,
              Accessibility.Public,
              DeclarationModifiers.None,
                methodBodySyntaxNodes);
            var setupAttribute = generator.Attribute(attributeName);

            var setupMethodWithSetupAttribute = generator.InsertAttributes(setupMethodDeclaration, 0, setupAttribute);
            return setupMethodWithSetupAttribute as MemberDeclarationSyntax;
        }
    }
}
