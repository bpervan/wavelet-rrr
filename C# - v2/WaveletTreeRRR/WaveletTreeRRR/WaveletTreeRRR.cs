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

            buildWaveletTree(alphabet, originalSequence, rootNode);


        }
        private void readFile(String filePath)
        {

            try
            {
                string[] allLines = System.IO.File.ReadAllLines(filePath);

                StringBuilder sb = new StringBuilder();
                StringBuilder comments = new StringBuilder();

                foreach (string line in allLines)
                {
                    if (!(line.StartsWith(">") || line.StartsWith(";")))
                    {
                        sb.Append(line);
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

        public void buildWaveletTree(ArrayList currentAlphabet, String currentLabel, WaveletNode currentNode)
        {
            if (currentAlphabet.Count > 2)
            {
                StringBuilder bitmapBuilder = new StringBuilder();
                StringBuilder leftLabel = new StringBuilder();
                StringBuilder rightLabel = new StringBuilder();
                int mid = (currentAlphabet.Count + 1) / 2;

                foreach (char c in currentLabel)
                {
                    if (getIndex(c, currentAlphabet) < mid)
                    {
                        bitmapBuilder.Append("0");
                        leftLabel.Append(Char.ToUpper(c));
                    }
                    else
                    {
                        bitmapBuilder.Append("1");
                        rightLabel.Append(Char.ToUpper(c));
                    }
                }


                currentNode.setBitmap(bitmapBuilder.ToString());

                currentNode.setLeftChild(new WaveletNode());
                currentNode.getLeftChild().setParent(currentNode);


                buildWaveletTree(currentAlphabet.GetRange(0, mid), leftLabel.ToString(), currentNode.getLeftChild());

                if (currentAlphabet.Count > 3)
                {
                    currentNode.setRightChild(new WaveletNode());
                    currentNode.getRightChild().setParent(currentNode);
                    buildWaveletTree(currentAlphabet.GetRange(mid, (currentAlphabet.Count - mid)),
                        rightLabel.ToString(), currentNode.getRightChild());
                }


            }
            else
            {
                if (currentAlphabet.Count == 2)
                {
                    StringBuilder bitmapBuilder = new StringBuilder();

                    foreach (char c in currentLabel)
                    {
                        if (getIndex(c, currentAlphabet) + 1 == 1)
                        {
                            bitmapBuilder.Append("0");
                        }
                        else
                        {
                            bitmapBuilder.Append("1");
                        }
                    }

                    currentNode.setBitmap(bitmapBuilder.ToString());
                }

                return;
            }

        }

        public int getIndex(char c, ArrayList arrayList)
        {
            for (int i = 0; i < arrayList.Count; i++)
            {
                if (c == (char)arrayList[i])
                {
                    return i;
                }
            }

            return -1;
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
