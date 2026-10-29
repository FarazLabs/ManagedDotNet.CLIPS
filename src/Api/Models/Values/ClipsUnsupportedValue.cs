namespace ManagedDotNet.Clips.Api.Models.Values;

public class ClipsUnsupportedValue : ClipsValue
{
    public override ClipsValueType Type { get; }

    internal ClipsUnsupportedValue(ClipsValueType type)
    {
        Type = type;
    }
}
