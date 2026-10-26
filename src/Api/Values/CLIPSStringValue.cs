namespace ManagedDotNet.CLIPS.Api.Values;

public class CLIPSStringValue : CLIPSValue
{
    public override CLIPSValueType Type => CLIPSValueType.STRING;

    public string Value { get; }

    public string AsString() => Value;

    internal CLIPSStringValue(string value)
    {
        Value = value;
    }
}
