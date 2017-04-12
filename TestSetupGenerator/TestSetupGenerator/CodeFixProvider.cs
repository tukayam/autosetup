using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using TestSetupGenerator.SyntaxFinders;

namespace TestSetupGenerator
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TestSetupGeneratorCodeFixProvider)), Shared]
    public class TestSetupGeneratorCodeFixProvider : CodeFixProvider
    {
        private const string title = "(Re-)Generate SetUp";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(TestSetupGeneratorAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();

            if (declaration.Identifier.Text.EndsWith("Tests"))
            {
                // Register a code action that will invoke the fix.
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: title,
                        createChangedDocument: c => CreateSetupMethod(context.Document, declaration, c),
                        equivalenceKey: title),
                    diagnostic);
            }
        }
        private async Task<Document> CreateSetupMethod(Document document, ClassDeclarationSyntax classDecl, CancellationToken cancellationToken)
        {
            var generator = SyntaxGenerator.GetGenerator(document);

            var classUnderTestName = classDecl.Identifier.Text.Replace("Tests", string.Empty);
            var classUnderTestDeclarationSyntax = await new ClassUnderTestFinder().GetAsync(document.Project.Solution, classUnderTestName);
            var setupMethodDeclaration = SetupMethod(classUnderTestName, classUnderTestDeclarationSyntax, generator);

            //var testClassDocumentSyntaxRoot = await document.GetSyntaxRootAsync();
            //List<UsingDirectiveSyntax> usingDirectives = UsingDirectives(testClassDocumentSyntaxRoot, generator);
            IEnumerable<SyntaxNode> fieldDeclarations = FieldDeclarations(classUnderTestDeclarationSyntax, generator);
            

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            //var existingSetupMethod = root.DescendantNodes().OfType<MethodDeclarationSyntax>()
            //                                                .FirstOrDefault(_ => _.Identifier.Text == "Setup");

            SyntaxNode newRoot;
            //if (existingSetupMethod != null)
            //{
            //    newRoot = root.ReplaceNode(existingSetupMethod, setupMethodDeclaration);
            //}
            //else
            //{
                var newClassDecl = classDecl;

                foreach (var fieldDeclaration in fieldDeclarations)
                {
                    newClassDecl = newClassDecl.AddMembers(fieldDeclaration as MemberDeclarationSyntax);
                }
                newClassDecl = newClassDecl.AddMembers(setupMethodDeclaration as MemberDeclarationSyntax);
                newRoot = root.ReplaceNode(classDecl, newClassDecl);
            //}

            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        private static List<UsingDirectiveSyntax> UsingDirectives(SyntaxNode docSyntaxRoot, SyntaxGenerator generator)
        {
            var usingDirectives = docSyntaxRoot.ChildNodes().OfType<UsingDirectiveSyntax>().ToList();
            usingDirectives.Add(generator.NamespaceImportDeclaration("Rhino.Mocks") as UsingDirectiveSyntax);
            return usingDirectives;
        }

        private IEnumerable<SyntaxNode> FieldDeclarations(ClassDeclarationSyntax classDec, SyntaxGenerator generator)
        {
            var fieldDeclarations = new List<SyntaxNode>();

            var constructorWithParameters =
                classDec.DescendantNodes()
                    .OfType<ConstructorDeclarationSyntax>()
                    .FirstOrDefault(
                        x => x.ParameterList.Parameters.Any());
            if (constructorWithParameters != null)
            {
                var constructorParam = constructorWithParameters.ParameterList.Parameters;
                foreach (var parameter in constructorParam)
                {
                    var parameterType = parameter.Type;
                    var fieldName = GetParameterFieldName(parameter);
                    var fieldDec = generator.FieldDeclaration(fieldName
                                                            , parameterType
                                                            , Accessibility.Private);
                    fieldDeclarations.Add(fieldDec);

                }
            }

            var targetFieldDec = generator.FieldDeclaration("_target"
                                                           , generator.IdentifierName(classDec.Identifier.Text)
                                                           , Accessibility.Private);
            fieldDeclarations.Add(targetFieldDec);
            return fieldDeclarations;
        }

        private string GetParameterFieldName(ParameterSyntax parameter)
        {
            var parameterType = parameter.Type;
            var parameterTypeName = parameterType.ToString();
            var isInterface = parameterTypeName.Substring(0, 1) == "I" &&
                              parameterTypeName.Substring(1, 2).ToLower() != parameterTypeName.Substring(1, 2);
            parameterTypeName = isInterface ? parameterTypeName.Replace("I", string.Empty) : parameterTypeName;
            return string.Format("_{0}", parameterTypeName.Substring(0,1).ToLowerInvariant()+parameterTypeName.Substring(1));
        }

        private SyntaxNode SetupMethod(string className, ClassDeclarationSyntax classDec, SyntaxGenerator generator)
        {
            var fieldDeclarations = new List<SyntaxNode>();
            var expressionStatements = new List<SyntaxNode>();
            var constructorWithParameters =
                classDec.DescendantNodes()
                    .OfType<ConstructorDeclarationSyntax>()
                    .FirstOrDefault(
                        x => x.ParameterList.Parameters.Any());
            if (constructorWithParameters != null)
            {
                var constructorParam = constructorWithParameters.ParameterList.Parameters;
                foreach (var parameter in constructorParam)
                {
                    var parameterType = parameter.Type;
                    var fieldName = GetParameterFieldName(parameter);
                    var fieldDec = generator.FieldDeclaration(fieldName
                                                            , parameterType
                                                            , Accessibility.Private);
                    fieldDeclarations.Add(fieldDec);

                    var fieldIdentifier = generator.IdentifierName(fieldName);
                    var mocksRepositoryIdentifier = generator.IdentifierName("MockRepository");
                    var parameterTypeIdentifier = generator.IdentifierName(parameterType.ToString());

                    var memberAccessExpression = generator.MemberAccessExpression(mocksRepositoryIdentifier, generator.GenericName("GenerateStub", parameterTypeIdentifier));
                    var invocationExpression = generator.InvocationExpression(memberAccessExpression);

                    var expressionStatementSettingField = generator.AssignmentStatement(fieldIdentifier, invocationExpression);
                    expressionStatements.Add(expressionStatementSettingField);
                }
            }

            var constructorParameters = fieldDeclarations.SelectMany(x => x.DescendantNodes().OfType<VariableDeclaratorSyntax>().Select(y => y.Identifier.Text));

            var setupBody = new List<SyntaxNode>();
            setupBody.AddRange(expressionStatements);

            var targetObjectCreationExpression = generator.ObjectCreationExpression(generator.IdentifierName(className), constructorParameters.Select(x => generator.IdentifierName(x)));

            var expressionStatementTargetInstantiation = generator.AssignmentStatement(generator.IdentifierName("_target"), targetObjectCreationExpression);
            setupBody.Add(expressionStatementTargetInstantiation);

            // Generate the Clone method declaration
            var setupMethodDeclaration = generator.MethodDeclaration("Setup", null,
              null, null,
              Accessibility.Public,
              DeclarationModifiers.None,
              setupBody);
            var setupAttribute = generator.Attribute("SetUp");

            var setupMethodWithSetupAttribute = generator.InsertAttributes(setupMethodDeclaration, 0, setupAttribute);
            return setupMethodWithSetupAttribute;
        }
    }
}