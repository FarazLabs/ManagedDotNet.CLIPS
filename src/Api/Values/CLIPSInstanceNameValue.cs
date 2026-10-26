using CLIPSNET;

namespace ManagedDotNet.CLIPS.Api.Values;
public class CLIPSInstanceNameValue : CLIPSValue
{
    public override CLIPSValueType Type { get; protected set; } = CLIPSValueType.INSTANCE_NAME;

    internal InstanceNameValue Value => (InstanceNameValue) PrimitiveValue;

    internal CLIPSInstanceNameValue(PrimitiveValue primitiveValue) : base(primitiveValue)
    {
        // Type validation happens in base constructor via ValidateType()
    }

    protected override bool ValidateType(out string? error)
    {
        if (PrimitiveValue is not InstanceNameValue)
        {
            error = $"{PrimitiveValue} is not a CLIPS instance name value";
            return false;
        }
        error = null;
        return true;
    }
}
