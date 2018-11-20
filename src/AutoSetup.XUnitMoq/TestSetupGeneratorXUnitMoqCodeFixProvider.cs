using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoSetup.XUnitMoq
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TestSetupGeneratorXUnitMoqCodeFixProvider)), Shared]
    public class TestSetupGeneratorXUnitMoqCodeFixProvider : CodeFixProvider
    {
        private const string title = "(Re-)Generate SetUp";
        private readonly IXUnitSetupGenerator _xUnitSetupGenerator = new IoCConfig().Container.GetInstance<IXUnitSetupGenerator>();

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(TestSetupGeneratorXUnitMoqAnalyzer.DiagnosticId);

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

            try
            {
                // Register a code action that will invoke the fix.
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: title,
                        createChangedDocument: c => _xUnitSetupGenerator.RegenerateSetup(context.Document, declaration, c),
                        equivalenceKey: title),
                    diagnostic);
            }
            catch
            {

            }
        }
    }
}
