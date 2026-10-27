namespace ManagedDotNet.Clips.Api.Values;

public class ClipsVoidValue : ClipsValue
{
    public override ClipsValueType Type => ClipsValueType.VOID;

    internal ClipsVoidValue()
    {
    }
}
