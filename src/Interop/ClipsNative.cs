using System.Runtime.InteropServices;

namespace ManagedDotNet.Clips.Interop;

internal static partial class ClipsNative
{
    private const string LibraryName = "clips";

    static ClipsNative()
    {
        NativeLibrary.SetDllImportResolver(typeof(ClipsNative).Assembly, Resolve);
    }

    private static IntPtr Resolve(string libraryName, System.Reflection.Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (!string.Equals(libraryName, LibraryName, StringComparison.OrdinalIgnoreCase))
        {
            return IntPtr.Zero;
        }

        string? baseDir = AppContext.BaseDirectory;
        string[] candidates =
        [
            Path.Combine(baseDir, "runtimes", "win-x64", "native", "clips.dll"),
            Path.Combine(baseDir, "runtimes", "linux-x64", "native", "libclips.so"),
            Path.Combine(baseDir, "clips.dll"),
            Path.Combine(baseDir, "libclips.so"),
            Path.Combine(baseDir, "native", "clips.dll"),
            Path.Combine(baseDir, "native", "libclips.so"),
        ];

        foreach (string candidate in candidates)
        {
            if (File.Exists(candidate) && NativeLibrary.TryLoad(candidate, out IntPtr handle))
            {
                return handle;
            }
        }

        if (NativeLibrary.TryLoad(OperatingSystem.IsWindows() ? "clips" : "libclips", assembly, searchPath, out IntPtr fallback))
        {
            return fallback;
        }

        return IntPtr.Zero;
    }

    [LibraryImport(LibraryName)]
    internal static partial IntPtr Clips_CreateEnvironment();

    [LibraryImport(LibraryName)]
    [return: MarshalAs(UnmanagedType.I4)]
    internal static partial int Clips_DestroyEnvironment(IntPtr env);

    [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.I4)]
    internal static partial int Clips_Eval(IntPtr env, string expression, out ClipsPInvokeValue outValue);

    [LibraryImport(LibraryName)]
    internal static partial void Clips_FreeValue(ref ClipsPInvokeValue value);

    [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.I4)]
    internal static partial int Clips_Build(IntPtr env, string construct);

    [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.I4)]
    internal static partial int Clips_Load(IntPtr env, string filePath);

    [LibraryImport(LibraryName)]
    internal static partial void Clips_Reset(IntPtr env);

    [LibraryImport(LibraryName)]
    internal static partial long Clips_Run(IntPtr env, long limit);

    [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.I4)]
    internal static partial int Clips_AssertString(IntPtr env, string fact);

    [LibraryImport(LibraryName)]
    [return: MarshalAs(UnmanagedType.I4)]
    internal static partial int Clips_GetModuleNames(IntPtr env, out IntPtr namesOut, out int countOut);

    [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.I4)]
    internal static partial int Clips_GetFactPPForms(IntPtr env, string moduleName, out IntPtr formsOut, out int countOut);

    [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.I4)]
    internal static partial int Clips_GetInstancePPForms(IntPtr env, string moduleName, out IntPtr formsOut, out int countOut);

    [LibraryImport(LibraryName)]
    internal static partial void Clips_FreeStringArray(IntPtr items, int count);
}
