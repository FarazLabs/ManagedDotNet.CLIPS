namespace ManagedDotNet.CLIPS.Api.Values;

public class CLIPSVoidValue : CLIPSValue
{
    public override CLIPSValueType Type => CLIPSValueType.VOID;

    internal CLIPSVoidValue()
    {
    }
}
