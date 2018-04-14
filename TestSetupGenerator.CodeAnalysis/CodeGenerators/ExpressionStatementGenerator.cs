using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;

namespace TestSetupGenerator.CodeAnalysis.CodeGenerators
{
    public class ExpressionStatementGenerator
    {
        public SyntaxNode MockRepositoryGenerateStubAssignmentExpression(string parameterType, string fieldName, SyntaxGenerator generator)
        {
            var fieldIdentifier = generator.IdentifierName(fieldName);
            var mocksRepositoryIdentifier = generator.IdentifierName("MockRepository");
            var parameterTypeIdentifier = generator.IdentifierName(parameterType);

            var memberAccessExpression = generator.MemberAccessExpression(mocksRepositoryIdentifier, generator.GenericName("GenerateStub", parameterTypeIdentifier));
            var invocationExpression = generator.InvocationExpression(memberAccessExpression);

            return generator.AssignmentStatement(fieldIdentifier, invocationExpression);
        }
    }
}
