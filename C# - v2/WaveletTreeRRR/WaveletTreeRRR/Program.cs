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
                Console.Write("\nMemory used on start od program: " + GC.GetTotalMemory(false) + "bytes\n");
                new WaveletTreeRRR(args);
                Console.Write("Memory used: " + GC.GetTotalMemory(false) + "bytes\n");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }


        }
        static int mapsize;

       /* public static void inOrder(TreeNode rootNode) 
        {
            if (rootNode == null)
                return;
            inOrder(rootNode.left);
            mapsize = mapsize + rootNode.map.size();
            inOrder(rootNode.right);
        }*/

    }
}
