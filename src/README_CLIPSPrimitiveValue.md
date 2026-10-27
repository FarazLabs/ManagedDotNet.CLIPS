# CLIPSPrimitiveValue Wrapper

The `CLIPSPrimitiveValue` class is a comprehensive wrapper around the original CLIPSNET `PrimitiveValue` that hides the underlying implementation details and provides a clean, type-safe interface for working with all CLIPS value types.

## Purpose

This wrapper was introduced to:
- Hide the original CLIPS `PrimitiveValue` implementation from consumers
- Provide a complete, consistent API for all CLIPS value types
- Support all CLIPS types: basic types, addresses, and collections
- Maintain type safety while abstracting away CLIPS-specific details
- Enable easier testing and mocking

## Supported CLIPS Types

### Basic Types
- **Integer**: Whole numbers
- **Float**: Decimal numbers  
- **String**: Text values
- **Symbol**: CLIPS symbols
- **InstanceName**: Object instance names
- **Void**: Null/empty values

### Address Types
- **FactAddress**: References to facts in working memory
- **InstanceAddress**: References to object instances
- **ExternalAddress**: Custom external data types

### Collection Types
- **Multifield**: Lists/arrays of values

## Key Features

### Complete Type Checking
```csharp
var result = environment.Eval("(+ 1 2)");
Console.WriteLine($"Is Number: {result.IsNumber}");
Console.WriteLine($"Is Integer: {result.IsInteger}");
Console.WriteLine($"Is String: {result.IsString}");
Console.WriteLine($"Is Multifield: {result.IsMultifield}");
Console.WriteLine($"Is Address: {result.IsAddress}");
Console.WriteLine($"Is Lexeme: {result.IsLexeme}");
```

### Type Conversion
```csharp
var result = environment.Eval("(+ 1 2)");
int intValue = result.AsInteger();
double doubleValue = result.AsDouble();
string stringValue = result.AsString();
bool boolValue = result.AsBoolean();
string symbolValue = result.AsSymbol(); // For symbols and instance names
```

### Safe Conversion with Defaults
```csharp
int safeInt = result.AsIntegerOrDefault(-1);
double safeDouble = result.AsDoubleOrDefault(0.0);
bool safeBool = result.AsBooleanOrDefault(false);
```

### Collection Handling
```csharp
var listResult = environment.Eval("(create$ 1 2 3)");
if (listResult.IsMultifield)
{
    Console.WriteLine($"Count: {listResult.Count}");
    
    // Access by index
    var firstItem = listResult[0];
    
    // Iterate through items
    foreach (var item in listResult.AsList())
    {
        Console.WriteLine($"Item: {item}");
    }
}
```

### Fact Address Support
```csharp
var factList = environment.Eval("(get-fact-list)");
foreach (var fact in factList.AsList())
{
    if (fact.IsFactAddress)
    {
        long index = fact.GetFactIndex();
        string factString = fact.GetFactString();
        Console.WriteLine($"Fact {index}: {factString}");
    }
}
```

### Instance Address Support
```csharp
var instanceList = environment.Eval("(find-all-instances ((?ins PERSON)) TRUE)");
foreach (var instance in instanceList.AsList())
{
    if (instance.IsInstanceAddress)
    {
        string instanceName = instance.GetInstanceName();
        Console.WriteLine($"Instance: {instanceName}");
    }
}
```

## API Changes

### Before (Exposing PrimitiveValue)
```csharp
public interface IClips
{
    PrimitiveValue Eval(string expression);
}
```

### After (Using CLIPSPrimitiveValue Wrapper)
```csharp
public interface IClips
{
    CLIPSPrimitiveValue Eval(string expression);
}
```

## Benefits

1. **Encapsulation**: The original CLIPS `PrimitiveValue` is no longer exposed to consumers
2. **Type Safety**: Provides compile-time type checking and conversion methods
3. **Consistency**: Uniform API across different CLIPS value types
4. **Testability**: Easier to mock and test without CLIPS dependencies
5. **Future-Proofing**: Changes to underlying CLIPS implementation won't affect consumers

## Usage Examples

See `CLIPSPrimitiveValueExample.cs` for comprehensive usage examples demonstrating:
- Basic value operations
- Type checking and conversion
- Working with collections
- Complex expression evaluation

## Internal Access

For internal components that need access to the original CLIPS data, use the `GetRawPrimitiveValue()` method:

```csharp
// Internal use only
var rawValue = clipsValue.GetRawPrimitiveValue();
```

This method should only be used by internal components that require direct access to the original CLIPS implementation.
