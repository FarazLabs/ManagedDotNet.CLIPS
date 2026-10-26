using CLIPSNET;

namespace ManagedDotNet.CLIPS.Api.Values;
public class CLIPSStringValue : CLIPSValue
{
    public override CLIPSValueType Type { get; protected set; } = CLIPSValueType.STRING;

    internal StringValue Value => (StringValue) PrimitiveValue;

    public string AsString() => Value.ToString();

    internal CLIPSStringValue(PrimitiveValue primitiveValue) : base(primitiveValue)
    {
        // Type validation happens in base constructor via ValidateType()
    }

    protected override bool ValidateType(out string? error)
    {
        if (PrimitiveValue is not StringValue)
        {
            error = $"{PrimitiveValue} is not a CLIPS string";
            return false;
        }
        error = null;
        return true;
    }
}