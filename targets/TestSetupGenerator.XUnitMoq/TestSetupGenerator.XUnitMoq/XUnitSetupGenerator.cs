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
        private readonly IFieldDeclarationsBuilder _fieldDeclarationsGenerator;
        private readonly ISetupMethodBodyBuilder _setupMethodBodyGenerator;
        private readonly IConstructorGenerator _constructorGenerator;
        private readonly IUsingDirectivesGenerator _usingDirectivesGenerator;
        private readonly IMemberReplacer _memberReplacer;

        public XUnitSetupGenerator(IClassUnderTestNameFinder classUnderTestNameFinder,
                                    IClassUnderTestFinder classUnderTestFinder,
                                    IFieldDeclarationsBuilder fieldDeclarationsGenerator,
                                    ISetupMethodBodyBuilder setupMethodBodyGenerator,
                                    IConstructorGenerator constructorGenerator,
                                    IUsingDirectivesGenerator usingDirectivesGenerator,
                                    IMemberReplacer memberReplacer)
        {
            _classUnderTestNameFinder = classUnderTestNameFinder;
            _classUnderTestFinder = classUnderTestFinder;
            _fieldDeclarationsGenerator = fieldDeclarationsGenerator;
            _setupMethodBodyGenerator = setupMethodBodyGenerator;
            _constructorGenerator = constructorGenerator;
            _usingDirectivesGenerator = usingDirectivesGenerator;
            _memberReplacer = memberReplacer;
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

                var genericSymbolForMoq = "Mock";
                var fieldDeclarations = _fieldDeclarationsGenerator.GetFieldDeclarationsAsGeneric(classUnderTest.ClassDeclarationSyntax, genericSymbolForMoq, generator);

                var namespaceForMoq = "Moq";
                var usings = _usingDirectivesGenerator.UsingDirectives(new[] { namespaceForMoq }, generator);

                var newDocument = await new DocumentBuilder(_memberReplacer, document,testClass)
                                    .WithSetupMethodAsync(setupMethodDeclaration)
                                    .WithFieldsAsync(fieldDeclarations)
                                    .WithUsingsAsync(usings)
                                    .Build();
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