# ManagedDotNet.Clips

Managed .NET 8 wrapper around **CLIPS 6.4** via **P/Invoke** (`libclips.so` / `clips.dll`).

Provides pooled rules-engine environments, typed `Eval` results, fact assert/fire, text and binary capture/restore, and ASP.NET Core DI helpers.

## Requirements

- .NET 8 SDK
- Native shared library built from `native/clips` (see below)
- **linux-x64** or **win-x64**

## Install

Published to **GitHub Packages** on every push to `main` (package includes both `linux-x64` and `win-x64` natives).

Add a `nuget.config` (or equivalent) pointing at the FarazLabs feed:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="github" value="https://nuget.pkg.github.com/FarazLabs/index.json" />
  </packageSources>
  <packageSourceCredentials>
    <github>
      <add key="Username" value="YOUR_GITHUB_USERNAME" />
      <add key="ClearTextPassword" value="YOUR_GITHUB_PAT" />
    </github>
  </packageSourceCredentials>
</configuration>
```

PAT needs at least `read:packages`. Then:

```bash
dotnet add package ManagedDotNet.Clips
```

## Quick start

```csharp
using ManagedDotNet.Clips;
using ManagedDotNet.Clips.Abstractions;
using ManagedDotNet.Clips.Api;
using ManagedDotNet.Clips.Api.Values;

services.AddClipsPool("dialogue", sp => sp.GetRequiredService<MyConfigurer>());

var pool = sp.GetRequiredKeyedService<IClipsPool>("dialogue");
IClips engine = pool.Rent();
try
{
    engine.Assert(myFact);
    engine.Fire();
    var result = engine.Eval<ClipsIntegerValue>("(+ 1 2)");
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
dotnet build ManagedDotNet.Clips.sln -c Release
dotnet test ManagedDotNet.Clips.sln -c Release
```

## Layout

| Path | Role |
|------|------|
| `src/` | Managed library (`ManagedDotNet.Clips.*`) + P/Invoke interop |
| `native/clips/` | CMake/Makefile + P/Invoke shim |
| `native/CLIPSCLRWrapper/Source/CLIPS/` | Vendored CLIPS 6.4 C sources |
| `tests/` | Smoke tests |

## Namespaces

| Namespace | Contents |
|-----------|----------|
| `ManagedDotNet.Clips` | DI extensions |
| `ManagedDotNet.Clips.Abstractions` | `IFact`, `IClipsConfigurer` |
| `ManagedDotNet.Clips.Api` | Environment / pool APIs, captures, value base types |
| `ManagedDotNet.Clips.Api.Values` | Typed CLIPS values |
| `ManagedDotNet.Clips.Core` | `Clips` implementation (qualify as `Core.Clips` from the root namespace) |
| `ManagedDotNet.Clips.Interop` | P/Invoke declarations (internal) |

## License

MIT — see [LICENSE](LICENSE).

CLIPS itself is subject to its upstream license terms under `native/CLIPSCLRWrapper/Source/CLIPS`.
