using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedDotNet.Clips.Api.Models.Values
{
    public enum ClipsValueType
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
