using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MyTelegram.Abstractions;
public static class MyTelegramAbstractionsExtensions
{
    public static void RegisterServices(this IServiceCollection services, Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();

        var singletonBaseInterface = typeof(ISingletonDependency);
        var transientBaseInterface = typeof(ITransientDependency);

        var types = assembly.GetTypes()
                .Where(p => p != singletonBaseInterface && p != transientBaseInterface && !p.IsAbstract)
                .ToList()
            ;
        var singletonTypes = types.Where(singletonBaseInterface.IsAssignableFrom);
        var transientTypes = types.Where(transientBaseInterface.IsAssignableFrom);

        foreach (var type in singletonTypes)
        {
            var baseInterfaces = type.GetInterfaces();
            foreach (var baseInterface in baseInterfaces)
            {
                services.AddSingleton(baseInterface, type);
            }

            services.AddSingleton(type);
        }

        foreach (var type in transientTypes)
        {
            var baseInterfaces = type.GetInterfaces();
            foreach (var baseInterface in baseInterfaces)
            {
                services.AddTransient(baseInterface, type);
            }

            services.AddTransient(type);
        }
    }
}
