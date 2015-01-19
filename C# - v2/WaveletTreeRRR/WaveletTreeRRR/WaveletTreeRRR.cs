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

        private char charachter;
        private char operation;
        private int boundary;


        public WaveletTreeRRR(string[] args)
        {
            alphabet = new ArrayList();
            rootNode = new WaveletNode();
            
            if (args.Length != 4)
            {
                throw new Exception("Error. Check your arguments.");
            }

            //Stopwatch is being used for measuring time consumption of key methods
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            //FASTA file reading
            readFile(args[0]);
            stopWatch.Stop();
            

            Console.WriteLine("Comments are: " + commentLines);
            Console.WriteLine("FASTA sequence has " + originalSequence.Length + " characters");
            Console.WriteLine("Reading of whole FASTA file was taking " + stopWatch.Elapsed.TotalMilliseconds + " ms");
            stopWatch.Reset();

            //determination of original sequence alphabet
            stopWatch.Start();
            getAlphabet();
            stopWatch.Stop();
            Console.WriteLine("Getting and sorting the FASTA sequence alphabet was taking " + stopWatch.Elapsed.TotalMilliseconds + " ms");
            stopWatch.Reset();
            
            //wavelet tree construction
            stopWatch.Start();
            buildWaveletTree(alphabet, originalSequence, rootNode);
            stopWatch.Stop();
            Console.WriteLine("Wavelet tree was made in " + stopWatch.Elapsed.TotalMilliseconds + " ms");
            stopWatch.Reset();

            //loading arguments
            try
			{
				operation = Char.ToUpper(Char.Parse(args[1]));
                charachter = Char.ToUpper(Char.Parse(args[2]));
                boundary = int.Parse(args[3]);

			}
            catch(Exception e)
            {
				throw new Exception("Error loading arguments");
			}

            

            if (operation == 'S')
            {
                /*stopWatch.Start();
                Console.WriteLine("Select operation without RRR data structure for ("+ charachter +", "+boundary+") gave result " + select(boundary,charachter));
                stopWatch.Stop();
                Console.WriteLine("Operation lasted for " + stopWatch.Elapsed.TotalMilliseconds + " ms");
                stopWatch.Reset();*/

                //calling select(c,i) method
                stopWatch.Start();
                Console.WriteLine("Select operation using RRR data structure for (" + charachter + ", " + boundary + ") gave result " + selectRRR(boundary, charachter));
                stopWatch.Stop();
                Console.WriteLine("Operation lasted for " + stopWatch.Elapsed.TotalMilliseconds + " ms");
                stopWatch.Reset();
            }
            else if (operation == 'R')
            {
                /*stopWatch.Start();
                Console.WriteLine("Rank operation without RRR data structure for (" + charachter + ", " + boundary + ") gave result " + rank(rootNode, boundary, charachter, alphabet));
                stopWatch.Stop();
                Console.WriteLine("Operation lasted for " + stopWatch.Elapsed.TotalMilliseconds + " ms");
                stopWatch.Reset();*/
                
                //calling rank(c,i) method
                stopWatch.Start();
                Console.WriteLine("Rank operation using RRR data structure for (" + charachter + ", " + boundary + ") gave result " + rankRRR(rootNode, boundary, charachter, alphabet));
                stopWatch.Stop();
                Console.WriteLine("Operation lasted for " + stopWatch.Elapsed.TotalMilliseconds + " ms");
                stopWatch.Reset();
            }
            else Console.WriteLine("'"+operation+"' operation does not exist!");


        }

        //method readFile() reads whole FASTA file and separates comments from nucleotide sequence 
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

        //getAlphabet() determinates charachers which builds input nucleotide string and puts them into ArrayString
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

        //method buildWaveletTree() constructs wavelet tree. It calls creation of RRR lookup table and
        //adds RRR data structure to every node.
        public void buildWaveletTree(ArrayList currentAlphabet, String currentLabel, WaveletNode currentNode)
        {
            if (currentAlphabet.Count > 2)
            {
                StringBuilder bitmapBuilder = new StringBuilder();
                StringBuilder leftLabel = new StringBuilder();
                StringBuilder rightLabel = new StringBuilder();
                StringBuilder RRRbitmap = new StringBuilder();
                int blockSize;
                int superblockSize;


                int mid = (currentAlphabet.Count + 1) / 2;

                //creation of node bitmap
                foreach (char c in currentLabel)
                {
                    if (getIndex(Char.ToUpper(c), currentAlphabet) < mid)
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

                blockSize = (int)(Math.Log(currentNode.getBitmap().Length, 2) / 2);
                if (blockSize == 0)
                {
                    blockSize = 1;
                }
                superblockSize = (int)(blockSize * Math.Floor(Math.Log(currentNode.getBitmap().Length, 2)));

                //creation of RRRLookupTable 
                currentNode.RRRTable.BlockSize = blockSize;
                currentNode.RRRTable.SuperblockSize = superblockSize;
                currentNode.RRRTable.ClassBitsNeeded = (int)Math.Floor(Math.Log(blockSize, 2)) + 1;
                currentNode.RRRTable.buildTableG();

                //filling the bitmap with additional zeroes so that every block is equal in size
                if (bitmapBuilder.Length % blockSize != 0)
                {
                    do
                    {
                        bitmapBuilder.Append("0");
                    } while (bitmapBuilder.Length % blockSize != 0);

                }

                //creation of bitmap which is being used for creation od RRR data structure
                currentNode.setHelpBitmap(bitmapBuilder.ToString());

                //creation od RRR Data Structure 
                int totalPopcount = 0;
                for (int i = 0; i < currentNode.getHelpBitmap().Length; i = i + blockSize)
                {
                    int popCount = popcount(currentNode.getHelpBitmap().Substring(i, blockSize));
                    RRRbitmap.Append(ToBin(popCount, currentNode.RRRTable.ClassBitsNeeded)); //spremi klasu sa potrebnim brojem bitova
                    RRRbitmap.Append(ToBin(currentNode.RRRTable.TableG.FirstOrDefault(t => t.Key == popCount).Value.IndexOf(currentNode.getHelpBitmap().Substring(i, blockSize)), (int)Math.Ceiling(Math.Log((GetBinCoeff(blockSize, popCount)), 2))));
                    totalPopcount += popCount;
                    if (((i + blockSize) % superblockSize) == 0 || (i + blockSize) >= currentNode.getHelpBitmap().Length)
                    {
                        currentNode.RRRStruct.superblockSums.Add(totalPopcount);
                        currentNode.RRRStruct.superblockOffsets.Add(RRRbitmap.Length);
                    }

                }
                currentNode.RRRStruct.Bitmap = RRRbitmap.ToString();
                currentNode.setLeftChild(new WaveletNode());
                currentNode.getLeftChild().setParent(currentNode);
                buildWaveletTree(currentAlphabet.GetRange(0, mid), leftLabel.ToString(), currentNode.getLeftChild());

                //if current alphabet has more than 3 characters, than it creates right child and calls recursion on this child
                if (currentAlphabet.Count > 3)
                {
                    currentNode.setRightChild(new WaveletNode());
                    currentNode.getRightChild().setParent(currentNode);
                    buildWaveletTree(currentAlphabet.GetRange(mid, (currentAlphabet.Count - mid)),
                        rightLabel.ToString(), currentNode.getRightChild());
                }


            }
            else
            {   //creation of child nodes
                if (currentAlphabet.Count == 2)
                {
                    StringBuilder bitmapBuilder = new StringBuilder();
                    StringBuilder RRRbitmap = new StringBuilder();
                    int blockSize;
                    int superblockSize;

                    foreach (char c in currentLabel)
                    {
                        if (getIndex(Char.ToUpper(c), currentAlphabet) + 1 == 1)
                        {
                            bitmapBuilder.Append("0");
                        }
                        else
                        {
                            bitmapBuilder.Append("1");
                        }
                    }

                    currentNode.setBitmap(bitmapBuilder.ToString());
                    blockSize = (int)(Math.Log(currentNode.getBitmap().Length, 2) / 2);
                    superblockSize = (int)(blockSize * Math.Floor(Math.Log(currentNode.getBitmap().Length, 2)));
                    currentNode.RRRTable.BlockSize = blockSize;
                    currentNode.RRRTable.SuperblockSize = superblockSize;
                    currentNode.RRRTable.ClassBitsNeeded = (int)Math.Floor(Math.Log(blockSize, 2)) + 1;
                    currentNode.RRRTable.buildTableG();

                    if (bitmapBuilder.Length % blockSize != 0)
                    {
                        do
                        {
                            bitmapBuilder.Append("0");
                        } while (bitmapBuilder.Length % blockSize != 0);

                    }

                    currentNode.setHelpBitmap(bitmapBuilder.ToString());

                    int totalPopcount = 0;
                    for (int i = 0; i < currentNode.getHelpBitmap().Length; i = i + blockSize)
                    {
                        int popCount = popcount(currentNode.getHelpBitmap().Substring(i, blockSize));
                        RRRbitmap.Append(ToBin(popCount, currentNode.RRRTable.ClassBitsNeeded)); //spremi klasu sa potrebnim brojem bitova
                        RRRbitmap.Append(ToBin(currentNode.RRRTable.TableG.FirstOrDefault(t => t.Key == popCount).Value.IndexOf(currentNode.getHelpBitmap().Substring(i, blockSize)), (int)Math.Ceiling(Math.Log((GetBinCoeff(blockSize, popCount)), 2))));
                        totalPopcount += popCount;
                        if (((i + blockSize) % superblockSize) == 0 || (i + blockSize) >= currentNode.getHelpBitmap().Length)
                        {
                            currentNode.RRRStruct.superblockSums.Add(totalPopcount);
                            currentNode.RRRStruct.superblockOffsets.Add(RRRbitmap.Length);
                        }

                    }
                    currentNode.RRRStruct.Bitmap = RRRbitmap.ToString();
                }
                return;
            }
        }

        //getIndex() returns index of a character in an arrayList. Used for determination of index of char in alphabet
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

        //method rank() which doesn't use RRR data structure. It returns number of occurenses for given character
        //in input nucleotide sequence
        public int rank(WaveletNode currentNode, int index, char character, ArrayList currentAlphabet)
           {

               if (!currentAlphabet.Contains(character))
               {
                   return 0;
               }

               int mid = (currentAlphabet.Count + 1) / 2;
               int newIndex;
               ArrayList currentAlphabetSliced = new ArrayList();

               if (getIndex(character, currentAlphabet) < mid)
               {
                   newIndex = index - popcount(currentNode.getBitmap(), index);
                   currentNode = currentNode.getLeftChild();
                   currentAlphabetSliced = currentAlphabet.GetRange(0, (currentAlphabet.Count - (mid - 1)));

               }
               else
               {
                   newIndex = popcount(currentNode.getBitmap(), index) - 1;
                   currentNode = currentNode.getRightChild();
                   currentAlphabetSliced = currentAlphabet.GetRange(mid, (currentAlphabet.Count - mid));
               }

               if (currentNode != null)
               {
                   return rank(currentNode, newIndex, character, currentAlphabetSliced);
               }
               else
               {
                   return newIndex+1;
               }
           }

        //method rank() which uses RRR data structure. It returns number of occurenses for given character
        //in input nucleotide sequence
        public int rankRRR(WaveletNode currentNode, int index, char character, ArrayList currentAlphabet)
           {
               int blockIndex;
               int superBlockIndex; 
               int bitsSum;
               int currentPos;
               int blocksRemaining;
               if (!currentAlphabet.Contains(character))
               {
                   return 0;
               }

               int mid = (currentAlphabet.Count + 1) / 2;
               int newIndex;
               ArrayList currentAlphabetSliced = new ArrayList();

               blockIndex = index / currentNode.RRRTable.BlockSize;
               superBlockIndex = blockIndex / (currentNode.RRRTable.SuperblockSize / currentNode.RRRTable.BlockSize);
               if (superBlockIndex == 0)
               {
                   currentPos = 0;
                   bitsSum = 0;
               }
               else
               {
                   currentPos = currentNode.RRRStruct.superblockOffsets[superBlockIndex - 1];
                   bitsSum = currentNode.RRRStruct.superblockSums[superBlockIndex - 1];
               }

               blocksRemaining = blockIndex - (currentNode.RRRTable.SuperblockSize / currentNode.RRRTable.BlockSize) * superBlockIndex;
               
               
               int shift = currentPos;
               int lastClass;
               int lastOffset;
               int klass;
               int offsetBits;
             
               for (int i=0; i<blocksRemaining;i++)
               {
                   klass=Convert.ToInt32(currentNode.RRRStruct.Bitmap.Substring(shift, currentNode.RRRTable.ClassBitsNeeded),2);
                   bitsSum+=klass;
                   offsetBits = (int)Math.Ceiling(Math.Log((GetBinCoeff(currentNode.RRRTable.BlockSize, klass)), 2));
                   if (offsetBits == 0) offsetBits = 1;
                   shift =shift + currentNode.RRRTable.ClassBitsNeeded + offsetBits;
               }

               lastClass = Convert.ToInt32(currentNode.RRRStruct.Bitmap.Substring(shift, currentNode.RRRTable.ClassBitsNeeded), 2);
              
               offsetBits = (int)Math.Ceiling(Math.Log((GetBinCoeff(currentNode.RRRTable.BlockSize, lastClass)), 2));
               if (offsetBits == 0) offsetBits = 1;
               lastOffset = Convert.ToInt32(currentNode.RRRStruct.Bitmap.Substring(shift+currentNode.RRRTable.ClassBitsNeeded, offsetBits), 2);
               bitsSum += popcount(currentNode.RRRTable.TableG[lastClass][lastOffset], index % currentNode.RRRTable.BlockSize);
               
               if (getIndex(character, currentAlphabet) < mid)
               {
                   newIndex = index - bitsSum;
                   currentNode = currentNode.getLeftChild();
                   currentAlphabetSliced = currentAlphabet.GetRange(0, (currentAlphabet.Count - (mid - 1)));

               }
               else
               {
                   newIndex = bitsSum-1;
                   currentNode = currentNode.getRightChild();
                   currentAlphabetSliced = currentAlphabet.GetRange(mid, (currentAlphabet.Count - mid));
               }

               if (currentNode != null)
               {
                   return rankRRR(currentNode, newIndex, character, currentAlphabetSliced);
               }
               else
               {
                   return newIndex+1;
               }
           }

       //popcount(string, int) iterates through string and returns number of sets bits in binary string until given index
        public int popcount(String bitmap, int index)
           {
               int counter = 0;

               for (int i = 0; i < bitmap.Length && i <= index; i++)
               {
                   if (bitmap.ElementAt(i) == '1')
                   {
                       counter++;
                   }
               }

               return counter;
           }

        //popcount(string) returns the number od set bits in binary string
        public int popcount(String bitmap)
        {
            int counter = 0;

            for (int i = 0; i < bitmap.Length; i++)
            {
                if (bitmap.ElementAt(i) == '1')
                {
                    counter++;
                }
            }

            return counter;
        }

        //ToBin() method is being used for returning integer value in binary base with given width
        public static string ToBin(int value, int len)
        {
            return (len > 1 ? ToBin(value >> 1, len - 1) : null) + "01"[value & 1];
        }

        //method GetBinCoeff() return the total number of unique combinations based upon N and K(binominal coefficient)
        public static long GetBinCoeff(long N, long K)
        {
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

        //selectRRR() method returns index of n-th occurence of given character in input nucleotide string
        public int selectRRR(int nthOccurrence, char character)
        {

            Interval alphabeticInterval = new Interval(0, alphabet.Count - 1);
            WaveletNode currentNode = rootNode;
            int indexOfCharInAlph = getIndex(character, alphabet);
            bool characterRepresentedWithZero = true;

            while (alphabeticInterval.isGreaterThanTwo())
            {
                if (alphabeticInterval.getSize() == 3)
                {
                    if (alphabeticInterval.getRightIndex() == indexOfCharInAlph)
                    {
                        characterRepresentedWithZero = false;
                        break;
                    }
                }

                if (indexOfCharInAlph <= alphabeticInterval.getMiddleIndex())
                {
                    currentNode = currentNode.getLeftChild();
                    alphabeticInterval.setRightIndex();
                }
                else
                {
                    currentNode = currentNode.getRightChild();
                    alphabeticInterval.setLeftIndex();
                }
            }

            if (characterRepresentedWithZero)
            {
                if (alphabeticInterval.getLeftIndex() == indexOfCharInAlph)
                    characterRepresentedWithZero = true;
                else
                    characterRepresentedWithZero = false;
            }

            //  bottom-up tree traversal once we have node representing given character
            int position = selectOnBitmap(currentNode, nthOccurrence, characterRepresentedWithZero);
            if (position == 0)
            {
                return -1; // no n occurrences of character
            }

            WaveletNode child = currentNode;
            currentNode = currentNode.getParent();

            while (currentNode != null)
            {
                if (currentNode.getLeftChild().Equals(child))
                {
                    position = selectOnBitmap(currentNode, position, true);
                }
                else
                {
                    position = selectOnBitmap(currentNode, position, false);
                }

                currentNode = currentNode.getParent();
                child = child.getParent();
            }

            return position - 1;
        }

        //selectOnBitmap() method returns the index of n-th occurence of given character(0 or 1).
        //It uses RRR data structure.
        int selectOnBitmap(WaveletNode currentNode, int index, bool characterRepresentedWithZero) 
        {

            int suma = 0, Select = 0, shift = 0;
            for (int i = 0; i < currentNode.RRRStruct.superblockOffsets.Count; i++)
            {
                if ((currentNode.RRRStruct.superblockSums[i] >= index && !characterRepresentedWithZero) || (((i+1) * currentNode.RRRTable.SuperblockSize - (currentNode.RRRStruct.superblockSums[i]) >= index && characterRepresentedWithZero)))
                    break;
                else
                {
                    Select = currentNode.RRRTable.SuperblockSize * (i+1);//broj bitova prijeđenih do sada
                    if (!characterRepresentedWithZero)
                        suma = currentNode.RRRStruct.superblockSums[i];
                    else
                        suma = (i+1) * currentNode.RRRTable.SuperblockSize - (currentNode.RRRStruct.superblockSums[i]);
                  
                    shift = currentNode.RRRStruct.superblockOffsets[i];             
                }
            }
                       
               int klass;
               int offsetBits;
               int lastClass;
               int lastOffset;
               string lastBlock;
               for (int i=0;i<currentNode.RRRTable.SuperblockSize/currentNode.RRRTable.BlockSize;i++) {
                   klass=Convert.ToInt32(currentNode.RRRStruct.Bitmap.Substring(shift, currentNode.RRRTable.ClassBitsNeeded),2);

                   if (((suma + klass >= index) && !characterRepresentedWithZero) || ((suma + currentNode.RRRTable.BlockSize - klass) >= index && characterRepresentedWithZero)) break;
                   else
                   {
                       if (!characterRepresentedWithZero)
                           suma += klass;
                       else
                           suma += currentNode.RRRTable.BlockSize - klass;
                       offsetBits = (int)Math.Ceiling(Math.Log((GetBinCoeff(currentNode.RRRTable.BlockSize, klass)), 2));
                       if (offsetBits == 0) offsetBits = 1;
                       shift = shift + currentNode.RRRTable.ClassBitsNeeded + offsetBits;
                       Select += currentNode.RRRTable.BlockSize;
                   }
               }

               lastClass = Convert.ToInt32(currentNode.RRRStruct.Bitmap.Substring(shift, currentNode.RRRTable.ClassBitsNeeded), 2);
              
               offsetBits = (int)Math.Ceiling(Math.Log((GetBinCoeff(currentNode.RRRTable.BlockSize, lastClass)), 2));
               if (offsetBits == 0) offsetBits = 1;
               lastOffset = Convert.ToInt32(currentNode.RRRStruct.Bitmap.Substring(shift+currentNode.RRRTable.ClassBitsNeeded, offsetBits), 2);
               lastBlock=currentNode.RRRTable.TableG[lastClass][lastOffset];
               for (int i = 0; i < lastBlock.Length; i++) {

                   if (suma == index) break;
                   Select++;
                   if ((lastBlock[i] == '1' && !characterRepresentedWithZero) || (lastBlock[i] == '0' && characterRepresentedWithZero))
                       suma ++;
               }
        
               return Select;
                 
            
        }

        //select() method returns the index of n-th occurence of given character in input nucleotide string.
        //It doesn't use RRR data structure, it uses regular bitmap.
        public int select(int nthOccurrence, char character)
        {

            Interval alphabeticInterval = new Interval(0, alphabet.Count - 1);
            WaveletNode currentNode = rootNode;
            int indexOfCharInAlph = getIndex(character, alphabet);
            bool characterRepresentedWithZero = true;

            while (alphabeticInterval.isGreaterThanTwo())
            {
                if (alphabeticInterval.getSize() == 3)
                {
                    if (alphabeticInterval.getRightIndex() == indexOfCharInAlph)
                    {
                        characterRepresentedWithZero = false;
                        break;
                    }
                }

                if (indexOfCharInAlph <= alphabeticInterval.getMiddleIndex())
                {
                    currentNode = currentNode.getLeftChild();
                    alphabeticInterval.setRightIndex();
                }
                else
                {
                    currentNode = currentNode.getRightChild();
                    alphabeticInterval.setLeftIndex();
                }
            }

            if (characterRepresentedWithZero)
            {
                if (alphabeticInterval.getLeftIndex() == indexOfCharInAlph)
                    characterRepresentedWithZero = true;
                else
                    characterRepresentedWithZero = false;
            }

            int position = getPositionOfNthOccurrence(currentNode.getBitmap(), nthOccurrence, characterRepresentedWithZero);
            if (position == 0)
            {
                return -1;
            }

            WaveletNode child = currentNode;
            currentNode = currentNode.getParent();

            while (currentNode != null)
            {
                if (currentNode.getLeftChild().Equals(child))
                {
                    position = getPositionOfNthOccurrence(currentNode.getBitmap(), position, true);
                }
                else
                {
                    position = getPositionOfNthOccurrence(currentNode.getBitmap(), position, false);
                }

                currentNode = currentNode.getParent();
                child = child.getParent();
            }

            return position-1;
        }

        //select() method returns the index of n-th occurence of given character(which is 0 or 1) in given bitmap.
        public int getPositionOfNthOccurrence(String bitmap, int nthOcurrance, bool characterRepresentedWithZero)
        {
            int counter = 0;
            int position = 0;
            foreach (char c in bitmap)
            {
                if (counter < nthOcurrance)
                {
                    position++;

                    if (characterRepresentedWithZero && c == '0')
                        counter++;
                    else if (!characterRepresentedWithZero && c == '1')
                        counter++;
                }
                else
                    break;
            }

            if (counter == nthOcurrance)
                return position;
            else
                return 0;
        }

        //Interval class is being used in select methods for determing the current alphabet (in certain wavalet node) 
        class Interval
        {
            private int leftIndex;
            private int rightIndex;

            public Interval(int leftIndex, int rightIndex)
            {
                this.leftIndex = leftIndex;
                this.rightIndex = rightIndex;
            }

            public int getLeftIndex()
            {
                return leftIndex;
            }

            public int getRightIndex()
            {
                return rightIndex;
            }

            public void setLeftIndex()
            {
                this.leftIndex = rightIndex - (getSize() / 2 - 1);
            }

            public void setRightIndex()
            {
                this.rightIndex = leftIndex + ((getSize() + 1) / 2 - 1); // getSize + 1 because on odd size of alphabet, left half is bigger for 1
            }

            public bool isGreaterThanTwo()
            {
                if ((rightIndex - leftIndex) > 1) 
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }

            public int getMiddleIndex()
            {
                return (this.leftIndex + this.rightIndex) / 2;
            }

            public int getSize()
            {
                return rightIndex - leftIndex + 1;
            }
        }

    }
     


    //WavaletNode class stores information nodes in wavelet tree
    class WaveletNode
    {
        private String bitmap;
        private String helpBitmap;
        public RRRDataStructure RRRStruct = new RRRDataStructure();
        public RRRLookupTable RRRTable=new RRRLookupTable();

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

         public String getHelpBitmap()
        {
            return helpBitmap;
        }

        public void setHelpBitmap(String helpBitmap)
        {
            this.helpBitmap = helpBitmap;
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

    //class RRRDataStructure stores information of RRR data structure of WaveletNode
    class RRRDataStructure {
        private String bitmap;                                  //string that stores class_index,offset_index pairs
        public List<int> superblockOffsets = new List<int>();   //offset number-pointer to next superblock(total number of bits until the end of superblock)
        public List<int> superblockSums= new List<int>();       //number of set bits until the end of superblock

        public String Bitmap
        {
            get;
            set;
        }

       
    };

    //class RRRLookupTable stores lookup table for creating RRR data structure, and mathod that creates it
    class RRRLookupTable {
        private int blockSize;
        private int superblockSize;
        Dictionary<int, List<string>> tableG;
        private int classBitsNeeded;

        public int BlockSize {
            get; 
            set;
        }
        public int SuperblockSize {
            get; 
            set; 
        }
        public int ClassBitsNeeded { 
            get; 
            set; 
        }
        public Dictionary<int, List<string>> TableG
        {
            get { return tableG; }
            set { tableG = value; }
        }  

        //construction of lookup Table for RRR structure
        public void buildTableG() {

           TableG = new Dictionary<int, List<string>>();
           string block;
           int popCount;
           for (int i = 0; i <= BlockSize; i++)
           {
               TableG.Add(i, new List<string>());
           }
               
           for (int i = 0; i < Math.Pow(2, BlockSize); ++i)
           {
               block = ToBin(i, BlockSize);
               popCount = popcount(block);
               TableG.FirstOrDefault(t => t.Key == popCount).Value.Add(block);
           }

        
        }
        public static string ToBin(int value, int len)
        {
            return (len > 1 ? ToBin(value >> 1, len - 1) : null) + "01"[value & 1];
        }
        public int popcount(String bitmap)
        {
            int counter = 0;

            for (int i = 0; i < bitmap.Length; i++)
            {
                if (bitmap.ElementAt(i) == '1')
                {
                    counter++;
                }
            }

            return counter;
        }
    };


}
