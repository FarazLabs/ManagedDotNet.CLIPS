using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedDotNet.Clips.Api.Models
{
    public class BinaryCapture
    {
        public byte[] Data { get; }

        public BinaryCapture(byte[] data)
        {
            Data = data;
        }
    }
}
