using ManagedDotNet.Clips.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;

namespace ManagedDotNet.Clips.Core;

internal class ClipsPooledObjectPolicy : PooledObjectPolicy<Clips>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly string _key;

    internal ClipsPooledObjectPolicy(IServiceProvider serviceProvider, string key)
    {
        _serviceProvider = serviceProvider;
        _key = key;
    }

    public override Clips Create()
    {
        var logger = _serviceProvider.GetService<ILogger<Clips>>();
        if (logger == null)
        {
            throw new InvalidOperationException("ILogger<> is not available in the service provider.");
        }

        Func<IServiceProvider, IClipsConfigurer> configurerFactory =
            _serviceProvider.GetRequiredKeyedService<Func<IServiceProvider, IClipsConfigurer>>(_key);

        IClipsConfigurer configurer = configurerFactory(_serviceProvider);
        var environment = new Clips(logger);
        configurer.Configure(environment);
        environment.Reset();
        return environment;
    }

    public override bool Return(Clips obj) => obj.Reset();
}
