using SimpleInjector;
using TestSetupGenerator.CodeAnalysis;
using TestSetupGenerator.CodeAnalysis.CodeAnalyzers;
using TestSetupGenerator.CodeAnalysis.CodeGenerators;

namespace TestSetupGenerator.XUnitMoq
{
    public class IoCConfig
    {
        private static Container _container;
        public static Container Container => _container ?? (_container = Configure());

        private static Container Configure()
        {
            var container = new Container();
            container.Options.AllowOverridingRegistrations = true;

            // CodeAnalyzers
            container.RegisterSingleton<IClassUnderTestFinder, ClassUnderTestFinder>();
            container.RegisterSingleton<IClassUnderTestNameFinder, ClassUnderTestNameFinder>();
            container.RegisterSingleton<IConstructorParametersExtractor, ConstructorParametersExtractor>();
            container.RegisterSingleton<IMemberFinder, MemberFinder>();

            // CodeGenerators
            container.RegisterSingleton<IConstructorGenerator, ConstructorGenerator>();
            container.RegisterSingleton<IExpressionStatementGenerator, ExpressionStatementGenerator>();
            container.RegisterSingleton<IFieldDeclarationGenerator, FieldDeclarationGenerator>();
            container.RegisterSingleton<IFieldNameGenerator, FieldNameGenerator>();
            container.RegisterSingleton<IMethodGenerator, MethodGenerator>();
            container.RegisterSingleton<IUsingDirectivesGenerator, UsingDirectivesGenerator>();

            container.Register<IDocumentBuilder, DocumentBuilder>();
            container.Register<IFieldDeclarationsBuilder, FieldDeclarationsBuilder>();
            container.Register<IMemberReplacer, MemberReplacer>();
            container.Register<ISetupMethodBodyBuilder, SetupMethodBodyBuilder>();
            
            container.Register<IXUnitSetupGenerator, XUnitSetupGenerator>();

            return container;
        }
    }
}
