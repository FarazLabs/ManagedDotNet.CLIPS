using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagedDotNet.CLIPS.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;

namespace ManagedDotNet.CLIPS.Core;

internal class ManagedCLIPSEnvironmentRulesEnginePool : IManagedCLIPSEnvironmentPool, IManagedCLIPSEnvironmentPool<ManagedCLIPSEnvironment>
{
    private readonly ObjectPool<ManagedCLIPSEnvironment> _pool;

    // Add constructor with keyed service injection
    public ManagedCLIPSEnvironmentRulesEnginePool(ObjectPool<ManagedCLIPSEnvironment> pool)
    {
        _pool = pool;
    }

    // Explicit implementation for the generic pool
    ManagedCLIPSEnvironment IManagedCLIPSEnvironmentPool<ManagedCLIPSEnvironment>.Rent()
        => _pool.Get();

    void IManagedCLIPSEnvironmentPool<ManagedCLIPSEnvironment>.Return(ManagedCLIPSEnvironment environment)
        => _pool.Return(environment);


    // Explicit implementation for the non-generic pool
    IManagedCLIPSEnvironment IManagedCLIPSEnvironmentPool.Rent()
        => _pool.Get();

    void IManagedCLIPSEnvironmentPool.Return(IManagedCLIPSEnvironment engine)
        => _pool.Return((ManagedCLIPSEnvironment)engine);
}
