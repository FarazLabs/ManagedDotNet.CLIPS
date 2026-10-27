using ManagedDotNet.Clips.Api.Values;

namespace ManagedDotNet.Clips.Api;

public abstract class ClipsValue
{
    public abstract ClipsValueType Type { get; }

    public ClipsMultifieldValue AsMultifieldValue()
    {
        if (IsMultifield)
        {
            return (ClipsMultifieldValue)this;
        }

        throw new InvalidOperationException($"Failed to cast clips value object with type {Type} to a multifield value");
    }

    public ClipsStringValue AsStringValue()
    {
        if (IsString)
        {
            return (ClipsStringValue)this;
        }

        throw new InvalidOperationException($"Failed to cast clips value object with type {Type} to a string value");
    }

    public ClipsIntegerValue AsIntegerValue()
    {
        if (IsInteger)
        {
            return (ClipsIntegerValue)this;
        }

        throw new InvalidOperationException($"Failed to cast clips value object with type {Type} to an integer value");
    }

    public ClipsFloatValue AsFloatValue()
    {
        if (IsFloat)
        {
            return (ClipsFloatValue)this;
        }

        throw new InvalidOperationException($"Failed to cast clips value object with type {Type} to a float value");
    }

    public ClipsSymbolValue AsSymbolValue()
    {
        if (IsSymbol)
        {
            return (ClipsSymbolValue)this;
        }

        throw new InvalidOperationException($"Failed to cast clips value object with type {Type} to a symbol value");
    }

    public ClipsFactAddressValue AsFactAddressValue()
    {
        if (IsFactAddress)
        {
            return (ClipsFactAddressValue)this;
        }

        throw new InvalidOperationException($"Failed to cast clips value object with type {Type} to a fact address value");
    }

    public ClipsInstanceAddressValue AsInstanceAddressValue()
    {
        if (IsInstanceAddress)
        {
            return (ClipsInstanceAddressValue)this;
        }

        throw new InvalidOperationException($"Failed to cast clips value object with type {Type} to an instance address value");
    }

    public ClipsInstanceNameValue AsInstanceNameValue()
    {
        if (IsInstanceName)
        {
            return (ClipsInstanceNameValue)this;
        }

        throw new InvalidOperationException($"Failed to cast clips value object with type {Type} to an instance name value");
    }

    public ClipsVoidValue AsVoidValue()
    {
        if (IsVoid)
        {
            return (ClipsVoidValue)this;
        }

        throw new InvalidOperationException($"Failed to cast clips value object with type {Type} to a void value");
    }

    public bool IsMultifield => this is ClipsMultifieldValue;
    public bool IsString => this is ClipsStringValue;
    public bool IsInteger => this is ClipsIntegerValue;
    public bool IsFloat => this is ClipsFloatValue;
    public bool IsSymbol => this is ClipsSymbolValue;
    public bool IsFactAddress => this is ClipsFactAddressValue;
    public bool IsInstanceAddress => this is ClipsInstanceAddressValue;
    public bool IsInstanceName => this is ClipsInstanceNameValue;
    public bool IsVoid => this is ClipsVoidValue;

    public bool IsNumber => IsInteger || IsFloat;
    public bool IsLexeme => IsString || IsSymbol || IsInstanceName;
    public bool IsAddress => IsFactAddress || IsInstanceAddress;
}
