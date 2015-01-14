using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace WaveletTree
{
    public class RRRStruct
    {
        public RRRStruct(BitArray bitArray)
        {
            _bitArray = new BitArray(bitArray); //TODO je ovo pametno?
            _wordLength = _bitArray.Count;
            _blockLength = (int)Math.Floor(Math.Log(_wordLength, 2) / 2);
            var lastBlockBitCount = bitArray.Length % this._blockLength;
            if (lastBlockBitCount != 0)
            {
                _bitArray = AppendArray(_bitArray, new BitArray(_blockLength - lastBlockBitCount, false));
            }


            _superBlockLength = _blockLength * (int)Math.Floor(Math.Log(_wordLength, 2));
            PopulateBlocks();
            PopulateBlocksInfoAndSuperBLocksInfo();

            Console.WriteLine("Created Node");
        }

        private BitArray _bitArray;
        private int _wordLength = 0;
        private int _blockLength = 0;
        private int _superBlockLength = 0;
        private List<BitArray> _blocks = new List<BitArray>();
        private List<Tuple<int, int>> _superBlocks = new List<Tuple<int, int>>();


        public void PopulateBlocks()
        {
            _blocks.Clear();
            int i = 0;
            while (i < _wordLength)
            {
                _blocks.Add(SubArray(_bitArray, i, _blockLength));
                i = i + _blockLength;
            }          
        }

        private BitArray _blocksInfo = new BitArray(0);

        public void PopulateBlocksInfoAndSuperBLocksInfo()
        {
            _superBlocks.Clear();
            _blocksInfo.Length = 0;
            int blockCount = 0;
            int tmp = 0;
            int rankSum = 0;
            foreach (var block in _blocks)
            {
                tmp++;
                if (tmp % 10000 == 0)
                    Console.WriteLine(tmp);
                blockCount++;
                var rank = GetRank(block);
                var rankArray = new BitArray(new int[] { rank });
                var permutations = GetPermutations(rank);
                var index = permutations.FindIndex(t => ArrayMatch(block, t.Item1));
                //var index = 0;

                AppendArray(_blocksInfo, InvertOrder(SubArray(rankArray, 0, (int)Math.Floor(Math.Log(_blockLength, 2)) + 1)));

                if (GetPermutations(rank).Count() != 1)
                    AppendArray(_blocksInfo, InvertOrder(SubArray(new BitArray(new int[] { index }), 0, (int)Math.Floor(Math.Log(GetPermutations(rank).Count() - 1, 2)) + 1)));
                else
                    AppendArray(_blocksInfo, InvertOrder(SubArray(new BitArray(new int[] { index }), 0, 1)));


                if (blockCount == _superBlockLength || block.Equals(_blocks[_blocks.Count() - 1]))
                {
                    rankSum += rank;
                    _superBlocks.Add(Tuple.Create(rankSum, _blocksInfo.Count));
                    blockCount = 0;
                }
                else
                {
                    rankSum += rank;
                }
            }            
        }

        //private int GetRank(string query)
        //{
        //    return query.Count(x => x == '1');
        //}

        private int GetRank(BitArray query)
        {
            var array = new bool[query.Count];
            query.CopyTo(array, 0);
            return array.Count(x => x == true);
        }

        private List<Tuple<BitArray, List<int>>> _permutations = new List<Tuple<BitArray, List<int>>>();

        private Dictionary<int, List<Tuple<BitArray, List<int>>>> _permutationsDictionary = new Dictionary<int, List<Tuple<BitArray, List<int>>>>();

        public List<Tuple<BitArray, List<int>>> GetPermutations(int rank)
        {

            if (!_permutationsDictionary.ContainsKey(rank))
            {
                _permutations = new List<Tuple<BitArray, List<int>>>();
                GeneratePermutations(new BitArray(0), rank);
                _permutationsDictionary.Add(rank, _permutations);
                return _permutations;
            }
            else
                return _permutationsDictionary.FirstOrDefault(t => t.Key == rank).Value;
        }

        private void GeneratePermutations(BitArray array, int rank)
        {
            if (array.Count == this._blockLength && rank == 0)
            {
                var tempTuple = Tuple.Create(new BitArray(array), CalculateCumulative(array));
                _permutations.Add(tempTuple);
                return;
            }
            if (array.Count == this._blockLength)
                return;

            array.Length = array.Length + 1;
            array.Set(array.Length - 1, true);
            GeneratePermutations(array, rank - 1);
            array.Length = array.Length - 1;

            array.Length = array.Length + 1;
            array.Set(array.Length - 1, false);
            GeneratePermutations(array, rank);
            array.Length = array.Length - 1;

        }
        private BitArray InvertOrder(BitArray array)
        {
            var newArray = new BitArray(array.Length);
            for (int i = 0; i < array.Length; i++)
                newArray.Set(i, array.Get(array.Length - i - 1));
            return newArray;
        }

        private BitArray SubArray(BitArray array, int startIndex, int length)
        {
            var subArray = new BitArray(length);
            for (int i = 0; i < length; i++)
                subArray.Set(i, array.Get(startIndex + i));

            return subArray;
        }


        public BitArray AAppendArray(BitArray current, BitArray after)
        {
            var bools = new bool[current.Count + after.Count];
            current.CopyTo(bools, 0);
            after.CopyTo(bools, current.Count);
            return new BitArray(bools);
        }

        private BitArray AppendArray(BitArray mainArray, BitArray toAdd)
        {
            for (int i = 0; i < toAdd.Length; i++)
            {
                mainArray.Length++;
                mainArray.Set(mainArray.Length - 1, toAdd.Get(i));
            }
            return mainArray;

        }

        private List<int> CalculateCumulative(BitArray array)
        {
            var cumulative = new List<int>();
            int sum = 0;
            for (int i = 0; i < array.Count; i++)
            {
                if (array.Get(i) == true)
                    sum++;

                cumulative.Add(sum);
            }
            return cumulative;
        }

        private bool ArrayMatch(BitArray item1, BitArray item2)
        {
            if (item1.Count != item2.Count)
                return false;
            for (int i = 0; i < item1.Count; i++)
                if (item1[i] != item2[i])
                    return false;
            return true;
        }

        public int CalculateRankDummy(int positition)
        {
            var array = _bitArray;
            int rank = 0;
            for (int i = 0; i <= positition; i++)
                if (array.Get(i) == true)
                    rank++;

            return rank;
        }

        public int CalculateRank(int position)
        {

            var blockNumber = (int)(position / _blockLength);
            var superBlockNumber = (int)(blockNumber / _superBlockLength);

            //superBlocks
            int result;
            if (superBlockNumber > 0)
                result = _superBlocks[superBlockNumber - 1].Item1;
            else
                result = 0;

            //blocks
            var blocksToAddWhole = blockNumber % _superBlockLength;
            var tuple = Tuple.Create(0, 0);
            if (superBlockNumber > 0)
                tuple = AddNBlocks(_superBlocks[superBlockNumber - 1].Item2, _blocksInfo, blocksToAddWhole);
            else
                tuple = AddNBlocks(0, _blocksInfo, blocksToAddWhole);
            result += tuple.Item1;
            var offset = tuple.Item2;

            var positionInLastBlock = position % _blockLength;
            var cumulativeSumArray = new List<int>();
            if (superBlockNumber > 0)
                cumulativeSumArray = GetBlockCumalativeSum(_superBlocks[superBlockNumber - 1].Item2 + offset, _blocksInfo);
            else
                cumulativeSumArray = GetBlockCumalativeSum(0 + offset, _blocksInfo);

            result = result + cumulativeSumArray[positionInLastBlock];
            return result;
        }

        private Tuple<int, int> AddNBlocks(int StartPosition, BitArray array, int n)
        {
            int sum = 0;
            int offset = 0;
            for (int i = 0; i < n; i++)
            {
                var blockClassBits = SubArray(array, StartPosition + offset, (int)Math.Floor(Math.Log(_blockLength, 2)) + 1);
                var blockClass = ArrayToInt(blockClassBits);
                sum = sum + blockClass;
                offset = offset + (int)Math.Floor(Math.Log(_blockLength, 2)) + 1;
                var permutationsListSize = GetPermutations(blockClass).Count();
                int bitsNeeded;
                if (permutationsListSize != 1)
                    bitsNeeded = (int)Math.Floor(Math.Log(permutationsListSize - 1, 2)) + 1;
                else bitsNeeded = 1;
                offset = offset + bitsNeeded;
            }
            return Tuple.Create(sum, offset);
        }

        private List<int> GetBlockCumalativeSum(int startPosition, BitArray array)
        {
            var blockClassBits = SubArray(array, startPosition, (int)Math.Floor(Math.Log(_blockLength, 2)) + 1);
            var blockClass = ArrayToInt(blockClassBits);
            var permutationsListSize = GetPermutations(blockClass).Count();
            int bitsNeeded;
            if (permutationsListSize != 1)
                bitsNeeded = (int)Math.Floor(Math.Log(permutationsListSize - 1, 2)) + 1;
            else bitsNeeded = 1;
            var permutationIndexArray = SubArray(array, startPosition + (int)Math.Floor(Math.Log(_blockLength, 2)) + 1, bitsNeeded);
            var permutationIndex = ArrayToInt(permutationIndexArray);
            return GetPermutations(blockClass)[permutationIndex].Item2;
        }

        private int ArrayToInt(BitArray array)
        {
            int sum = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (array.Get(i))
                    sum = sum + (int)(Math.Pow(2, array.Length - 1 - i));
            }
            return sum;
        }




    }
}
