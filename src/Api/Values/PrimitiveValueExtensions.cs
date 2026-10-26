using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLIPSNET;

namespace ManagedDotNet.CLIPS.Api.Values;
public static class PrimitiveValueExtensions
{
    public static CLIPSValue ToCLIPSValue(this PrimitiveValue value)
    {
        return value.CLIPSType() switch
        {
            CLIPSNETType.MULTIFIELD => new CLIPSMultifieldValue(value),
            CLIPSNETType.FACT_ADDRESS => new CLIPSFactAddressValue(value),
            CLIPSNETType.INSTANCE_ADDRESS => new CLIPSInstanceAddressValue(value),
            CLIPSNETType.INSTANCE_NAME => new CLIPSInstanceNameValue(value),
            CLIPSNETType.SYMBOL => new CLIPSSymbolValue(value),
            CLIPSNETType.INTEGER => new CLIPSIntegerValue(value),
            CLIPSNETType.STRING => new CLIPSStringValue(value),
            CLIPSNETType.FLOAT => new CLIPSFloatValue(value),
            CLIPSNETType.VOID => new CLIPSVoidValue(value),

            CLIPSNETType.EXTERNAL_ADDRESS => new CLIPSUnsupportedValue(value, CLIPSNETType.EXTERNAL_ADDRESS),
            CLIPSNETType.UNKNOWN => new CLIPSUnsupportedValue(value, CLIPSNETType.UNKNOWN),

            _ => throw new ArgumentException($"Unsupported CLIPS type: {value.CLIPSType()}")
        };
    }

}
