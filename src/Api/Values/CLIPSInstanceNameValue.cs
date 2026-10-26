namespace ManagedDotNet.CLIPS.Api.Values;

public class CLIPSInstanceNameValue : CLIPSValue
{
    public override CLIPSValueType Type => CLIPSValueType.INSTANCE_NAME;

    public string Value { get; }

    public override string ToString() => Value;

    internal CLIPSInstanceNameValue(string value)
    {
        Value = value;
    }
}
