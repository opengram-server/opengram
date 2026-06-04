using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace MyTelegram.Core;

public class DefaultObjectMapper<TContext>(IServiceProvider serviceProvider)
    : DefaultObjectMapper(serviceProvider), IObjectMapper<TContext>;

public class DefaultObjectMapper(IServiceProvider serviceProvider) : IObjectMapper
{
    protected IServiceProvider ServiceProvider { get; } = serviceProvider;

    [return: NotNullIfNotNull("source")]
    public virtual TDestination? Map<TSource, TDestination>(TSource source)
    {
        if (source == null)
        {
            return default;
        }

        var specificMapper = ServiceProvider.GetService<IObjectMapper<TSource, TDestination>>();
        if (specificMapper != null)
        {
            return specificMapper.Map(source);
        }

        throw new InvalidOperationException(
            $"Can not map source type {typeof(TSource).FullName} to destination type {typeof(TDestination).FullName}");
    }

    [return: NotNullIfNotNull("source")]
    public virtual TDestination? Map<TSource, TDestination>(TSource source,
        TDestination destination)
    {
        if (source == null)
        {
            return default;
        }

        var specificMapper = ServiceProvider.GetService<IObjectMapper<TSource, TDestination>>();
        if (specificMapper != null)
        {
            return specificMapper.Map(source, destination);
        }

        throw new InvalidOperationException(
            $"Can not map source type {typeof(TSource).FullName} to destination type {typeof(TDestination).FullName}");
    }
}

/// <summary>
///     Defines a simple interface to automatically map objects for a specific context.
/// </summary>
// ReSharper disable once UnusedTypeParameter
public interface IObjectMapper<TContext> : IObjectMapper
{
}

/// <summary>
///     Maps an object to another.
///     Implement this interface to override object to object mapping for specific types.
/// </summary>
/// <typeparam name="TSource"></typeparam>
/// <typeparam name="TDestination"></typeparam>
public interface IObjectMapper<in TSource, TDestination>
{
    /// <summary>
    ///     Converts an object to another. Creates a new object of <see cref="TDestination" />.
    /// </summary>
    /// <param name="source">Source object</param>
    [return: NotNullIfNotNull("source")]
    TDestination? Map(TSource source);

    /// <summary>
    ///     Execute a mapping from the source object to the existing destination object
    /// </summary>
    /// <param name="source">Source object</param>
    /// <param name="destination">Destination object</param>
    /// <returns>Returns the same <see cref="destination" /> object after mapping operation</returns>
    [return: NotNullIfNotNull("source")]
    TDestination? Map(TSource source,
        TDestination destination);
}