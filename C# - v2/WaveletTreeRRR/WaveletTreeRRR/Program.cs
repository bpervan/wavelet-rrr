using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WaveletTreeRRR
{
   class Program
    {
      
        static void Main(string[] args)
        {
            try
            {
               
                new WaveletTreeRRR(args);
            
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
