using CLIPSNET;

namespace ManagedDotNet.CLIPS.Api.Values;
public class CLIPSFactAddressValue : CLIPSValue
{
    public override CLIPSValueType Type { get; protected set; } = CLIPSValueType.FACT_ADDRESS;

    internal FactAddressValue Value => (FactAddressValue) PrimitiveValue;

    internal CLIPSFactAddressValue(PrimitiveValue primitiveValue) : base(primitiveValue)
    {
        // Type validation happens in base constructor via ValidateType()
    }

    protected override bool ValidateType(out string? error)
    {
        if (PrimitiveValue is not FactAddressValue)
        {
            error = $"{PrimitiveValue} is not a CLIPS fact address";
            return false;
        }
        error = null;
        return true;
    }
}