using ManagedDotNet.Clips.Api.Values;
using ManagedDotNet.Clips.Core;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ManagedDotNet.Clips.Tests;

public class SmokeTests
{
    [Fact]
    public void Eval_AddsIntegers()
    {
        using ManagedDotNet.Clips.Core.Clips env = new ManagedDotNet.Clips.Core.Clips(NullLogger.Instance);
        ClipsIntegerValue result = env.Eval<ClipsIntegerValue>("(+ 1 2)");
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public void Reset_Succeeds()
    {
        using ManagedDotNet.Clips.Core.Clips env = new ManagedDotNet.Clips.Core.Clips(NullLogger.Instance);
        Assert.True(env.Reset());
    }

    [Fact]
    public void GetModules_IncludesMain()
    {
        using ManagedDotNet.Clips.Core.Clips env = new ManagedDotNet.Clips.Core.Clips(NullLogger.Instance);
        string[] modules = env.GetModules();
        Assert.Contains("MAIN", modules);
    }
}
