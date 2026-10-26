using CLIPSNET;

namespace ManagedDotNet.CLIPS.Api.Values;
public class CLIPSSymbolValue : CLIPSValue
{
    public override CLIPSValueType Type { get; protected set; } = CLIPSValueType.SYMBOL;

    internal SymbolValue Value => (SymbolValue) PrimitiveValue;

    public string AsString() => Value.ToString();

    internal CLIPSSymbolValue(PrimitiveValue primitiveValue) : base(primitiveValue)
    {
        // Type validation happens in base constructor via ValidateType()
    }

    protected override bool ValidateType(out string? error)
    {
        if (PrimitiveValue is not SymbolValue)
        {
            error = $"{PrimitiveValue} is not a CLIPS symbol";
            return false;
        }
        error = null;
        return true;
    }
}