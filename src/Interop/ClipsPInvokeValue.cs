using System.Runtime.InteropServices;

namespace ManagedDotNet.Clips.Interop;

[StructLayout(LayoutKind.Sequential)]
internal struct ClipsPInvokeValue
{
    public int type;
    public int reserved;
    public long integerValue;
    public double floatValue;
    public IntPtr stringValue;
    public long factIndex;
    public int multifieldLength;
    public int reserved2;
    public IntPtr multifieldItems;
}

internal enum ClipsPInvokeType : int
{
    Float = 0,
    Integer = 1,
    Symbol = 2,
    String = 3,
    Multifield = 4,
    ExternalAddress = 5,
    FactAddress = 6,
    InstanceAddress = 7,
    InstanceName = 8,
    Void = 9,
    Unknown = 255
}
