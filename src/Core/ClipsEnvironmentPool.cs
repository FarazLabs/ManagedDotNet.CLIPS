using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagedDotNet.Clips.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;

namespace ManagedDotNet.Clips.Core;

internal class ClipsEnvironmentPool : IClipsEnvironmentPool, IClipsEnvironmentPool<ClipsEnvironment>
{
    private readonly ObjectPool<ClipsEnvironment> _pool;

    // Add constructor with keyed service injection
    public ClipsEnvironmentPool(ObjectPool<ClipsEnvironment> pool)
    {
        _pool = pool;
    }

    // Explicit implementation for the generic pool
    ClipsEnvironment IClipsEnvironmentPool<ClipsEnvironment>.Rent()
        => _pool.Get();

    void IClipsEnvironmentPool<ClipsEnvironment>.Return(ClipsEnvironment environment)
        => _pool.Return(environment);


    // Explicit implementation for the non-generic pool
    IClipsEnvironment IClipsEnvironmentPool.Rent()
        => _pool.Get();

    void IClipsEnvironmentPool.Return(IClipsEnvironment engine)
        => _pool.Return((ClipsEnvironment)engine);
}
