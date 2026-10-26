using CLIPSNET;

namespace ManagedDotNet.CLIPS.Api.Values;

/// <summary>
/// CLIPS Multifield value that can hold items of any CLIPSValue type.
/// </summary>
public class CLIPSMultifieldValue : CLIPSValue, IEnumerable<CLIPSValue>
{
    public override CLIPSValueType Type { get; protected set; } = CLIPSValueType.MULTIFIELD;

    private MultifieldValue Value => (MultifieldValue) PrimitiveValue;

    public int Count => Value.Count;
    
    public bool IsEmpty => Count == 0;

    public CLIPSValue this[int index]
    {
        get
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(
                    nameof(index),
                    index,
                    $"Index {index} is out of range [0, {Count})."
                );

            PrimitiveValue? primitiveValue = Value[index];
            
            if (primitiveValue == null)
                throw new InvalidOperationException($"Primitive value at index {index} is null");

            // Convert the primitive value to appropriate a CLIPSValue type
            CLIPSValue value = primitiveValue.ToCLIPSValue();
            return value;
        }
    }

    public IEnumerator<CLIPSValue> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
        {
            yield return this[i];
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public List<CLIPSValue> AsList()
    {
        var list = new List<CLIPSValue>();
        for (int i = 0; i < Count; i++)
        {
            list.Add(this[i]);
        }
        return list;
    }

    internal CLIPSMultifieldValue(PrimitiveValue primitiveValue) : base(primitiveValue) {}

    protected override bool ValidateType(out string? error)
    {
        if (PrimitiveValue is not MultifieldValue)
        {
            error = $"{PrimitiveValue} is not a CLIPS multifield";
            return false;
        }
        error = null;
        return true;
    }
}