namespace ManagedDotNet.Clips.Api.Values;

public class ClipsInstanceNameValue : ClipsValue
{
    public override ClipsValueType Type => ClipsValueType.INSTANCE_NAME;

    public string Value { get; }

    public override string ToString() => Value;

    internal ClipsInstanceNameValue(string value)
    {
        Value = value;
    }
}
