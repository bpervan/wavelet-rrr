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
            //paring arguments
            string fileName = args[0];
            string opertation = args[1];
            char inputChar = args[2][0];
            int position = Convert.ToInt32(args[3]);


            //parsing input text (comments and newlines)
            string text;
            text = System.IO.File.ReadAllText(fileName);
            text = text.Substring(text.IndexOf(System.Environment.NewLine) + System.Environment.NewLine.Count());
            var replaceWith = "";
            string newString = text.Replace("\r\n", replaceWith).Replace("\n", replaceWith).Replace("\r", replaceWith);

            System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
            System.Diagnostics.Stopwatch st1 = new System.Diagnostics.Stopwatch();

            //creating WaveletTree and calling rank/select operations
            st.Start();
            var tree = new Tree(newString);
            st.Stop();
            Console.Write("Tree Creation: ");
            Console.WriteLine(st.ElapsedMilliseconds);
            var list = newString.ToList();
            if (opertation == "rank")
            {
                int rankDummy = 0;
                st1.Start();
                Console.WriteLine(tree.GetRank(inputChar, position));
                st1.Stop();
                Console.Write("Rank calculating: ");
                Console.WriteLine(st1.ElapsedMilliseconds);
            }
            if (opertation == "select")
            {
                int selectDummy = 0;
                int sum = 0;
                st1.Start();
                Console.Write("Result: ");
                Console.WriteLine(tree.GetSelect(inputChar, position));
                st1.Stop();
                Console.Write("Select calculating: ");
                Console.WriteLine(st1.ElapsedMilliseconds);
                
            }
            

        }

        
    }
}
