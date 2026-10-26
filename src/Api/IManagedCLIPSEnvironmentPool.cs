using CLIPSNET;
using ManagedDotNet.CLIPS.Abstractions;
using ManagedDotNet.CLIPS.Core;

namespace ManagedDotNet.CLIPS.Api;

public interface IManagedCLIPSEnvironmentPool
{
    IManagedCLIPSEnvironment Rent();
    void Return(IManagedCLIPSEnvironment engine);
}

public interface IManagedCLIPSEnvironmentPool<T> where T : class, IManagedCLIPSEnvironment
{
    T Rent();
    void Return(T environment);
}
