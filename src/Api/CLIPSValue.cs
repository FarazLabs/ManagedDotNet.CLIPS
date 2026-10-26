using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLIPSNET;
using ManagedDotNet.CLIPS.Api.Values;

namespace ManagedDotNet.CLIPS.Api;
public abstract class CLIPSValue
{
    protected PrimitiveValue PrimitiveValue { get; init; }

    internal CLIPSValue(PrimitiveValue primitiveValue)
    {
        PrimitiveValue = primitiveValue;
        bool validated = ValidateType(out var error);
        if (!validated)
            throw new InvalidOperationException(error);
    }

    public abstract CLIPSValueType Type { get; protected set; }

    protected abstract bool ValidateType(out string? error); 

    public CLIPSMultifieldValue AsMultifieldValue()
    {
        if (IsMultifield)
        {
            return (CLIPSMultifieldValue)this;
        }

        throw new InvalidOperationException($"Failed to cast clips value object with type {this.Type} to a multifield value");
    }

    public CLIPSStringValue AsStringValue()
    {
        if (IsString)
        {
            return (CLIPSStringValue)this;
        }

        throw new InvalidOperationException($"Failed to cast clips value object with type {this.Type} to a string value");
    }

    public CLIPSIntegerValue AsIntegerValue()
    {
        if (IsInteger)
        {
            return (CLIPSIntegerValue)this;
        }

        throw new InvalidOperationException($"Failed to cast clips value object with type {this.Type} to an integer value");
    }

    public CLIPSFloatValue AsFloatValue()
    {
        if (IsFloat)
        {
            return (CLIPSFloatValue)this;
        }

        throw new InvalidOperationException($"Failed to cast clips value object with type {this.Type} to a float value");
    }

    public CLIPSSymbolValue AsSymbolValue()
    {
        if (IsSymbol)
        {
            return (CLIPSSymbolValue)this;
        }

        throw new InvalidOperationException($"Failed to cast clips value object with type {this.Type} to a symbol value");
    }

    public CLIPSFactAddressValue AsFactAddressValue()
    {
        if (IsFactAddress)
        {
            return (CLIPSFactAddressValue)this;
        }

        throw new InvalidOperationException($"Failed to cast clips value object with type {this.Type} to a fact address value");
    }

    public CLIPSInstanceAddressValue AsInstanceAddressValue()
    {
        if (IsInstanceAddress)
        {
            return (CLIPSInstanceAddressValue)this;
        }

        throw new InvalidOperationException($"Failed to cast clips value object with type {this.Type} to an instance address value");
    }

    public CLIPSInstanceNameValue AsInstanceNameValue()
    {
        if (IsInstanceName)
        {
            return (CLIPSInstanceNameValue)this;
        }

        throw new InvalidOperationException($"Failed to cast clips value object with type {this.Type} to an instance name value");
    }

    public CLIPSVoidValue AsVoidValue()
    {
        if (IsVoid)
        {
            return (CLIPSVoidValue)this;
        }

        throw new InvalidOperationException($"Failed to cast clips value object with type {this.Type} to a void value");
    }

    // Type checking properties using 'this is' pattern
    public bool IsMultifield => this is CLIPSMultifieldValue;
    public bool IsString => this is CLIPSStringValue;
    public bool IsInteger => this is CLIPSIntegerValue;
    public bool IsFloat => this is CLIPSFloatValue;
    public bool IsSymbol => this is CLIPSSymbolValue;
    public bool IsFactAddress => this is CLIPSFactAddressValue;
    public bool IsInstanceAddress => this is CLIPSInstanceAddressValue;
    public bool IsInstanceName => this is CLIPSInstanceNameValue;
    public bool IsVoid => this is CLIPSVoidValue;
    
    // Convenience properties for common groupings
    public bool IsNumber => IsInteger || IsFloat;
    public bool IsLexeme => IsString || IsSymbol || IsInstanceName;
    public bool IsAddress => IsFactAddress || IsInstanceAddress;
}