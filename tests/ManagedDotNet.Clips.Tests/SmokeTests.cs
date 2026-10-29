using ManagedDotNet.Clips.Api.Models.Values;
using ManagedDotNet.Clips.Core;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ManagedDotNet.Clips.Tests;

public class SmokeTests
{
    [Fact]
    public void Eval_AddsIntegers()
    {
        using ClipsEnvironment env = new ClipsEnvironment(NullLogger.Instance);
        ClipsIntegerValue result = env.Eval<ClipsIntegerValue>("(+ 1 2)");
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public void Reset_Succeeds()
    {
        using ClipsEnvironment env = new ClipsEnvironment(NullLogger.Instance);
        Assert.True(env.Reset());
    }

    [Fact]
    public void GetModules_IncludesMain()
    {
        using ClipsEnvironment env = new ClipsEnvironment(NullLogger.Instance);
        string[] modules = env.GetModules();
        Assert.Contains("MAIN", modules);
    }
}
