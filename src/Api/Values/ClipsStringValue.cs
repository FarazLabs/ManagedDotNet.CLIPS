namespace ManagedDotNet.Clips.Api.Values;

public class ClipsStringValue : ClipsValue
{
    public override ClipsValueType Type => ClipsValueType.STRING;

    public string Value { get; }

    public string AsString() => Value;

    internal ClipsStringValue(string value)
    {
        Value = value;
    }
}
