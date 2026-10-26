using CLIPSNET;

namespace ManagedDotNet.CLIPS.Api.Values;
public class CLIPSFloatValue : CLIPSValue
{
 public override CLIPSValueType Type { get; protected set; } = CLIPSValueType.FLOAT;

 internal FloatValue Value => (FloatValue) PrimitiveValue;

 internal CLIPSFloatValue(PrimitiveValue primitiveValue) : base(primitiveValue)
 {
 // Type validation happens in base constructor via ValidateType()
 }

 protected override bool ValidateType(out string? error)
 {
 if (PrimitiveValue is not FloatValue)
 {
 error = $"{PrimitiveValue} is not a CLIPS float value";
 return false;
 }
 error = null;
 return true;
 }
}
