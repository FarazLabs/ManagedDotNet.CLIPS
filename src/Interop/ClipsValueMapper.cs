using System.Runtime.InteropServices;
using ManagedDotNet.CLIPS.Api;
using ManagedDotNet.CLIPS.Api.Values;

namespace ManagedDotNet.CLIPS.Interop;

internal static class ClipsValueMapper
{
    public static CLIPSValue ToManaged(ref ClipsPInvokeValue native)
    {
        return (ClipsPInvokeType)native.type switch
        {
            ClipsPInvokeType.Float => new CLIPSFloatValue(native.floatValue),
            ClipsPInvokeType.Integer => new CLIPSIntegerValue(native.integerValue),
            ClipsPInvokeType.Symbol => new CLIPSSymbolValue(ReadUtf8(native.stringValue) ?? string.Empty),
            ClipsPInvokeType.String => new CLIPSStringValue(ReadUtf8(native.stringValue) ?? string.Empty),
            ClipsPInvokeType.InstanceName => new CLIPSInstanceNameValue(ReadUtf8(native.stringValue) ?? string.Empty),
            ClipsPInvokeType.Multifield => MapMultifield(ref native),
            ClipsPInvokeType.FactAddress => new CLIPSFactAddressValue(native.factIndex),
            ClipsPInvokeType.InstanceAddress => new CLIPSInstanceAddressValue(ReadUtf8(native.stringValue) ?? string.Empty),
            ClipsPInvokeType.Void => new CLIPSVoidValue(),
            ClipsPInvokeType.ExternalAddress => new CLIPSUnsupportedValue(CLIPSValueType.EXTERNAL_ADDRESS),
            _ => new CLIPSUnsupportedValue(CLIPSValueType.UNKNOWN),
        };
    }

    private static CLIPSMultifieldValue MapMultifield(ref ClipsPInvokeValue native)
    {
        var items = new List<CLIPSValue>(native.multifieldLength);
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

        return new CLIPSMultifieldValue(items);
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
