using AutoSetup.CodeAnalyzers;
using AutoSetup.CodeGenerators;

namespace AutoSetup
{
    public class IoCConfig
    {
        public IXUnitSetupGenerator GetInstance()
        {
            var classUnderTestFinder = new ClassUnderTestFinder();
            return GetInstance(classUnderTestFinder);
        }

        public IXUnitSetupGenerator GetInstance(IClassUnderTestFinder classUnderTestFinder)
        {
            var classUnderTestNameFinder = new ClassUnderTestNameFinder();
            var constructorParametersExtractor = new ConstructorParametersExtractor();
            var fieldFinder = new FieldFinder();
            var memberFinder = new MemberFinder();
            var constructorGenerator = new ConstructorGenerator();
            var expressionStatementGenerator = new ExpressionStatementGenerator();
            var fieldNameGenerator = new FieldNameGenerator();
            var fieldDeclarationGenerator = new FieldDeclarationGenerator(fieldNameGenerator);
            var methodGenerator = new MethodGenerator();
            var usingDirectivesGenerator = new UsingDirectivesGenerator();

            var setupMethodBodyBuilder = new SetupMethodBodyBuilder(constructorParametersExtractor, expressionStatementGenerator, fieldNameGenerator);
            return new XUnitSetupGenerator(classUnderTestNameFinder, classUnderTestFinder, constructorParametersExtractor, fieldDeclarationGenerator, setupMethodBodyBuilder, constructorGenerator, usingDirectivesGenerator, memberFinder, fieldFinder);
        }
    }
}
