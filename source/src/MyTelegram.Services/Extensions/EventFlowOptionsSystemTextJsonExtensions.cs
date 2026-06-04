using EventFlow;
using Microsoft.Extensions.DependencyInjection;
using MyTelegram.Services.NativeAot;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace MyTelegram.Services.Extensions;

public static class EventFlowOptionsSystemTextJsonExtensions
{
    public static IEventFlowOptions AddSystemTextJson(
        this IEventFlowOptions eventFlowOptions,
        Action<JsonSerializerOptions>? configure = null)
    {
        eventFlowOptions.ServiceCollection.AddSystemTextJson(configure);

        return eventFlowOptions;
    }

    public static IServiceCollection AddSystemTextJson(this IServiceCollection services,
        Action<JsonSerializerOptions>? configure = null)
    {
        services.AddMySystemTextJson(options =>
        {
            options.TypeInfoResolverChain.Add(MyJsonSerializeContext.Default);
            configure?.Invoke(options);

            // If the JsonSerializerContext has too many JsonSerializableAttributes, the compilation speed will be significantly reduced.
            // To improve compilation speed, remove the JsonSerializableAttribute of JsonSerializeContext
            // in debug mode and fallback to reflection mode.
            options.TypeInfoResolverChain.Add(new DefaultJsonTypeInfoResolver());
        });

        return services;
    }
}