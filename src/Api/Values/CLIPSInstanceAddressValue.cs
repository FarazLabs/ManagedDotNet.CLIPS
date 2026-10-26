namespace ManagedDotNet.CLIPS.Api.Values;

public class CLIPSInstanceAddressValue : CLIPSValue
{
    public override CLIPSValueType Type => CLIPSValueType.INSTANCE_ADDRESS;

    public string Display { get; }

    public override string ToString() => Display;

    internal CLIPSInstanceAddressValue(string display)
    {
        Display = display;
    }
}
