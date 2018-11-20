using AutoSetup.CodeAnalyzers;
using AutoSetup.CodeGenerators;
using SimpleInjector;

namespace AutoSetup
{
    public class IoCConfig
    {
        private Container _container;
        public Container Container => _container ?? (_container = Configure());

        private Container Configure()
        {
            var container = new Container();
            container.Options.AllowOverridingRegistrations = true;

            // CodeAnalyzers
            container.RegisterSingleton<IClassUnderTestFinder, ClassUnderTestFinder>();
            container.RegisterSingleton<IClassUnderTestNameFinder, ClassUnderTestNameFinder>();
            container.RegisterSingleton<IConstructorParametersExtractor, ConstructorParametersExtractor>();
            container.Register<IFieldFinder, FieldFinder>();
            container.RegisterSingleton<IMemberFinder, MemberFinder>();

            // CodeGenerators
            container.RegisterSingleton<IConstructorGenerator, ConstructorGenerator>();
            container.RegisterSingleton<IExpressionStatementGenerator, ExpressionStatementGenerator>();
            container.RegisterSingleton<IFieldDeclarationGenerator, FieldDeclarationGenerator>();
            container.RegisterSingleton<IFieldNameGenerator, FieldNameGenerator>();
            container.RegisterSingleton<IMethodGenerator, MethodGenerator>();
            container.RegisterSingleton<IUsingDirectivesGenerator, UsingDirectivesGenerator>();
            
            container.Register<ISetupMethodBodyBuilder, SetupMethodBodyBuilder>();
            container.Register<IXUnitSetupGenerator, XUnitSetupGenerator>();

            return container;
        }
    }
}
