using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.InteropServices;

namespace WaveletTree
{
    class Program
    {
        static void Main(string[] args)
        {
            //var rrr = new RRRStruct("0010100110101101000001001010011011000101101101");
            var rrr = new RRRStruct(new BitArray(new bool[]{false,false, true,false, true, false, true, true, true, false,
                false,false, true,false, true, false, true, true, true,
                false,false, true,false, true, false, true, true, true,
                false,false, true,false, true, false, true, true, true,
                false,false, true,false, true, false, true, true, true, 
                false,false, true,false, true, false, true, true, true,
                false,false, true,false, true, false, true, true, true, 
                false,false, true,false, true, false, true, true, true,
                false,false, true,false, true, false, true, true, true, 
                false,false, true,false, true, false, true, true, true,
                false,false, true,false, true, false, true, true, true, 
                false,false, true,false, true, false, true, true, true,
                false,false, true,false, true, false, true, true, true, 
                false,false, true,false, true, false, true, true, true,
                false,false, true,false, true, false, true, true, true, 
                false,false, true,false, true, false, true, true, true,
                false,false, true,false, true, false, true, true, true, 
                false,false, true,false, true, false, true, true, true,
                false,false, true,false, true, false, true, true, true, 
                false,false, true,false, true, false, true, true, true,
                false,false, true,false, true, false, true, true, true
            }));
            var n = rrr.WordLength;
            var blocklength = rrr.BlockLength;
            var superblleng = rrr.SuperBlockLength;
            var bl = rrr.Blocks;
            var bi = rrr.BlocksInfo;
            var sb = rrr.SuperBlocks;

            for (int i = 0; i < rrr.WordLength; i++)
            {
                var tmp = rrr.CalculateRank(i);
                var tmp1 = rrr.CalculateRankDummy(i);
                if (tmp != tmp1)
                {
                    Console.Write("Failed at: ");
                    Console.WriteLine(i);
                    return;
                }
            }
            Console.WriteLine("OK");
            //Console.ReadLine();

            var tree = new Tree("Three Swedish switched witches watch three Swiss Swatch watch switches. Which Swedish switched witch watch which Swiss Swatch watch switch?");
            var rank = tree.GetRank('e', 138);

        }

        
    }
}
