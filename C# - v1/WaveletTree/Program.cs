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
            //string opertation = args[1];
            //char inputChar = args[2][0];
            //int position = Convert.ToInt32(args[3]);

            string fileName = "input1.fa";
            string opertation = "select";
            char inputChar = 'T';
            int position = 1000000;



            string text;
            text = System.IO.File.ReadAllText(fileName);
            text = text.Substring(text.IndexOf(System.Environment.NewLine) + System.Environment.NewLine.Count());


            var replaceWith = "";
            string newString = text.Replace("\r\n", replaceWith).Replace("\n", replaceWith).Replace("\r", replaceWith);

            System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
            System.Diagnostics.Stopwatch st1 = new System.Diagnostics.Stopwatch();


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
                //for (int i = 0; i <= position; i++)
                //{
                //    if (newString[i] == inputChar)
                //        rankDummy++;
                //}
                //Console.WriteLine(rankDummy);
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
                //for (int i = 0; i <= newString.Count(); i++)
                //{
                //    if (newString[i] == inputChar)
                //        sum++;
                //    if (sum == position)
                //    {
                //        Console.WriteLine(i);
                //        break;
                //    }                   

                //}
                
            }
            //var rankDummy = 0;
            //st1.Start();            
            //for (int i = 0; i <= position; i++)
            //{
            //    if (newString[i] == inputChar)
            //        rankDummy++;
            //}
            //st1.Stop();
            //var el = st1.ElapsedMilliseconds;
            ////Console.WriteLine(rank);
            //Console.Write(rankDummy);
            Console.ReadLine();
            

        }

        
    }
}
