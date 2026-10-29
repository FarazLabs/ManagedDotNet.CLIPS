using ManagedDotNet.Clips.Core;

namespace ManagedDotNet.Clips.Api;

public interface IClipsEnvironmentPool
{
    IClipsEnvironment Rent();
    void Return(IClipsEnvironment engine);
}

public interface IClipsEnvironmentPool<T> where T : class, IClipsEnvironment
{
    T Rent();
    void Return(T environment);
}
