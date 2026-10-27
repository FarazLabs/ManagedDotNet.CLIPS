namespace ManagedDotNet.Clips.Api.Values;

public class ClipsFactAddressValue : ClipsValue
{
    public override ClipsValueType Type => ClipsValueType.FACT_ADDRESS;

    public long FactIndex { get; }

    internal ClipsFactAddressValue(long factIndex)
    {
        FactIndex = factIndex;
    }
}
