namespace ManagedDotNet.Clips.Api.Values;

public class ClipsUnsupportedValue : ClipsValue
{
    public override ClipsValueType Type { get; }

    internal ClipsUnsupportedValue(ClipsValueType type)
    {
        Type = type;
    }
}
