using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLIPSNET;

namespace ManagedDotNet.CLIPS.Api.Values
{
    public class CLIPSUnsupportedValue : CLIPSValue
    {
        public override CLIPSValueType Type { get; protected set; }

        protected override bool ValidateType(out string error)
        {
            error = string.Empty;
            return true;
        }


        internal CLIPSUnsupportedValue(PrimitiveValue primitiveValue, CLIPSNETType type) : base(primitiveValue)
        {
            Type = type switch
            {
                CLIPSNETType.UNKNOWN => CLIPSValueType.UNKNOWN,
                CLIPSNETType.EXTERNAL_ADDRESS => CLIPSValueType.EXTERNAL_ADDRESS,
                _ => throw new NotSupportedException(
                    $"{nameof(CLIPSUnsupportedValue)} cannot be instantiated for internal CLIPS type {type.ToString()}")
            };
            PrimitiveValue = primitiveValue;
        }
    }
}
