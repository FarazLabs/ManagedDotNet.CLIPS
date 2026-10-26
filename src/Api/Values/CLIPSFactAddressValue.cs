namespace ManagedDotNet.CLIPS.Api.Values;

public class CLIPSFactAddressValue : CLIPSValue
{
    public override CLIPSValueType Type => CLIPSValueType.FACT_ADDRESS;

    public long FactIndex { get; }

    internal CLIPSFactAddressValue(long factIndex)
    {
        FactIndex = factIndex;
    }
}
