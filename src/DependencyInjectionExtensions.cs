using ManagedDotNet.Clips.Abstractions;
using ManagedDotNet.Clips.Api;
using ManagedDotNet.Clips.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;

namespace ManagedDotNet.Clips;

public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds a CLIPS rules engine and its pool as a keyed service.
    /// Each keyed instance uses a dedicated environment builder.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="builderFactory">A factory that provides an initialized <see cref="IClipsConfigurer"/> using the service provider.</param>
    /// <param name="key">The key to identify this CLIPS instance in the DI container.</param>
    /// <returns>The modified service collection.</returns>
    public static IServiceCollection AddClipsPool(this IServiceCollection services, string key, Func<IServiceProvider, IClipsConfigurer> builderFactory)
    {
        // Register the factory as keyed service - convert the service provider factory to a parameterless factory
        services.AddKeyedSingleton<Func<IServiceProvider, IClipsConfigurer>>(key, builderFactory);


        // Register keyed object pool for this specific CLIPS instance
        // Qualify Core.Clips — simple name "Clips" collides with this namespace.
        services.AddKeyedSingleton<ObjectPool<Core.Clips>>(key, (provider, _) =>
        {
            var policy = new ClipsPooledObjectPolicy(provider, key);
            var poolProvider = provider.GetRequiredService<ObjectPoolProvider>();
            ObjectPool<Core.Clips> objectPool = poolProvider.Create(policy);
            return objectPool;
        });

        // Add the pooled service
        services.AddKeyedTransient<IClipsPool>(key, (provider, _) =>
        {
            var pool = provider.GetRequiredKeyedService<ObjectPool<Core.Clips>>(key);
            return new ClipsPool(pool);
        });

        // Create a pool of BusinessRules
        if (!services.Any(sd => sd.ServiceType == typeof(ObjectPoolProvider)))
        {
            services.TryAddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        }

        return services;
    }

    /// <summary>
    /// Warms a keyed <see cref="IClipsPool"/> by taking and releasing
    /// a specified number of instances from the pool.
    /// </summary>
    /// <param name="app">The application builder used to access services.</param>
    /// <param name="key">The key identifying the CLIPS rules engine pool.</param>
    /// <param name="count">The number of instances to initialize (default: 10).</param>
    public static void WarmUpClipsPool(this IApplicationBuilder app, string key, int count = 10)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var pool = scope.ServiceProvider.GetRequiredKeyedService<ObjectPool<Core.Clips>>(key);

        var items = new List<Core.Clips>(count);

        // Take objects from the pool
        for (int i = 0; i < count; i++)
        {
            items.Add(pool.Get());
        }

        // Return them to make them ready in the pool
        foreach (var item in items)
        {
            pool.Return(item);
        }
    }

}