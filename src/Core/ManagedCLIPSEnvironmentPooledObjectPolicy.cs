using ManagedDotNet.CLIPS.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Environment = CLIPSNET.Environment;

namespace ManagedDotNet.CLIPS.Core;

internal class ManagedCLIPSEnvironmentPooledObjectPolicy : PooledObjectPolicy<ManagedCLIPSEnvironment>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly string _key; 
    internal ManagedCLIPSEnvironmentPooledObjectPolicy
    (
        IServiceProvider serviceProvider,
        string key
    )
    {
        _serviceProvider = serviceProvider;
        _key = key;
    }

    public override ManagedCLIPSEnvironment Create()
    {
        // Retrieve the logger service for the ManagedCLIPSEnvironment
        // This logger will be used for debugging and monitoring CLIPS operations
        var logger = _serviceProvider.GetService<ILogger<ManagedCLIPSEnvironment>>();
        if (logger == null) throw new InvalidOperationException("ILogger<> is not available in the service provider.");

        // Get the keyed environment configurer factory from dependency injection
        // The factory returns an IEnvironmentConfigurer instance associated with the current key
        // This allows different environment configurations to be registered with different keys
        Func<IServiceProvider, IEnvironmentConfigurer> configurer_factory = _serviceProvider.GetRequiredKeyedService<Func<IServiceProvider, IEnvironmentConfigurer>>(_key);

        // Create the environment configurer instance using the factory
        // This configurer will be responsible for setting up the CLIPS environment with rules and functions
        IEnvironmentConfigurer configurer = configurer_factory(_serviceProvider);

        // Create a new ManagedCLIPSEnvironment instance with the logger
        var environment = new ManagedCLIPSEnvironment(logger);


        // Configure the environment using the configurer
        // This loads CLIPS rules, defines functions, and sets up the environment for use
        configurer.Configure(environment);

        // Reset the environment to ensure it starts in a clean state 
        // for example the module will not be main if not reset
        // This guarantees consistent behavior between different uses of the pooled environment
        environment.Reset();

        return environment;
    }

    public override bool Return(ManagedCLIPSEnvironment obj)
    {
        return obj.Reset();
    }
}