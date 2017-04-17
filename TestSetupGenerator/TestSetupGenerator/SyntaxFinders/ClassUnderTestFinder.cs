using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestSetupGenerator.SyntaxFinders
{
    public class ClassUnderTestFinder
    {
        public async Task<ClassDeclarationSyntax> GetAsync(string testProjectName, Solution solution, string className)
        {
            var projectNameUnderTest = testProjectName.Replace(".UnitTests", string.Empty).Replace(".Tests", string.Empty);
            var project = solution.Projects.FirstOrDefault(_ => _.Name == projectNameUnderTest);
            if (project == null)
            {
                return null;
            }

            foreach (var document in project.Documents)
            {
                var root = await document.GetSyntaxRootAsync();
                var syntaxDec = root.DescendantNodes().OfType<ClassDeclarationSyntax>().Where(_ => _.Identifier.Text == className).FirstOrDefault();

                if (syntaxDec != null)
                {
                    return syntaxDec;
                }
            }

            return null;
        }
    }
}
