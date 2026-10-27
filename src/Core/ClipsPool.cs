using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagedDotNet.Clips.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;

namespace ManagedDotNet.Clips.Core;

internal class ClipsPool : IClipsPool, IClipsPool<Clips>
{
    private readonly ObjectPool<Clips> _pool;

    // Add constructor with keyed service injection
    public ClipsPool(ObjectPool<Clips> pool)
    {
        _pool = pool;
    }

    // Explicit implementation for the generic pool
    Clips IClipsPool<Clips>.Rent()
        => _pool.Get();

    void IClipsPool<Clips>.Return(Clips environment)
        => _pool.Return(environment);


    // Explicit implementation for the non-generic pool
    IClips IClipsPool.Rent()
        => _pool.Get();

    void IClipsPool.Return(IClips engine)
        => _pool.Return((Clips)engine);
}
