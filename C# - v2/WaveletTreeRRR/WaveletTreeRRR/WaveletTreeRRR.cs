using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections;

namespace WaveletTreeRRR
{
    class WaveletTreeRRR
    {
        private String commentLines;
        private String originalSequence;
        private ArrayList alphabet;
        private WaveletNode rootNode;
 
        public WaveletTreeRRR(string[] args)
		{
			alphabet = new ArrayList();
			rootNode = new WaveletNode();

			if (args.Length != 3)
			{
				throw new Exception("Error. Check your arguments.");
			}

			readFile(args[0]);
			Console.WriteLine(commentLines);
			Console.WriteLine("Sequence has" + originalSequence.Length + " characters");
		
			getAlphabet();
			


        }
        private void readFile(String filePath)
		{

			try
			{
				string[] allLines= System.IO.File.ReadAllLines(filePath);

				StringBuilder sb = new StringBuilder();
				StringBuilder comments = new StringBuilder();

				foreach (string line in allLines)
				{
					if (!(line.StartsWith(">") || line.StartsWith(";")))
					{
						sb.Append( line);
					}
					else
					{
						comments.Append(line); 
					}

					originalSequence = sb.ToString();
					commentLines = comments.ToString();           
				}
			}
			catch (Exception e)
			{
				throw new Exception("Error. Check the filepath.");
			}

		}

        public void getAlphabet()
		{
			if (originalSequence.Length > 0)
			{


				foreach (char c in originalSequence)
				{
					if (!alphabet.Contains(Char.ToUpper(c)))
					{
						alphabet.Add(Char.ToUpper(c));

					}
				}

				alphabet.Sort();


			}
			else
			{
				throw new Exception("Error. File has no data.");
			}
		}

		
    }

    

	class WaveletNode 
	{
		private String bitmap;

		private WaveletNode parent;
		private WaveletNode leftChild;
		private WaveletNode rightChild;
 
		public String getBitmap()
		{
			return bitmap;
		}

		public void setBitmap(String bitmap)
		{
			this.bitmap = bitmap;
		}

		public WaveletNode getLeftChild()
		{
			return leftChild;
		}

		public WaveletNode getRightChild()
		{
			return rightChild;
		}

		public void setLeftChild(WaveletNode child)
		{
			this.leftChild = child;
		}

		public void setRightChild(WaveletNode child)
		{
			this.rightChild = child;
		}

		public void setParent(WaveletNode parent)
		{
			this.parent = parent;
		}

		public WaveletNode getParent()
		{
			return this.parent;
		}
	}
}
