using CLIPSNET;

namespace ManagedDotNet.CLIPS.Api.Values;
public class CLIPSIntegerValue : CLIPSValue
{
    public override CLIPSValueType Type { get; protected set; } = CLIPSValueType.INTEGER;

    internal IntegerValue Value => (IntegerValue) PrimitiveValue;

    internal CLIPSIntegerValue(PrimitiveValue primitiveValue) : base(primitiveValue)
    {
        // Type validation happens in base constructor via ValidateType()
    }

    protected override bool ValidateType(out string? error)
    {
        if (PrimitiveValue is not IntegerValue)
        {
            error = $"{PrimitiveValue} is not a CLIPS integer value";
            return false;
        }
        error = null;
        return true;
    }
}
