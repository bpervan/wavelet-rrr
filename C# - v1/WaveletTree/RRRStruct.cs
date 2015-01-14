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
        public RRRStruct(List<bool> bitArray)
        {
            _bitArray = new List<bool>(bitArray); //TODO je ovo pametno?
            _wordLength = _bitArray.Count;
            _blockLength = (int)Math.Floor(Math.Log(_wordLength, 2) / 2);
            var lastBlockBitCount = bitArray.Count % this._blockLength;
            if (lastBlockBitCount != 0)
            {
                for (int i = 0; i < _blockLength - lastBlockBitCount; i++ )
                    _bitArray.Add(false);
                //_bitArray = AppendArray(_bitArray, new List<bool>(_blockLength - lastBlockBitCount, false));
            }


            _superBlockLength = _blockLength * (int)Math.Floor(Math.Log(_wordLength, 2));
            PopulateBlocks();
            PopulateBlocksInfoAndSuperBLocksInfo();

            Console.WriteLine("Created Node");
        }

        private List<bool> _bitArray;
        private int _wordLength = 0;
        private int _blockLength = 0;
        private int _superBlockLength = 0;
        private List<List<bool>> _blocks = new List<List<bool>>();
        private List<Tuple<int, int>> _superBlocks = new List<Tuple<int, int>>();


        public void PopulateBlocks()
        {
            _blocks.Clear();
            int i = 0;
            while (i < _wordLength)
            {
                _blocks.Add(_bitArray.GetRange(i, _blockLength));
                //_blocks.Add(SubArray(_bitArray, i, _blockLength));
                i = i + _blockLength;
            }          
        }

        private List<bool> _blocksInfo = new List<bool>(0);

        public void PopulateBlocksInfoAndSuperBLocksInfo()
        {
            _superBlocks.Clear();
            _blocksInfo.Clear();
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
                //var rankArray = new List<bool>(new int[] { rank });
                var rankArray = Convert.ToString(rank, 2).PadLeft((int)Math.Floor(Math.Log(_blockLength, 2)) + 1,'0').ToList();
                var rankArrayCoverted = rankArray.Select(chr => chr == '1').ToList();
                var permutations = GetPermutations(rank);
                var index = permutations.FindIndex(t => ArrayMatch(block, t.Item1));


                _blocksInfo.AddRange(rankArrayCoverted);
                
                //AppendArray(_blocksInfo, InvertOrder(SubArray(rankArray, 0, (int)Math.Floor(Math.Log(_blockLength, 2)) + 1)));

                if (GetPermutations(rank).Count() != 1)
                {
                    var indexArray = Convert.ToString(index, 2).PadLeft((int)Math.Floor(Math.Log(GetPermutations(rank).Count() - 1, 2)) + 1, '0').ToList();
                    var indexArrayCoverted = indexArray.Select(chr => chr == '1').ToList();
                    //AppendArray(_blocksInfo, InvertOrder(SubArray(new List<bool>(new int[] { index }), 0, (int)Math.Floor(Math.Log(GetPermutations(rank).Count() - 1, 2)) + 1)));
                    _blocksInfo.AddRange(indexArrayCoverted);
                }
                else
                {
                    var indexArray = Convert.ToString(index, 2).PadLeft(1, '0').ToList();
                    var indexArrayCoverted = indexArray.Select(chr => chr == '1').ToList();
                    _blocksInfo.AddRange(indexArrayCoverted);
                    //AppendArray(_blocksInfo, InvertOrder(SubArray(new List<bool>(new int[] { index }), 0, 1)));
                }


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

        private int GetRank(List<bool> query)
        {
            return query.Count(x => x == true);
        }

        private List<Tuple<List<bool>, List<int>>> _permutations = new List<Tuple<List<bool>, List<int>>>();

        private Dictionary<int, List<Tuple<List<bool>, List<int>>>> _permutationsDictionary = new Dictionary<int, List<Tuple<List<bool>, List<int>>>>();

        public List<Tuple<List<bool>, List<int>>> GetPermutations(int rank)
        {

            if (!_permutationsDictionary.ContainsKey(rank))
            {
                _permutations = new List<Tuple<List<bool>, List<int>>>();
                GeneratePermutations(new List<bool>(0), rank);
                _permutationsDictionary.Add(rank, _permutations);
                return _permutations;
            }
            else
                return _permutationsDictionary.FirstOrDefault(t => t.Key == rank).Value;
        }

        private void GeneratePermutations(List<bool> array, int rank)
        {
            if (array.Count == this._blockLength && rank == 0)
            {
                var tempTuple = Tuple.Create(new List<bool>(array), CalculateCumulative(array));
                _permutations.Add(tempTuple);
                return;
            }
            if (array.Count == this._blockLength)
                return;

            array.Add(true);
            GeneratePermutations(array, rank - 1);
            array.RemoveAt(array.Count() - 1);
            array.Add(false);
            GeneratePermutations(array, rank);
            array.RemoveAt(array.Count() - 1);

            //array.Length = array.Length + 1;
            //array.Set(array.Length - 1, true);
            //GeneratePermutations(array, rank - 1);
            //array.Length = array.Length - 1;

            //array.Length = array.Length + 1;
            //array.Set(array.Length - 1, false);
            //GeneratePermutations(array, rank);
            //array.Length = array.Length - 1;

        }
        private List<bool> InvertOrder(List<bool> array)
        {
            return array;
            //var newArray = new List<bool>(array.Length);
            //for (int i = 0; i < array.Length; i++)
            //    newArray.Set(i, array.Get(array.Length - i - 1));
            //return newArray;
        }

        //private List<bool> SubArray(List<bool> array, int startIndex, int length)
        //{
        //    var subArray = new List<bool>(length);
        //    for (int i = 0; i < length; i++)
        //        subArray.Set(i, array.Get(startIndex + i));

        //    return subArray;
        //}


        //public List<bool> AAppendArray(List<bool> current, List<bool> after)
        //{
        //    var bools = new bool[current.Count + after.Count];
        //    current.CopyTo(bools, 0);
        //    after.CopyTo(bools, current.Count);
        //    return new List<bool>(bools);
        //}

        //private List<bool> AppendArray(List<bool> mainArray, List<bool> toAdd)
        //{
        //    for (int i = 0; i < toAdd.Length; i++)
        //    {
        //        mainArray.Length++;
        //        mainArray.Set(mainArray.Length - 1, toAdd.Get(i));
        //    }
        //    return mainArray;

        //}

        private List<int> CalculateCumulative(List<bool> array)
        {
            var cumulative = new List<int>();
            int sum = 0;
            for (int i = 0; i < array.Count; i++)
            {
                if (array[i] == true)
                    sum++;

                cumulative.Add(sum);
            }
            return cumulative;
        }

        private bool ArrayMatch(List<bool> item1, List<bool> item2)
        {
            return item1.SequenceEqual(item2);

            //if (item1.Count != item2.Count)
            //    return false;
            //for (int i = 0; i < item1.Count; i++)
            //    if (item1[i] != item2[i])
            //        return false;
            //return true;
        }

        public int CalculateRankDummy(int positition)
        {
            var array = _bitArray;
            int rank = 0;
            for (int i = 0; i <= positition; i++)
                if (array[i] == true)
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

        private Tuple<int, int> AddNBlocks(int StartPosition, List<bool> array, int n)
        {
            int sum = 0;
            int offset = 0;
            for (int i = 0; i < n; i++)
            {
                var blockClassBits = array.GetRange(StartPosition + offset, (int)Math.Floor(Math.Log(_blockLength, 2)) + 1); 
                //var blockClassBits = SubArray(array, StartPosition + offset, (int)Math.Floor(Math.Log(_blockLength, 2)) + 1);
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

        private List<int> GetBlockCumalativeSum(int startPosition, List<bool> array)
        {
            var blockClassBits = array.GetRange(startPosition, (int)Math.Floor(Math.Log(_blockLength, 2)) + 1); 
            //var blockClassBits = SubArray(array, startPosition, (int)Math.Floor(Math.Log(_blockLength, 2)) + 1);
            var blockClass = ArrayToInt(blockClassBits);
            var permutationsListSize = GetPermutations(blockClass).Count();
            int bitsNeeded;
            if (permutationsListSize != 1)
                bitsNeeded = (int)Math.Floor(Math.Log(permutationsListSize - 1, 2)) + 1;
            else bitsNeeded = 1;
            var permutationIndexArray = array.GetRange(startPosition + (int)Math.Floor(Math.Log(_blockLength, 2)) + 1, bitsNeeded);
            //var permutationIndexArray = SubArray(array, startPosition + (int)Math.Floor(Math.Log(_blockLength, 2)) + 1, bitsNeeded);
            var permutationIndex = ArrayToInt(permutationIndexArray);
            return GetPermutations(blockClass)[permutationIndex].Item2;
        }

        private int ArrayToInt(List<bool> array)
        {
            int sum = 0;
            for (int i = 0; i < array.Count; i++)
            {
                if (array[i])
                    sum = sum + (int)(Math.Pow(2, array.Count - 1 - i));
            }
            return sum;
        }




    }
}
