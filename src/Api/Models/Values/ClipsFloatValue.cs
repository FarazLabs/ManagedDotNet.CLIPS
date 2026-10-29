namespace ManagedDotNet.Clips.Api.Models.Values;

public class ClipsFloatValue : ClipsValue
{
    public override ClipsValueType Type => ClipsValueType.FLOAT;

    public double Value { get; }

    internal ClipsFloatValue(double value)
    {
        Value = value;
    }
}
