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

        private char symbol;
        private int pos;

        public WaveletTreeRRR(string[] args)
        {
            alphabet = new ArrayList();
            rootNode = new WaveletNode();
            /*
            if (args.Length != 3)
            {
                throw new Exception("Error. Check your arguments.");
            }
            
            readFile(args[0]);
             */
            originalSequence = "GGGCTATCGTGCAGGATCAGACTCCTTAACGCGCGTAGGAGACTATGGTGGAAAGACATCAAGAGAGGCTGTCCGAAGTAGATGAAGGTTCTACCTTGGACAGCCTCCCATTTTACTGGATATGAAAGCGTTATTTTAAGAAGTGGACTAGCGCATCGGGGAGCTCTTGCTGCCAAAATCCCCACAAATGTTTCCCGTCCTTCTCCAAATAGGTGACTCTGGCTCCGCGCTCTTCGAGCAAGGCTTTGGCTTTGCGATTTAACTCAACAAAATTATAGGTGCCTGTATCCGTTGTATAATGATCTTCCTGTAATCCCACGATCATAAAGAGCGACAAATAGGAGAGATCGGTCTCCTGAGCGAGGAGGTTAAGGGACGGTTCATAGTAGGCGCCGGACAAGCTAATGATTTTGGAAAACAGTTTTGGATTTTTCATAGCCAGGTGGAGGGAGACCGTTGCTCCTAAAGAGTCACCAGCCAGCACCCTTTCCTCGGGTTCACGG";
            commentLines = "KOMENTAR";
            Console.WriteLine(commentLines);
            Console.WriteLine("Sequence has" + originalSequence.Length + " characters");

            getAlphabet();
            Console.WriteLine("ABECEDA ULAZNOG NIZA");
            for (int i = 0; i < alphabet.Count;i++)
                Console.WriteLine(alphabet[i]);

            buildWaveletTree(alphabet, originalSequence, rootNode);

          // symbol = Char.ToUpper(Char.Parse(args[1]));
           // pos = int.Parse(args[2]) - 1;

            symbol = 'G';
            pos = 0;
            Console.WriteLine("simbol= "+symbol + " pozicija= "+ pos + " RANK= " + rankRRR(rootNode, pos, symbol, alphabet));
            Console.WriteLine("simbol= " + symbol + " pozicija= " +pos + " SELECT= " + select(2,'R'));


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
                StringBuilder RRRbitmap = new StringBuilder();
                int blockSize;
                int superblockSize;
                

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

                blockSize = (int)(Math.Log(currentNode.getBitmap().Length, 2) / 2);
                if (blockSize == 0)
                {
                    blockSize = 1;
                }
                superblockSize = (int)(blockSize* Math.Floor(Math.Log(currentNode.getBitmap().Length, 2)));
                
                currentNode.RRRTable.BlockSize = blockSize;
                currentNode.RRRTable.SuperblockSize = superblockSize;
                Console.WriteLine("BLOKSIZE " + blockSize + " SUPERBLOKSIZE " + superblockSize);
                currentNode.RRRTable.ClassBitsNeeded = (int)Math.Floor(Math.Log(blockSize, 2)) + 1;
                currentNode.RRRTable.buildTableG();

                //isois tablice
                for (int i = 0; i < currentNode.RRRTable.TableG.Keys.Count; i++)
                {
                    List<String> listaOffset = new List<string>();
                    Console.WriteLine("index klase " + i);
                    listaOffset = currentNode.RRRTable.TableG[i];
                    foreach (string s in listaOffset)
                        Console.WriteLine(s);
                }

                    //nadopuni zadnji blok sa nulama
                    if (bitmapBuilder.Length % blockSize != 0)
                    {
                        do
                        {
                            bitmapBuilder.Append("0");
                        } while (bitmapBuilder.Length % blockSize != 0);

                    }
                //postavi bitmapBuilder u helpBitmap
                currentNode.setHelpBitmap(bitmapBuilder.ToString());
                Console.Write("bitmapa: " + currentNode.getHelpBitmap());
                //iteriranje od bloka do bloka sa zbrajanjem jedinica u pojedinom bloku
                //pseudokod u biljeznici
                int totalPopcount=0;
                for (int i = 0; i < currentNode.getHelpBitmap().Length; i=i+blockSize)
                { 
                    int popCount = popcount(currentNode.getHelpBitmap().Substring(i,blockSize));
                    RRRbitmap.Append(ToBin(popCount,currentNode.RRRTable.ClassBitsNeeded)); //spremi klasu sa potrebnim brojem bitova
                    RRRbitmap.Append(ToBin(currentNode.RRRTable.TableG.FirstOrDefault(t => t.Key == popCount).Value.IndexOf(currentNode.getHelpBitmap().Substring(i,blockSize)),(int)Math.Ceiling(Math.Log((GetBinCoeff(blockSize, popCount)),2))));
                    totalPopcount += popCount;
                    if (((i+blockSize) % superblockSize) == 0 || (i+blockSize) >= currentNode.getHelpBitmap().Length)
                    {
                        currentNode.RRRStruct.superblockSums.Add(totalPopcount);
                        Console.WriteLine("\nSuperblock sum " + totalPopcount);
                        currentNode.RRRStruct.superblockOffsets.Add(RRRbitmap.Length);
                        Console.WriteLine("Superblok offset " + RRRbitmap.Length);
                     }
  
                }

               

                currentNode.RRRStruct.Bitmap = RRRbitmap.ToString();

                Console.WriteLine("RRR bitmapa: " + currentNode.RRRStruct.Bitmap + "\n\n");

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
                    StringBuilder RRRbitmap = new StringBuilder();
                    int blockSize;
                    int superblockSize;

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
                    blockSize = (int)(Math.Log(currentNode.getBitmap().Length, 2) / 2);
                    superblockSize = (int)(blockSize * Math.Floor(Math.Log(currentNode.getBitmap().Length, 2)));
                    currentNode.RRRTable.BlockSize = blockSize;
                    currentNode.RRRTable.SuperblockSize = superblockSize;
                    currentNode.RRRTable.ClassBitsNeeded = (int)Math.Floor(Math.Log(blockSize, 2)) + 1;
                    currentNode.RRRTable.buildTableG();

                    

                    for (int i = 0; i < currentNode.RRRTable.TableG.Keys.Count; i++)
                    {
                        List<String> listaOffset = new List<string>();
                        Console.WriteLine("klasa " + i);
                        listaOffset = currentNode.RRRTable.TableG[i];
                        foreach (string s in listaOffset)
                            Console.WriteLine(s);
                    }


                    //nadopuni zadnji blok sa nulama
                    if (bitmapBuilder.Length % blockSize != 0)
                    {
                        do
                        {
                            bitmapBuilder.Append("0");
                        } while (bitmapBuilder.Length % blockSize != 0);

                    }
                    //postavi bitmapBuilder u helpBitmap
                    currentNode.setHelpBitmap(bitmapBuilder.ToString());

                    Console.Write("bitmapa: " + currentNode.getHelpBitmap());
                    //iteriranje od bloka do bloka sa zbrajanjem jedinica u pojedinom bloku
                    //pseudokodsam napisao u biljeznicu
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
                            Console.WriteLine("\nSuperblock sum " + totalPopcount);
                            currentNode.RRRStruct.superblockOffsets.Add(RRRbitmap.Length);
                            Console.WriteLine("Superblok offset " + RRRbitmap.Length);
                        }

                    }

                   

                    currentNode.RRRStruct.Bitmap = RRRbitmap.ToString();
                    Console.WriteLine("RRR bitmapa: " + currentNode.RRRStruct.Bitmap+"\n\n");


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
                Console.WriteLine("INDEX BLOKA " + blockIndex + " INDEX SUPERBLOKA " + superBlockIndex);
               if (superBlockIndex == 0)
               {
                   currentPos = 0;
                   bitsSum = 0;
               }
               else
               {
                   currentPos = currentNode.RRRStruct.superblockOffsets[superBlockIndex - 1];
                   Console.WriteLine("CURRENT POSITION(MORAM BITI 224 " + currentPos); //dobro je
                   bitsSum = currentNode.RRRStruct.superblockSums[superBlockIndex - 1];
                   Console.WriteLine("OVO MORA BITI 114 " + bitsSum);//dobro je
               }
            

               
               blocksRemaining =blockIndex - (currentNode.RRRTable.SuperblockSize / currentNode.RRRTable.BlockSize) * superBlockIndex;
               Console.WriteLine("blocks rem -->moram biti 2 " + blocksRemaining);
               //blockIndex - kolko sam blokova preskočio
               int shift = currentPos;
               int lastClass;
               int lastOffset;
               int klass;
               int offsetBits;
             
               for (int i=0; i<blocksRemaining;i++)
               {
                   klass=Convert.ToInt32(currentNode.RRRStruct.Bitmap.Substring(shift, currentNode.RRRTable.ClassBitsNeeded),2);
                   Console.WriteLine("Class: " + klass);
                   bitsSum+=klass;
                   offsetBits = (int)Math.Ceiling(Math.Log((GetBinCoeff(currentNode.RRRTable.BlockSize, klass)), 2));
                   if (offsetBits == 0) offsetBits = 1;
                   shift =shift + currentNode.RRRTable.ClassBitsNeeded + offsetBits;
               }

               Console.WriteLine("SHIFT " + shift);

               Console.WriteLine("KLASA ČIJI OFFSET TRAŽIMO " + currentNode.RRRStruct.Bitmap.Substring(shift, currentNode.RRRTable.ClassBitsNeeded));//dobije se klasa 6, nje nema
               lastClass = Convert.ToInt32(currentNode.RRRStruct.Bitmap.Substring(shift, currentNode.RRRTable.ClassBitsNeeded), 2);
              
               offsetBits = (int)Math.Ceiling(Math.Log((GetBinCoeff(currentNode.RRRTable.BlockSize, lastClass)), 2));
               if (offsetBits == 0) offsetBits = 1;
               Console.WriteLine("BROJ BITOVA KOJE CITAM ZA OFFSET " + offsetBits);            
               Console.WriteLine("INDEX OFFSETA " + currentNode.RRRStruct.Bitmap.Substring(shift+currentNode.RRRTable.ClassBitsNeeded, offsetBits));
               lastOffset = Convert.ToInt32(currentNode.RRRStruct.Bitmap.Substring(shift+currentNode.RRRTable.ClassBitsNeeded, offsetBits), 2);
               bitsSum += popcount(currentNode.RRRTable.TableG[lastClass][lastOffset], index % currentNode.RRRTable.BlockSize);
               Console.WriteLine("IZ TABLICE: " + currentNode.RRRTable.TableG[lastClass][lastOffset]);
               Console.WriteLine("Bits sum: " + bitsSum);

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

          /*public int selectRRR(WaveletNode currentNode, int index, char character, ArrayList currentAlphabet)
           {
               int mid = (currentAlphabet.Count + 1) / 2;
               int Select = 0, shift, suma = 0;
               bool value;
               if (getIndex(character, currentAlphabet) < mid)
                   value = false;
               else 
                   value=true;
                   //overflow
               for (superblock in superblocks) {
                   if ((superblock.sum > index && value) || (superblock.index*superblock.size - superblock.sum) && !value) {
                       break;
                   } else {
                       Select = superblock.size * superblock.index;//broj bitova prijeđenih do sada
                       suma = superblock.sum;
                       shift = superblock.offset;
                   }
               }

               for (blocks in superblock) {
                   klasa = block.klasa;
                   if (suma + klasa > index) break;
                   else {
                       suma += klasa; // ako je value false onda je suma += blocksize - klasa
                       shift += brojBitovazaklasu + brojbitovazaoffsetteklase;
                       Select += velicina_bloka;
                   }
               }

               block = tablica[lastClass][lastoffset];
               for (i = 0; i < block.size; ++i) {

                   if (suma == index) break;

                   Select++;
                   if (block[i] == 1 && value || block[i] == 0 && !value)
                       suma ++;
               }

               return SelectRRR (currentNode->parent, Select);

               return 0;
           }
        */
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

        public static string ToBin(int value, int len)
        {
            return (len > 1 ? ToBin(value >> 1, len - 1) : null) + "01"[value & 1];
        }

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



            // tree traversal bottom-up once we have node representing our character
            int position = getPositionOfNthOccurrence(currentNode.getBitmap(), nthOccurrence, characterRepresentedWithZero);
            if (position == 0)
            {
                return -1; // that indicates that there is no n occurrences of character!
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

            if (counter == nthOcurrance) // if this is not true, that means that there are no n occurances in bitmap!
                return position;
            else
                return 0;
        }

        class Interval
        {
            private int leftIndex;
            private int rightIndex;

            //Sets starting and ending index
            public Interval(int leftIndex, int rightIndex)
            {
                this.leftIndex = leftIndex;
                this.rightIndex = rightIndex;
            }

            //Get left index
            public int getLeftIndex()
            {
                return leftIndex;
            }

            //Get right index
            public int getRightIndex()
            {
                return rightIndex;
            }

            //Set left index while going into right node
            public void setLeftIndex()
            {
                this.leftIndex = rightIndex - (getSize() / 2 - 1);
            }

            //Set right index while going into left node
            public void setRightIndex()
            {
                this.rightIndex = leftIndex + ((getSize() + 1) / 2 - 1); // getSize + 1 because on odd size of alphabet, left half is bigger for 1
            }

            //Check if interval is bigger than 2
            public bool isGreaterThanTwo()
            {
                if ((rightIndex - leftIndex) > 1) // because it's inclusive
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }


            // Get index in the middle of the interval
            public int getMiddleIndex()
            {
                return (this.leftIndex + this.rightIndex) / 2;
            }

            //Get interval size
            public int getSize()
            {
                return rightIndex - leftIndex + 1;
            }
        }

    }
     



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

    class RRRDataStructure {
        private String bitmap; //string that stores class_index,offset_index pairs
        public List<int> superblockOffsets = new List<int>(); // offset number--pointer to next superblock(total number of bits until the end of superblock)
        public List<int> superblockSums= new List<int>();  //number of set bits until the end of superblock

        public String Bitmap
        {
            get;
            set;
        }

       
    };


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

        //construction of Lookup Table for RRR structure
        public void buildTableG() {

           TableG = new Dictionary<int, List<string>>();
            
            for(int i=0; i<=BlockSize; i++)
              {
                List<string> offsets = new List<string>();
                StringBuilder initOffset= new StringBuilder();
                //int offsetBitsNeeded =(int)Math.Log((GetBinCoeff(blockSize, i)),2); //kao dictionary (klasa, kolko bitova za offsete)
                for (int j = 0; j < BlockSize; j++)
                    if (j <= BlockSize - 1 - i)  //i broj jedinica
                        initOffset.Append("0");
                    else initOffset.Append("1");

                Permutation(initOffset.ToString(), offsets);
                TableG.Add(i, offsets);
            }
        
        }

        
        
        public static void Permutation(string input, List<string> list)
        {
            RecPermutation("", input, list);
        }
        private static void RecPermutation(string soFar, string input, List<string> list)
        {
            if (string.IsNullOrEmpty(input))
            {
                if (!(list.Contains(soFar)))
                    list.Add(soFar);

                return;
            }
            else
            {
                for (int i = 0; i < input.Length; i++)
                {

                    string remaining = input.Substring(0, i) + input.Substring(i + 1);
                    RecPermutation(soFar + input[i], remaining, list);
                }
            }
        }
    };


}
