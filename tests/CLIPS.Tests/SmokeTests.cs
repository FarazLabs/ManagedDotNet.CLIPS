using ManagedDotNet.Clips.Api.Values;
using ManagedDotNet.Clips.Core;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CLIPS.Tests;

public class SmokeTests
{
    [Fact]
    public void Eval_AddsIntegers()
    {
        using var env = new ManagedDotNet.Clips.Core.Clips(NullLogger.Instance);
        var result = env.Eval<ClipsIntegerValue>("(+ 1 2)");
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public void Reset_Succeeds()
    {
        using var env = new ManagedDotNet.Clips.Core.Clips(NullLogger.Instance);
        Assert.True(env.Reset());
    }

    [Fact]
    public void GetModules_IncludesMain()
    {
        using var env = new ManagedDotNet.Clips.Core.Clips(NullLogger.Instance);
        string[] modules = env.GetModules();
        Assert.Contains("MAIN", modules);
    }
}
