using ManagedDotNet.Clips.Core;

namespace ManagedDotNet.Clips.Api;

public interface IClipsPool
{
    IClips Rent();
    void Return(IClips engine);
}

public interface IClipsPool<T> where T : class, IClips
{
    T Rent();
    void Return(T environment);
}
