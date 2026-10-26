using ManagedDotNet.CLIPS.Api.Values;

namespace ManagedDotNet.CLIPS.Api;

public abstract class CLIPSValue
{
    public abstract CLIPSValueType Type { get; }

    public CLIPSMultifieldValue AsMultifieldValue()
    {
        if (IsMultifield)
        {
            return (CLIPSMultifieldValue)this;
        }

        throw new InvalidOperationException($"Failed to cast clips value object with type {Type} to a multifield value");
    }

    public CLIPSStringValue AsStringValue()
    {
        if (IsString)
        {
            return (CLIPSStringValue)this;
        }

        throw new InvalidOperationException($"Failed to cast clips value object with type {Type} to a string value");
    }

    public CLIPSIntegerValue AsIntegerValue()
    {
        if (IsInteger)
        {
            return (CLIPSIntegerValue)this;
        }

        throw new InvalidOperationException($"Failed to cast clips value object with type {Type} to an integer value");
    }

    public CLIPSFloatValue AsFloatValue()
    {
        if (IsFloat)
        {
            return (CLIPSFloatValue)this;
        }

        throw new InvalidOperationException($"Failed to cast clips value object with type {Type} to a float value");
    }

    public CLIPSSymbolValue AsSymbolValue()
    {
        if (IsSymbol)
        {
            return (CLIPSSymbolValue)this;
        }

        throw new InvalidOperationException($"Failed to cast clips value object with type {Type} to a symbol value");
    }

    public CLIPSFactAddressValue AsFactAddressValue()
    {
        if (IsFactAddress)
        {
            return (CLIPSFactAddressValue)this;
        }

        throw new InvalidOperationException($"Failed to cast clips value object with type {Type} to a fact address value");
    }

    public CLIPSInstanceAddressValue AsInstanceAddressValue()
    {
        if (IsInstanceAddress)
        {
            return (CLIPSInstanceAddressValue)this;
        }

        throw new InvalidOperationException($"Failed to cast clips value object with type {Type} to an instance address value");
    }

    public CLIPSInstanceNameValue AsInstanceNameValue()
    {
        if (IsInstanceName)
        {
            return (CLIPSInstanceNameValue)this;
        }

        throw new InvalidOperationException($"Failed to cast clips value object with type {Type} to an instance name value");
    }

    public CLIPSVoidValue AsVoidValue()
    {
        if (IsVoid)
        {
            return (CLIPSVoidValue)this;
        }

        throw new InvalidOperationException($"Failed to cast clips value object with type {Type} to a void value");
    }

    public bool IsMultifield => this is CLIPSMultifieldValue;
    public bool IsString => this is CLIPSStringValue;
    public bool IsInteger => this is CLIPSIntegerValue;
    public bool IsFloat => this is CLIPSFloatValue;
    public bool IsSymbol => this is CLIPSSymbolValue;
    public bool IsFactAddress => this is CLIPSFactAddressValue;
    public bool IsInstanceAddress => this is CLIPSInstanceAddressValue;
    public bool IsInstanceName => this is CLIPSInstanceNameValue;
    public bool IsVoid => this is CLIPSVoidValue;

    public bool IsNumber => IsInteger || IsFloat;
    public bool IsLexeme => IsString || IsSymbol || IsInstanceName;
    public bool IsAddress => IsFactAddress || IsInstanceAddress;
}
