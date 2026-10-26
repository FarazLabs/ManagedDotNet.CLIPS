using ManagedDotNet.CLIPS.Api.Values;
using ManagedDotNet.CLIPS.Core;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CLIPS.Tests;

public class SmokeTests
{
    [Fact]
    public void Eval_AddsIntegers()
    {
        using var env = new ManagedCLIPSEnvironment(NullLogger.Instance);
        var result = env.Eval<CLIPSIntegerValue>("(+ 1 2)");
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public void Reset_Succeeds()
    {
        using var env = new ManagedCLIPSEnvironment(NullLogger.Instance);
        Assert.True(env.Reset());
    }

    [Fact]
    public void GetModules_IncludesMain()
    {
        using var env = new ManagedCLIPSEnvironment(NullLogger.Instance);
        string[] modules = env.GetModules();
        Assert.Contains("MAIN", modules);
    }
}
