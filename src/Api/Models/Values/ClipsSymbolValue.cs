namespace ManagedDotNet.Clips.Api.Models.Values;

public class ClipsSymbolValue : ClipsValue
{
    public override ClipsValueType Type => ClipsValueType.SYMBOL;

    public string Value { get; }

    public override string ToString() => Value;

    internal ClipsSymbolValue(string value)
    {
        Value = value;
    }
}
