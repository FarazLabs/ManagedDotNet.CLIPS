using CLIPSNET;

namespace ManagedDotNet.CLIPS.Api.Values;
public class CLIPSInstanceAddressValue : CLIPSValue
{
    public override CLIPSValueType Type { get; protected set; } = CLIPSValueType.INSTANCE_ADDRESS;

    internal InstanceAddressValue Value => (InstanceAddressValue) PrimitiveValue;

    internal CLIPSInstanceAddressValue(PrimitiveValue primitiveValue) : base(primitiveValue)
    {
        // Type validation happens in base constructor via ValidateType()
    }

    protected override bool ValidateType(out string? error)
    {
        if (PrimitiveValue is not InstanceAddressValue)
        {
            error = $"{PrimitiveValue} is not a CLIPS instance address";
            return false;
        }
        error = null;
        return true;
    }
}
