using ManagedDotNet.CLIPS.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;

namespace ManagedDotNet.CLIPS.Core;

internal class ManagedCLIPSEnvironmentPooledObjectPolicy : PooledObjectPolicy<ManagedCLIPSEnvironment>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly string _key;

    internal ManagedCLIPSEnvironmentPooledObjectPolicy(IServiceProvider serviceProvider, string key)
    {
        _serviceProvider = serviceProvider;
        _key = key;
    }

    public override ManagedCLIPSEnvironment Create()
    {
        var logger = _serviceProvider.GetService<ILogger<ManagedCLIPSEnvironment>>();
        if (logger == null)
        {
            throw new InvalidOperationException("ILogger<> is not available in the service provider.");
        }

        Func<IServiceProvider, IEnvironmentConfigurer> configurerFactory =
            _serviceProvider.GetRequiredKeyedService<Func<IServiceProvider, IEnvironmentConfigurer>>(_key);

        IEnvironmentConfigurer configurer = configurerFactory(_serviceProvider);
        var environment = new ManagedCLIPSEnvironment(logger);
        configurer.Configure(environment);
        environment.Reset();
        return environment;
    }

    public override bool Return(ManagedCLIPSEnvironment obj) => obj.Reset();
}
