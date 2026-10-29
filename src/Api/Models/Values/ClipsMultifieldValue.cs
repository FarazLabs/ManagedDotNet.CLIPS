namespace ManagedDotNet.Clips.Api.Models.Values;

public class ClipsMultifieldValue : ClipsValue, IEnumerable<ClipsValue>
{
    private readonly IReadOnlyList<ClipsValue> _items;

    public override ClipsValueType Type => ClipsValueType.MULTIFIELD;

    public int Count => _items.Count;

    public bool IsEmpty => Count == 0;

    public ClipsValue this[int index]
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

    internal ClipsMultifieldValue(IReadOnlyList<ClipsValue> items)
    {
        _items = items;
    }

    public IEnumerator<ClipsValue> GetEnumerator() => _items.GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

    public List<ClipsValue> AsList() => _items.ToList();
}
