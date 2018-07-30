using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using TestSetupGenerator.CodeAnalysis;
using TestSetupGenerator.CodeAnalysis.CodeAnalyzers;
using TestSetupGenerator.CodeAnalysis.CodeGenerators;

namespace TestSetupGenerator.XUnitMoq
{
    public interface IXUnitSetupGenerator
    {
        Task<Document> RegenerateSetup(Document document, ClassDeclarationSyntax testClass, CancellationToken cancellationToken);
    }

    public class XUnitSetupGenerator : IXUnitSetupGenerator
    {
        private readonly IClassUnderTestNameFinder _classUnderTestNameFinder;
        private readonly IClassUnderTestFinder _classUnderTestFinder;
        private readonly IConstructorParametersExtractor _constructorParametersExtractor;
        private readonly IFieldDeclarationGenerator _fieldDeclarationGenerator;
        private readonly ISetupMethodBodyBuilder _setupMethodBodyGenerator;
        private readonly IConstructorGenerator _constructorGenerator;
        private readonly IUsingDirectivesGenerator _usingDirectivesGenerator;
        private readonly IMemberFinder _memberFinder;

        public XUnitSetupGenerator(IClassUnderTestNameFinder classUnderTestNameFinder,
                                    IClassUnderTestFinder classUnderTestFinder,
                                    IConstructorParametersExtractor constructorParametersExtractor,
                                    IFieldDeclarationGenerator fieldDeclarationGenerator,
                                    ISetupMethodBodyBuilder setupMethodBodyGenerator,
                                    IConstructorGenerator constructorGenerator,
                                    IUsingDirectivesGenerator usingDirectivesGenerator,
                                    IMemberFinder memberFinder)
        {
            _classUnderTestNameFinder = classUnderTestNameFinder;
            _classUnderTestFinder = classUnderTestFinder;
            _constructorParametersExtractor = constructorParametersExtractor;
            _fieldDeclarationGenerator = fieldDeclarationGenerator;
            _setupMethodBodyGenerator = setupMethodBodyGenerator;
            _constructorGenerator = constructorGenerator;
            _usingDirectivesGenerator = usingDirectivesGenerator;
            _memberFinder = memberFinder;
        }

        public async Task<Document> RegenerateSetup(Document document, ClassDeclarationSyntax testClass, CancellationToken cancellationToken)
        {
            try
            {
                var generator = SyntaxGenerator.GetGenerator(document);

                var testProjectName = document.Project.Name;
                var testClassName = testClass.Identifier.Text;
                var classUnderTestName = _classUnderTestNameFinder.GetClassUnderTestName(testClassName);
                var classUnderTest = await _classUnderTestFinder.GetAsync(document.Project.Solution, testProjectName, classUnderTestName);

                if (classUnderTest == null)
                {
                    return document;
                }

                var setupMethodDeclaration = Constructor(testClassName, classUnderTest.ClassDeclarationSyntax, generator);

                var constructorParameters = _constructorParametersExtractor.GetParametersOfConstructor(classUnderTest.ClassDeclarationSyntax).ToList();
                var genericSymbolForMoq = "Mock";
                var fieldDeclarations = constructorParameters.Select(_ => _fieldDeclarationGenerator.GetGenericFieldDeclaration(_, genericSymbolForMoq, generator));

                var namespaceForMoq = "Moq";
                var usings = _usingDirectivesGenerator.UsingDirectives(new[] { namespaceForMoq }, generator);

                var newDocument = await new DocumentBuilder(_memberFinder, document, testClass)
                                    .WithSetupMethod(setupMethodDeclaration)
                                    .WithFields(fieldDeclarations)
                                    .WithUsings(usings)
                                    .BuildAsync(cancellationToken);
                return newDocument;
            }
            catch
            {
                return document;
            }
        }

        private MethodDeclarationSyntax Constructor(string testClassName, ClassDeclarationSyntax classUnderTestDec, SyntaxGenerator generator)
        {
            var methodBody = _setupMethodBodyGenerator.GetSetupMethodBodyMembers(classUnderTestDec, generator);
            return _constructorGenerator.Constructor(testClassName, methodBody, generator) as MethodDeclarationSyntax;
        }
    }
}