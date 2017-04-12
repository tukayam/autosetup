using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestSetupGenerator.SyntaxFinders
{
    public class ClassUnderTestFinder
    {
        public async Task<ClassDeclarationSyntax> GetAsync(Solution solution, string className)
        {
            foreach (var project in solution.Projects)
            {
                foreach (var document in project.Documents)
                {
                    var root =await document.GetSyntaxRootAsync();
                    var syntaxDec = root.DescendantNodes().OfType<ClassDeclarationSyntax>().Where(_ => _.Identifier.Text == className).FirstOrDefault();
                    
                    if (syntaxDec != null)
                        return syntaxDec;
                }
            }

            return null;
        }
    }
}
