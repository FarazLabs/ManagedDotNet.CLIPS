using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLIPSNET;

namespace ManagedDotNet.CLIPS.Api.Values
{
    public class CLIPSVoidValue : CLIPSValue
    {
        internal VoidValue _value => (VoidValue) PrimitiveValue; 


        public override CLIPSValueType Type { get; protected set; }
        protected override bool ValidateType(out string? error)
        {
            if (PrimitiveValue is not VoidValue)
            {
                error = $"{PrimitiveValue} is not a CLIPS void value";
                return false;
            }
            error = null;
            return true;
        }

        public CLIPSVoidValue(PrimitiveValue primitiveValue) : base(primitiveValue)
        {
        }
    }
}
