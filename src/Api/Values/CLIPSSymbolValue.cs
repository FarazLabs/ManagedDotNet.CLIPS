namespace ManagedDotNet.CLIPS.Api.Values;

public class CLIPSSymbolValue : CLIPSValue
{
    public override CLIPSValueType Type => CLIPSValueType.SYMBOL;

    public string Value { get; }

    public override string ToString() => Value;

    internal CLIPSSymbolValue(string value)
    {
        Value = value;
    }
}
