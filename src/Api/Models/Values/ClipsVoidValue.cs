namespace ManagedDotNet.Clips.Api.Models.Values;

public class ClipsVoidValue : ClipsValue
{
    public override ClipsValueType Type => ClipsValueType.VOID;

    internal ClipsVoidValue()
    {
    }
}
