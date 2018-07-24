using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using TestSetupGenerator.CodeAnalysis.CodeAnalyzers;

namespace TestSetupGenerator.CodeAnalysis.CodeGenerators
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
        private readonly IFieldDeclarationsBuilder _fieldDeclarationGenerator;

        public SetupMethodBodyBuilder(IConstructorParametersExtractor constructorParametersExtractor,
                                        IExpressionStatementGenerator expeExpressionStatementGenerator,
                                        IFieldNameGenerator fieldNameGenerator,
                                        IFieldDeclarationsBuilder fieldDeclarationsGenerator)
        {
            _constructorParametersExtractor = constructorParametersExtractor;
            _expressionStatementGenerator = expeExpressionStatementGenerator;
            _fieldNameGenerator = fieldNameGenerator;
            _fieldDeclarationGenerator = fieldDeclarationsGenerator;
        }

        public IEnumerable<SyntaxNode> GetSetupMethodBodyMembers(ClassDeclarationSyntax classUnderTestDec, SyntaxGenerator generator)
        {
            var constructorParameters = _constructorParametersExtractor.GetParametersOfConstructor(classUnderTestDec).ToList();
            var expressionStatements = new List<SyntaxNode>();

            foreach (var parameter in constructorParameters)
            {
                var fieldName = _fieldNameGenerator.GetFromParameter(parameter);
                var expressionStatementFieldInstantiation =
                    _expressionStatementGenerator.MoqStubAssignmentExpression(parameter.Type.ToString(), fieldName,
                        generator);

                expressionStatements.Add(expressionStatementFieldInstantiation);
            }

            var fieldDeclarations = _fieldDeclarationGenerator.GetFieldDeclarations(classUnderTestDec, generator);
            var classUnderTestName = classUnderTestDec.Identifier.Text;
            var expressionStatementTargetInstantiation = _expressionStatementGenerator.TargetObjectAssignmentExpression(fieldDeclarations, classUnderTestName, generator);

            var setupBody = new List<SyntaxNode>();
            setupBody.AddRange(expressionStatements);
            setupBody.Add(expressionStatementTargetInstantiation);

            return setupBody;
        }
    }
}
