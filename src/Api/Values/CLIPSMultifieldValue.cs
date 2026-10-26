namespace ManagedDotNet.CLIPS.Api.Values;

public class CLIPSMultifieldValue : CLIPSValue, IEnumerable<CLIPSValue>
{
    private readonly IReadOnlyList<CLIPSValue> _items;

    public override CLIPSValueType Type => CLIPSValueType.MULTIFIELD;

    public int Count => _items.Count;

    public bool IsEmpty => Count == 0;

    public CLIPSValue this[int index]
    {
        get
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, $"Index {index} is out of range [0, {Count}).");
            }

            return _items[index];
        }
    }

    internal CLIPSMultifieldValue(IReadOnlyList<CLIPSValue> items)
    {
        _items = items;
    }

    public IEnumerator<CLIPSValue> GetEnumerator() => _items.GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

    public List<CLIPSValue> AsList() => _items.ToList();
}
