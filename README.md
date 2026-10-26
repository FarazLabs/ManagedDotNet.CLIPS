# ManagedDotNet.CLIPS

Managed .NET 8 wrapper around **CLIPS 6.4** via the official-style C++/CLI `CLIPSNET` layer (`CLIPSCLRWrapper`).

Provides pooled rules-engine environments, typed `Eval` results, fact assert/fire, text and binary capture/restore, and ASP.NET Core DI helpers.

## Requirements

- **Windows x64** only (`RuntimeIdentifier=win-x64`)
- Visual Studio with **Desktop development with C++** (C++/CLI / CLR)
- MSVC toolset compatible with the wrapper project (v145 / current VS)
- .NET 8 SDK

Linux/macOS are not supported (C++/CLI).

## Install

```bash
dotnet add package ManagedDotNet.CLIPS
```

Or reference the project / solution after cloning this repo.

## Quick start

```csharp
using ManagedDotNet.CLIPS;
using ManagedDotNet.CLIPS.Abstractions;
using ManagedDotNet.CLIPS.Api;

services.AddManagedCLIPSPool("dialogue", sp => sp.GetRequiredService<MyConfigurer>());

// later
var pool = sp.GetRequiredKeyedService<IManagedCLIPSEnvironmentPool>("dialogue");
IManagedCLIPSEnvironment engine = pool.Rent();
try
{
    engine.Assert(myFact);
    engine.Fire();
    var result = engine.Eval<CLIPSStringValue>("(+ 1 2)");
}
finally
{
    pool.Return(engine);
}
```

Implement `IEnvironmentConfigurer` to load `.clp` files and configure each pooled environment before first use. Call `WarmUpManagedCLIPSPool` at app startup if you want to pre-create pool instances.

## Layout

| Path | Role |
|------|------|
| `src/` | Managed library (`ManagedDotNet.CLIPS.*`) |
| `native/CLIPSCLRWrapper/` | C++/CLI wrapper + vendored CLIPS 6.4 C sources (`CLIPSNET`) |

## Namespaces

| Namespace | Contents |
|-----------|----------|
| `ManagedDotNet.CLIPS` | DI extensions |
| `ManagedDotNet.CLIPS.Abstractions` | `IFact`, `IEnvironmentConfigurer` |
| `ManagedDotNet.CLIPS.Api` | Environment / pool APIs, captures, value base types |
| `ManagedDotNet.CLIPS.Api.Values` | Typed CLIPS values |
| `ManagedDotNet.CLIPS.Core` | `ManagedCLIPSEnvironment` implementation |
| `CLIPSNET` | Low-level CLR interop (native wrapper; unchanged) |

## Build

```bash
dotnet build ManagedDotNet.CLIPS.sln -c Release
```

Building the managed project also builds `CLIPSCLRWrapper` (requires MSBuild C++/CLI on Windows).

## License

MIT — see [LICENSE](LICENSE).

CLIPS itself is subject to its upstream license terms bundled with the engine sources under `native/CLIPSCLRWrapper/Source/CLIPS`.
