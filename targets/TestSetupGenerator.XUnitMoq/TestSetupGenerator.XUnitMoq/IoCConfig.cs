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

            // DataAccess
            container.RegisterSingleton<IConstructorParametersExtractor, ConstructorParametersExtractor>();
            container.RegisterSingleton<IExpressionStatementGenerator, ExpressionStatementGenerator>();
            container.RegisterSingleton<IFieldNameGenerator, FieldNameGenerator>();
            container.RegisterSingleton<IFieldDeclarationsGenerator, FieldDeclarationsBuilder>();
            container.RegisterSingleton<ISetupMethodBodyGenerator, SetupMethodBodyBuilder>();
            container.RegisterSingleton<IConstructorGenerator, ConstructorGenerator>();

            container.RegisterSingleton<IXUnitSetupGenerator, XUnitSetupGenerator>();

            return container;
        }
    }
}
