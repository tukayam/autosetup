using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using TestSetupGenerator.CodeAnalysis.CodeAnalyzers;
using TestSetupGenerator.CodeAnalysis.CodeGenerators;

namespace TestSetupGenerator.XUnitMoq
{
    class XUnitSetupGenerator
    {
        private readonly ConstructorParametersExtractor _constructorParametersExtractor;
        private readonly ExpressionStatementGenerator _expressionStatementGenerator;
        private readonly FieldNameGenerator _fieldNameGenerator;
        private readonly FieldDeclarationGenerator _fieldDeclarationGenerator;

        public XUnitSetupGenerator()
        {
            _constructorParametersExtractor = new ConstructorParametersExtractor();
            _expressionStatementGenerator = new ExpressionStatementGenerator();
            _fieldNameGenerator = new FieldNameGenerator();
            _fieldDeclarationGenerator = new FieldDeclarationGenerator();
        }

        public MemberDeclarationSyntax Constructor(string className, ClassDeclarationSyntax classDec, SyntaxGenerator generator)
        {
            var constructorParameters = _constructorParametersExtractor.GetParametersOfConstructor(classDec).ToList();
            var expressionStatements = new List<SyntaxNode>();

            foreach (var parameter in constructorParameters)
            {
                var fieldName = _fieldNameGenerator.GetFromParameter(parameter);
                var expressionStatementFieldInstantiation =
                    _expressionStatementGenerator.MoqStubAssignmentExpression(parameter.Type.ToString(), fieldName,
                        generator);

                expressionStatements.Add(expressionStatementFieldInstantiation);
            }

            var fieldDeclarations = _fieldDeclarationGenerator.GetFieldDeclarations(constructorParameters, generator);
            var targetObjectCreationParameters = fieldDeclarations.SelectMany(x => x.DescendantNodes().OfType<VariableDeclaratorSyntax>().Select(y => y.Identifier.Text));

            var setupBody = new List<SyntaxNode>();
            setupBody.AddRange(expressionStatements);

            var targetObjectCreationExpression = generator.ObjectCreationExpression(generator.IdentifierName(className), targetObjectCreationParameters.Select(x => generator.MemberAccessExpression(generator.IdentifierName(x), "Object")));

            var expressionStatementTargetInstantiation = generator.AssignmentStatement(generator.IdentifierName("_target"), targetObjectCreationExpression);
            setupBody.Add(expressionStatementTargetInstantiation);

            // Generate the Clone method declaration
            var constructorDeclaration = generator.ConstructorDeclaration(null,
              null,
              setupBody);

            return constructorDeclaration as MemberDeclarationSyntax;
        }
    }
}
