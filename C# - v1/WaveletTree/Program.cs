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
            //string fileName = args[0];
            //char inputChar = args[1][0];
            //int position = Convert.ToInt32(args[2]);
            int position = 10;
            char inputChar = 'G';

            //Console.WriteLine(fileName);
            //Console.WriteLine(inputChar);
            //Console.WriteLine(position);

            //todo better fasta recoignition

            bool fasta = false;
            //if (fileName.Split('.')[1] == "fa" || fileName.Split('.')[1] == "fas")
            //    fasta = true;

            string text;
            text = System.IO.File.ReadAllText("input.fa");
            text = text.Substring(text.IndexOf(System.Environment.NewLine) + System.Environment.NewLine.Count());
            //if (fasta)
            //{
            //    text = System.IO.File.ReadAllText(fileName);
            //    text = text.Substring(text.IndexOf(System.Environment.NewLine) + 2);
            //    //Console.WriteLine(text.IndexOf(System.Environment.NewLine));
            //}
            //else
            //{
            //    text = System.IO.File.ReadAllText(fileName);
            //}
            //Console.WriteLine(text);
            var z = text.Count();

            var replaceWith = "";
            string newString = text.Replace("\r\n", replaceWith).Replace("\n", replaceWith).Replace("\r", replaceWith);
            //string newString = text.Replace(System.Environment.NewLine, "");
            var x = newString.Count();

            //Console.WriteLine(newString.Count());

            System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
            System.Diagnostics.Stopwatch st1 = new System.Diagnostics.Stopwatch();




            
           
            var tree = new Tree(newString);
            var list = newString.ToList();
            st.Start();
            //var rrr = new RRRStruct(list.Select(t => t == '1').ToList());
            //var xx = rrr.CalculateSelectZero(400);
            var gg = tree.GetSelect(inputChar,position);
            //var rank = tree.GetRank(inputChar, position);
            st.Stop();
            var elap = st.ElapsedMilliseconds;
            var rankDummy = 0;
            st1.Start();            
            for (int i = 0; i <= position; i++ )
            {
                if (newString[i] == inputChar)
                    rankDummy++;
            }
            st1.Stop();
            var el = st1.ElapsedMilliseconds;
            //Console.WriteLine(rank);
            Console.Write(rankDummy);
            Console.ReadLine();
            

        }

        
    }
}
