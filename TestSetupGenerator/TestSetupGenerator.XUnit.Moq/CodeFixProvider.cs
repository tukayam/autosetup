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
                try
                {
                    // Register a code action that will invoke the fix.
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: title,
                            createChangedDocument: c => CreateSetupMethod(context.Document, declaration, c),
                            equivalenceKey: title),
                        diagnostic);
                }
                catch
                {

                }
            }
        }
        private async Task<Document> CreateSetupMethod(Document document, ClassDeclarationSyntax classDecl, CancellationToken cancellationToken)
        {
            try
            {
                var generator = SyntaxGenerator.GetGenerator(document);

                var testProjectName = document.Project.Name;
                var classUnderTestName = classDecl.Identifier.Text.Replace("Tests", string.Empty);
                var classUnderTestDeclarationSyntax = await new ClassUnderTestFinder().GetAsync(testProjectName,
                    document.Project.Solution, classUnderTestName);

                if (classUnderTestDeclarationSyntax == null)
                {
                    // todo: show message "Class under test could not be found. Looked for class with name {0}"
                    return document;
                }

                var setupMethodDeclaration = Constructor(classUnderTestName, classUnderTestDeclarationSyntax, generator);

                var root = await document.GetSyntaxRootAsync(cancellationToken);
                var members = classDecl.Members;
                members = GetNewClassWithSetupMethod(setupMethodDeclaration, root, members);
                members = GetNewClassWithFieldDeclarations(generator, classUnderTestDeclarationSyntax, members);

                var newClass = classDecl.WithMembers(members);
                var newRoot = root.ReplaceNode(classDecl, newClass);
                newRoot = await AddUsingDirectives(document, generator, newRoot);
                var newDocument = document.WithSyntaxRoot(newRoot);
                return newDocument;
            }
            catch
            {
                return document;
            }
        }

        private async Task<SyntaxNode> AddUsingDirectives(Document document, SyntaxGenerator generator, SyntaxNode newRoot)
        {
            var testClassDocumentSyntaxRoot = await document.GetSyntaxRootAsync();
            List<UsingDirectiveSyntax> usingDirectives = UsingDirectives(testClassDocumentSyntaxRoot, generator);
            var existingUsingDirectives = newRoot.DescendantNodes().OfType<UsingDirectiveSyntax>();

            var firstUsingDirectiveInFile = existingUsingDirectives.FirstOrDefault();
            var nodeToInsertAfter = firstUsingDirectiveInFile ?? newRoot.DescendantNodes().First();

            foreach (var usingDirective in usingDirectives)
            {
                if (!existingUsingDirectives.Any(_ => _.Name == usingDirective.Name))
                {
                    if (firstUsingDirectiveInFile != null)
                    {
                        newRoot = newRoot.InsertNodesAfter(firstUsingDirectiveInFile, usingDirectives);
                    }
                    //else
                    //{
                    //    newRoot = newRoot.InsertNodesBefore(nodeToInsertAfter, usingDirectives);
                    //}
                }
            }

            return newRoot;
        }

        private static SyntaxList<MemberDeclarationSyntax> GetNewClassWithSetupMethod(MemberDeclarationSyntax setupMethodDeclaration, SyntaxNode root, SyntaxList<MemberDeclarationSyntax> members)
        {
            var existingSetupMethod = root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                                                                        .FirstOrDefault(_ => _.AttributeLists.SelectMany(x => x.Attributes).Any(x => x.Name.ToFullString() == "SetUp"));

            if (existingSetupMethod != null)
            {
                members = members.Replace(existingSetupMethod, setupMethodDeclaration);
            }
            else
            {
                members = members.Insert(0, setupMethodDeclaration as MemberDeclarationSyntax);
            }

            return members;
        }

        private SyntaxList<MemberDeclarationSyntax> GetNewClassWithFieldDeclarations(SyntaxGenerator generator, ClassDeclarationSyntax classUnderTestDeclarationSyntax, SyntaxList<MemberDeclarationSyntax> members)
        {
            // Replace add field declarations
            IEnumerable<SyntaxNode> fieldDeclarations = FieldDeclarations(classUnderTestDeclarationSyntax, generator);

            var existingFieldDeclarationVariables = members.OfType<FieldDeclarationSyntax>().SelectMany(_ => _.Declaration.Variables).Select(_ => _.Identifier.Text);

            var index = 0;
            //foreach field replace or add
            foreach (FieldDeclarationSyntax fieldDeclaration in fieldDeclarations)
            {
                var fieldDeclarationText = fieldDeclaration.Declaration.Variables.Select(_ => _.Identifier.Text);
                if (!existingFieldDeclarationVariables.Any(_ => fieldDeclarationText.Contains(_)))
                {
                    members = members.Insert(index++, fieldDeclaration as MemberDeclarationSyntax);
                }
            }

            return members;
        }

        private static List<UsingDirectiveSyntax> UsingDirectives(SyntaxNode docSyntaxRoot, SyntaxGenerator generator)
        {
            //todo: add other usings for class under test
            return new List<UsingDirectiveSyntax>() { generator.NamespaceImportDeclaration("Moq") as UsingDirectiveSyntax };
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
            parameterTypeName = isInterface ? parameterTypeName.Remove(0,1) : parameterTypeName;
           var isGeneric= parameterTypeName.Contains("<");
            parameterTypeName =
                isGeneric ? parameterTypeName.Remove(parameterTypeName.IndexOf('<')) : parameterTypeName;
            return string.Format("_{0}", parameterTypeName.Substring(0, 1).ToLowerInvariant() + parameterTypeName.Substring(1));
        }

        private MemberDeclarationSyntax Constructor(string className, ClassDeclarationSyntax classDec, SyntaxGenerator generator)
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
                    var parameterType = SyntaxFactory.GenericName(
                        SyntaxFactory.Identifier("Mock"),
                        SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(new []{ parameter.Type})));
                    var fieldName = GetParameterFieldName(parameter);
                    var fieldDec = generator.FieldDeclaration(fieldName
                                                            , parameterType
                                                            , Accessibility.Private);
                    fieldDeclarations.Add(fieldDec);

                    var fieldIdentifier = generator.IdentifierName(fieldName);
                    
                    var parameterTypeIdentifier = generator.IdentifierName(parameterType.ToString());
                    var mocksRepositoryIdentifier = generator.GenericName("Mock", parameterTypeIdentifier);

                    var fieldInitializationExpression = generator.ObjectCreationExpression(mocksRepositoryIdentifier);
                    var expressionStatementFieldInstantiation = generator.AssignmentStatement(fieldIdentifier, fieldInitializationExpression);

                    expressionStatements.Add(expressionStatementFieldInstantiation);
                }
            }

            var constructorParameters = fieldDeclarations.SelectMany(x => x.DescendantNodes().OfType<VariableDeclaratorSyntax>().Select(y => y.Identifier.Text));

            var setupBody = new List<SyntaxNode>();
            setupBody.AddRange(expressionStatements);

            var targetObjectCreationExpression = generator.ObjectCreationExpression(generator.IdentifierName(className), constructorParameters.Select(x =>generator.MemberAccessExpression(generator.IdentifierName(x),"Object" )));

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
            return setupMethodWithSetupAttribute as MemberDeclarationSyntax;
        }
    }
}