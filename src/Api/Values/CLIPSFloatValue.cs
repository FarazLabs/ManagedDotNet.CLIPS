namespace ManagedDotNet.CLIPS.Api.Values;

public class CLIPSFloatValue : CLIPSValue
{
    public override CLIPSValueType Type => CLIPSValueType.FLOAT;

    public double Value { get; }

    internal CLIPSFloatValue(double value)
    {
        Value = value;
    }
}
