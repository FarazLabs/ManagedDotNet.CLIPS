namespace ManagedDotNet.Clips.Api.Models.Values;

public class ClipsInstanceAddressValue : ClipsValue
{
    public override ClipsValueType Type => ClipsValueType.INSTANCE_ADDRESS;

    public string Display { get; }

    public override string ToString() => Display;

    internal ClipsInstanceAddressValue(string display)
    {
        Display = display;
    }
}
