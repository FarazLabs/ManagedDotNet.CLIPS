namespace ManagedDotNet.Clips.Api.Models.Values;

public class ClipsIntegerValue : ClipsValue
{
    public override ClipsValueType Type => ClipsValueType.INTEGER;

    public long Value { get; }

    internal ClipsIntegerValue(long value)
    {
        Value = value;
    }
}
