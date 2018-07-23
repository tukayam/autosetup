using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestSetupGenerator.CodeAnalysis.Models;

namespace TestSetupGenerator.CodeAnalysis.CodeAnalyzers
{
    public interface IClassUnderTestFinder
    {
        Task<ClassUnderTest> GetAsync(Solution solution, string testProjectName, string className);
    }

    public class ClassUnderTestFinder : IClassUnderTestFinder
    {
        /*string testProjectName,*/
        public async Task<ClassUnderTest> GetAsync(Solution solution, string testProjectName, string className)
        {
            var projectNameUnderTest = testProjectName.Replace(".UnitTests", string.Empty).Replace(".Tests", string.Empty);
            var project = solution.Projects.FirstOrDefault(_ => _.Name == projectNameUnderTest);

            if (project != null)
            {
                var classDec = await TryGetClassUnderTestInProject(project, className);
                if (classDec != null)
                {
                    return classDec;
                }
            }

            var projects = project != null ? solution.Projects.Where(_ => _.Id != project.Id) : solution.Projects;
            foreach (var proj in projects)
            {
                var classDec = await TryGetClassUnderTestInProject(proj, className);
                if (classDec != null)
                {
                    return classDec;
                }
            }

            return null;
        }

        private async Task<ClassUnderTest> TryGetClassUnderTestInProject(Project project, string className)
        {
            foreach (var document in project.Documents)
            {
                var root = await document.GetSyntaxRootAsync();
                var syntaxDec = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault(_ => _.Identifier.Text == className);

                if (syntaxDec != null)
                {
                    var semanticModel = await document.GetSemanticModelAsync();
                    return new ClassUnderTest(syntaxDec, semanticModel);
                }
            }

            return null;
        }
    }
}