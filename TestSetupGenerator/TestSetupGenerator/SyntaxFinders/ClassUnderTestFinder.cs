using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;

namespace TestSetupGenerator.SyntaxFinders
{
    public class ClassUnderTestFinder
    {
        public async Task<ClassDeclarationSyntax> GetAsync(Solution solution, string className)
        {
            foreach (var project in solution.Projects)
            {
                var symbols = await SymbolFinder.FindDeclarationsAsync(project, className, true);
                var syntaxDec = symbols.Select(_ => _.DeclaringSyntaxReferences).OfType<ClassDeclarationSyntax>().FirstOrDefault();

                if (syntaxDec != null)
                    return syntaxDec;
            }

            return null;
        }
    }
}
