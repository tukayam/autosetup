using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using TestSetupGenerator.CodeAnalysis.CodeAnalyzers;
using TestSetupGenerator.CodeAnalysis.CodeGenerators;

namespace TestSetupGenerator.CodeAnalysis
{
    public interface ISetupMethodBodyBuilder
    {
        IEnumerable<SyntaxNode> GetSetupMethodBodyMembers(ClassDeclarationSyntax classUnderTestDec, SyntaxGenerator generator);
    }

    public class SetupMethodBodyBuilder : ISetupMethodBodyBuilder
    {
        private readonly IConstructorParametersExtractor _constructorParametersExtractor;
        private readonly IExpressionStatementGenerator _expressionStatementGenerator;
        private readonly IFieldNameGenerator _fieldNameGenerator;

        public SetupMethodBodyBuilder(IConstructorParametersExtractor constructorParametersExtractor,
                                        IExpressionStatementGenerator expeExpressionStatementGenerator,
                                        IFieldNameGenerator fieldNameGenerator)
        {
            _constructorParametersExtractor = constructorParametersExtractor;
            _expressionStatementGenerator = expeExpressionStatementGenerator;
            _fieldNameGenerator = fieldNameGenerator;
        }

        public IEnumerable<SyntaxNode> GetSetupMethodBodyMembers(ClassDeclarationSyntax classUnderTestDec, SyntaxGenerator generator)
        {
            var constructorParameters = _constructorParametersExtractor.GetParametersOfConstructor(classUnderTestDec).ToList();

            var expressionStatements = constructorParameters.Select(_ => _expressionStatementGenerator.MoqStubAssignmentExpression(
                                                                            _.Type.ToString(), _fieldNameGenerator.GetFromParameter(_), generator));

            var fieldDeclarations = constructorParameters.Select(_ => _fieldNameGenerator.GetFromParameter(_));
            var classUnderTestName = classUnderTestDec.Identifier.Text;
            var expressionStatementTargetInstantiation = _expressionStatementGenerator.MoqTargetObjectAssignmentExpression(fieldDeclarations, classUnderTestName, generator);

            var setupBody = new List<SyntaxNode>();
            setupBody.AddRange(expressionStatements);
            setupBody.Add(expressionStatementTargetInstantiation);

            return setupBody;
        }
    }
}
