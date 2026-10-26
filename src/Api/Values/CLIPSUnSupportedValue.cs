namespace ManagedDotNet.CLIPS.Api.Values;

public class CLIPSUnsupportedValue : CLIPSValue
{
    public override CLIPSValueType Type { get; }

    internal CLIPSUnsupportedValue(CLIPSValueType type)
    {
        Type = type;
    }
}
