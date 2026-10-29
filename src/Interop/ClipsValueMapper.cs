using System.Runtime.InteropServices;
using ManagedDotNet.Clips.Api.Models.Values;

namespace ManagedDotNet.Clips.Interop;

internal static class ClipsValueMapper
{
    public static ClipsValue ToManaged(ref ClipsPInvokeValue native)
    {
        return (ClipsPInvokeType)native.type switch
        {
            ClipsPInvokeType.Float => new ClipsFloatValue(native.floatValue),
            ClipsPInvokeType.Integer => new ClipsIntegerValue(native.integerValue),
            ClipsPInvokeType.Symbol => new ClipsSymbolValue(ReadUtf8(native.stringValue) ?? string.Empty),
            ClipsPInvokeType.String => new ClipsStringValue(ReadUtf8(native.stringValue) ?? string.Empty),
            ClipsPInvokeType.InstanceName => new ClipsInstanceNameValue(ReadUtf8(native.stringValue) ?? string.Empty),
            ClipsPInvokeType.Multifield => MapMultifield(ref native),
            ClipsPInvokeType.FactAddress => new ClipsFactAddressValue(native.factIndex),
            ClipsPInvokeType.InstanceAddress => new ClipsInstanceAddressValue(ReadUtf8(native.stringValue) ?? string.Empty),
            ClipsPInvokeType.Void => new ClipsVoidValue(),
            ClipsPInvokeType.ExternalAddress => new ClipsUnsupportedValue(ClipsValueType.EXTERNAL_ADDRESS),
            _ => new ClipsUnsupportedValue(ClipsValueType.UNKNOWN),
        };
    }

    private static ClipsMultifieldValue MapMultifield(ref ClipsPInvokeValue native)
    {
        var items = new List<ClipsValue>(native.multifieldLength);
        if (native.multifieldItems != IntPtr.Zero && native.multifieldLength > 0)
        {
            int size = Marshal.SizeOf<ClipsPInvokeValue>();
            for (int i = 0; i < native.multifieldLength; i++)
            {
                IntPtr itemPtr = native.multifieldItems + (i * size);
                ClipsPInvokeValue item = Marshal.PtrToStructure<ClipsPInvokeValue>(itemPtr);
                items.Add(ToManaged(ref item));
            }
        }

        return new ClipsMultifieldValue(items);
    }

    public static string[] ReadStringArray(IntPtr arrayPtr, int count)
    {
        if (arrayPtr == IntPtr.Zero || count <= 0)
        {
            return Array.Empty<string>();
        }

        var result = new string[count];
        for (int i = 0; i < count; i++)
        {
            IntPtr strPtr = Marshal.ReadIntPtr(arrayPtr, i * IntPtr.Size);
            result[i] = ReadUtf8(strPtr) ?? string.Empty;
        }

        return result;
    }

    private static string? ReadUtf8(IntPtr ptr)
    {
        return ptr == IntPtr.Zero ? null : Marshal.PtrToStringUTF8(ptr);
    }
}
