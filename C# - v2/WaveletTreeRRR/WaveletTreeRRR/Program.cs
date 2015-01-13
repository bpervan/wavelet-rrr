using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WaveletTreeRRR
{
   class Program
    {
       public static long GetBinCoeff(long N, long K)
       {
           // This function gets the total number of unique combinations based upon N and K.
           long r = 1;
           long d;
           if (K > N) return 0;
           for (d = 1; d <= K; d++)
           {
               r *= N--;
               r /= d;
           }
           return r;
       }
        static void Main(string[] args)
        {
            long nPovrhk = GetBinCoeff(5, 0);
            string[] nista={"nikaj"};
            new WaveletTreeRRR(nista);

            Console.WriteLine(Convert.ToInt32("1", 2));

        }

    }
}
