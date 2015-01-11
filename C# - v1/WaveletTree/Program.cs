using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveletTree
{
    class Program
    {
        static void Main(string[] args)
        {
            var rrr = new RRRStruct("001010011010110100000100101001101100010110110");
            var n = rrr.WordLength;
            var blocklength = rrr.BlockLength;
            var superblleng = rrr.SuperBlockLength;
            var bl = rrr.Blocks;
        }
    }
}
