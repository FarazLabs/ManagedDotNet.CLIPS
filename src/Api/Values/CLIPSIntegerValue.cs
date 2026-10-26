namespace ManagedDotNet.CLIPS.Api.Values;

public class CLIPSIntegerValue : CLIPSValue
{
    public override CLIPSValueType Type => CLIPSValueType.INTEGER;

    public long Value { get; }

    internal CLIPSIntegerValue(long value)
    {
        Value = value;
    }
}
