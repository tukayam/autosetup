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
        private readonly ConstructorGenerator _constructorGenerator;

        public XUnitSetupGenerator()
        {
            _constructorParametersExtractor = new ConstructorParametersExtractor();
            _expressionStatementGenerator = new ExpressionStatementGenerator();
            _fieldNameGenerator = new FieldNameGenerator();
            _fieldDeclarationGenerator = new FieldDeclarationGenerator();
            _constructorGenerator = new ConstructorGenerator();
        }

        public MemberDeclarationSyntax Constructor(string testClassName, ClassDeclarationSyntax classUnderTestDec, SyntaxGenerator generator)
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

            var fieldDeclarations = _fieldDeclarationGenerator.GetFieldDeclarations(constructorParameters, generator);
            var classUnderTestName = classUnderTestDec.Identifier.Text;
            var expressionStatementTargetInstantiation = _expressionStatementGenerator.TargetObjectAssignmentExpression(fieldDeclarations, classUnderTestName, generator);

            var setupBody = new List<SyntaxNode>();
            setupBody.AddRange(expressionStatements);
            setupBody.Add(expressionStatementTargetInstantiation);

            return _constructorGenerator.Constructor(testClassName, setupBody, generator) as MemberDeclarationSyntax;
        }
    }
}
