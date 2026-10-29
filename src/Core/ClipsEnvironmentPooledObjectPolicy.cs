using ManagedDotNet.Clips.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;

namespace ManagedDotNet.Clips.Core;

internal class ClipsEnvironmentPooledObjectPolicy : PooledObjectPolicy<ClipsEnvironment>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly string _key;

    internal ClipsEnvironmentPooledObjectPolicy(IServiceProvider serviceProvider, string key)
    {
        _serviceProvider = serviceProvider;
        _key = key;
    }

    public override ClipsEnvironment Create()
    {
        var logger = _serviceProvider.GetService<ILogger<ClipsEnvironment>>();
        if (logger == null)
        {
            throw new InvalidOperationException("ILogger<> is not available in the service provider.");
        }

        Func<IServiceProvider, IClipsEnvironmentConfigurator> configurator =
            _serviceProvider.GetRequiredKeyedService<Func<IServiceProvider, IClipsEnvironmentConfigurator>>(_key);

        IClipsEnvironmentConfigurator configurer = configurator(_serviceProvider);
        var environment = new ClipsEnvironment(logger);
        configurer.Configure(environment);
        environment.Reset();
        return environment;
    }

    public override bool Return(ClipsEnvironment obj) => obj.Reset();
}
