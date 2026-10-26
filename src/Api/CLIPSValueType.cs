using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedDotNet.CLIPS.Api
{
    public enum CLIPSValueType
    {
        UNKNOWN,
        FLOAT,
        INTEGER,
        SYMBOL,
        STRING,
        MULTIFIELD,
        EXTERNAL_ADDRESS,
        FACT_ADDRESS,
        INSTANCE_ADDRESS,
        INSTANCE_NAME,
        VOID
    }
}
