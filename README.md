# ManagedDotNet.CLIPS

Managed .NET 8 wrapper around **CLIPS 6.4** via **P/Invoke** (`libclips.so` / `clips.dll`).

Provides pooled rules-engine environments, typed `Eval` results, fact assert/fire, text and binary capture/restore, and ASP.NET Core DI helpers.

## Requirements

- .NET 8 SDK
- Native shared library built from `native/clips` (see below)
- **linux-x64** or **win-x64**

## Install

```bash
dotnet add package ManagedDotNet.CLIPS
```

The NuGet package includes RID-specific native binaries under `runtimes/{rid}/native/` when they were present at pack time.

## Quick start

```csharp
using ManagedDotNet.CLIPS;
using ManagedDotNet.CLIPS.Abstractions;
using ManagedDotNet.CLIPS.Api;
using ManagedDotNet.CLIPS.Api.Values;

services.AddManagedCLIPSPool("dialogue", sp => sp.GetRequiredService<MyConfigurer>());

var pool = sp.GetRequiredKeyedService<IManagedCLIPSEnvironmentPool>("dialogue");
IManagedCLIPSEnvironment engine = pool.Rent();
try
{
    engine.Assert(myFact);
    engine.Fire();
    var result = engine.Eval<CLIPSIntegerValue>("(+ 1 2)");
}
finally
{
    pool.Return(engine);
}
```

## Build native library

### Linux

```bash
cd native/clips
make
# produces build/native/linux-x64/libclips.so
```

Or with CMake:

```bash
cmake -S native/clips -B native/clips/build -DCMAKE_BUILD_TYPE=Release
cmake --build native/clips/build
# copy the .so into native/clips/build/native/linux-x64/libclips.so
```

### Windows

Requires CMake + MSVC (C compiler, not C++/CLI):

```bat
cmake -S native/clips -B native/clips/build -A x64
cmake --build native/clips/build --config Release
```

Place `clips.dll` at `native/clips/build/native/win-x64/clips.dll` before packing.

## Build managed

```bash
dotnet build ManagedDotNet.CLIPS.sln -c Release
dotnet test ManagedDotNet.CLIPS.sln -c Release
```

## Layout

| Path | Role |
|------|------|
| `src/` | Managed library (`ManagedDotNet.CLIPS.*`) + P/Invoke interop |
| `native/clips/` | CMake/Makefile + P/Invoke shim |
| `native/CLIPSCLRWrapper/Source/CLIPS/` | Vendored CLIPS 6.4 C sources |
| `tests/` | Smoke tests |

## Namespaces

| Namespace | Contents |
|-----------|----------|
| `ManagedDotNet.CLIPS` | DI extensions |
| `ManagedDotNet.CLIPS.Abstractions` | `IFact`, `IEnvironmentConfigurer` |
| `ManagedDotNet.CLIPS.Api` | Environment / pool APIs, captures, value base types |
| `ManagedDotNet.CLIPS.Api.Values` | Typed CLIPS values |
| `ManagedDotNet.CLIPS.Core` | `ManagedCLIPSEnvironment` implementation |
| `ManagedDotNet.CLIPS.Interop` | P/Invoke declarations (internal) |

## License

MIT — see [LICENSE](LICENSE).

CLIPS itself is subject to its upstream license terms under `native/CLIPSCLRWrapper/Source/CLIPS`.
