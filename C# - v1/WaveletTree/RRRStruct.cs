using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace WaveletTree
{
    //class that creates rrr data and calculates rank and select
    public class RRRStruct
    {
        //constructor
        //initializes data, iterates through input data and creates rrr-structure (blocks and superblocks data) amd permutation tables
        public RRRStruct(List<bool> bitArray)
        {
            _bitArray = new List<bool>(bitArray);
            _wordLength = _bitArray.Count;
            _blockLength = (int)Math.Floor(Math.Log(_wordLength, 2) / 2);
            var lastBlockBitCount = bitArray.Count % this._blockLength;
            if (lastBlockBitCount != 0)
            {
                for (int i = 0; i < _blockLength - lastBlockBitCount; i++ )
                    _bitArray.Add(false);
            }

            _superBlockLength = _blockLength * (int)Math.Floor(Math.Log(_wordLength, 2));
            PopulateBlocks();
            PopulateBlocksInfoAndSuperBLocksInfo();

            for (int i = 0; i <= _blockLength; i++)
                GetPermutations(i);

        }
        public static int sumofallsums;

        private List<bool> _bitArray;
        private int _wordLength = 0;
        private int _blockLength = 0;
        private int _superBlockLength = 0;
        private List<List<bool>> _blocks = new List<List<bool>>();
        private List<Tuple<int, int>> _superBlocks = new List<Tuple<int, int>>();

        //separates input data into blocks
        public void PopulateBlocks()
        {
            _blocks.Clear();
            int i = 0;
            while (i < _wordLength)
            {
                _blocks.Add(_bitArray.GetRange(i, _blockLength));
                i = i + _blockLength;
            }          
        }

        private List<bool> _blocksInfo = new List<bool>(0);

        //called from constructor
        //populates rrr-structure
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
                    Console.WriteLine("Creating...");

                blockCount++;
                var rank = GetRank(block);
                var rankArray = Convert.ToString(rank, 2).PadLeft((int)Math.Floor(Math.Log(_blockLength, 2)) + 1, '0').ToList();
                var rankArrayCoverted = rankArray.Select(chr => chr == '1').ToList();
                var index = GetPermutations(rank).FindIndex(t => ArrayMatch(block, t.Item1));

                _blocksInfo.AddRange(rankArrayCoverted);

                if (GetPermutations(rank).Count() != 1)
                {
                    var indexArray = Convert.ToString(index, 2).PadLeft((int)Math.Floor(Math.Log(GetPermutations(rank).Count() - 1, 2)) + 1, '0').ToList();
                    var indexArrayCoverted = indexArray.Select(chr => chr == '1').ToList();
                    _blocksInfo.AddRange(indexArrayCoverted);
                }
                else
                {
                    var indexArray = Convert.ToString(index, 2).PadLeft(1, '0').ToList();
                    var indexArrayCoverted = indexArray.Select(chr => chr == '1').ToList();
                    _blocksInfo.AddRange(indexArrayCoverted);
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

        //calculates rank for entire list
        private int GetRank(List<bool> query)
        {
            return query.Count(x => x == true);
        }

        private List<Tuple<List<bool>, List<int>>> _permutations = new List<Tuple<List<bool>, List<int>>>();

        private Dictionary<int, List<Tuple<List<bool>, List<int>>>> _permutationsDictionary = new Dictionary<int, List<Tuple<List<bool>, List<int>>>>();

        //gets permutations for given rank or calculates them if they dont exist yet
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

        //recursively generates permutations
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

        }

        //creates cumulative sum array for given array [0,1,1,0] - [0,1,2,2]
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

        //checks if two arrays are equal
        private bool ArrayMatch(List<bool> item1, List<bool> item2)
        {
            return item1.SequenceEqual(item2);
        }

        //brute force rank calculating for small lists
        public int CalculateRankDummy(int positition)
        {
            var array = _bitArray;
            int rank = 0;
            for (int i = 0; i <= positition; i++)
                if (array[i] == true)
                    rank++;

            return rank;
        }

        //brute force select calculating for small lists (when 1 is searched)
        public int CalculateSelectDummy(int value)
        {
            var array = _bitArray;
            int sum = 0;
            for (int i = 0; i <= array.Count; i++)
            {
                if (array[i] == true)
                    sum++;
                if (sum == value)
                    return i;
            }

            return 0;
        }

        //brute force select calculating for small lists (when 0 is searched)
        public int CalculateSelectDummyZero(int value)
        {
            var array = _bitArray;
            int sum = 0;
            for (int i = 0; i <= array.Count; i++)
            {
                if (array[i] == false)
                    sum++;
                if (sum == value)
                    return i;
            }

            return 0;
        }

        //uses created structure and calculates rank for given position
        //first it uses superblocks, then blocks, and then specific block info
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

        //helper method to add n block-ranks to result
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

        //helper method that reads rrr-record and fetches cumulative sum for block
        private List<int> GetBlockCumalativeSum(int startPosition, List<bool> array)
        {
            var blockClassBits = array.GetRange(startPosition, (int)Math.Floor(Math.Log(_blockLength, 2)) + 1); 
            var blockClass = ArrayToInt(blockClassBits);
            var permutationsListSize = GetPermutations(blockClass).Count();
            int bitsNeeded;
            if (permutationsListSize != 1)
                bitsNeeded = (int)Math.Floor(Math.Log(permutationsListSize - 1, 2)) + 1;
            else bitsNeeded = 1;
            var permutationIndexArray = array.GetRange(startPosition + (int)Math.Floor(Math.Log(_blockLength, 2)) + 1, bitsNeeded);
            var permutationIndex = ArrayToInt(permutationIndexArray);
            return GetPermutations(blockClass)[permutationIndex].Item2;
        }

        //converts bool array to integer
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

        //uses created structure and calculates rank for given position (when 1 is searched)
        //first it uses superblocks, then blocks, and then specific block info
        public int CalculateSelect(int value)
        {
            int rez = 0;
            int offset = 0;
            int index = 0;
            int sum = 0;
            int StartPosition = 0;
            for (int i = 0; i < _superBlocks.Count(); i++)
                if (_superBlocks[i].Item1 >= value)
                {
                    if (i > 0)
                    {
                        rez = i * _superBlockLength * _blockLength;
                        StartPosition = _superBlocks[i - 1].Item2;
                        sum += _superBlocks[i - 1].Item1;
                    }
                    else
                        rez = 0;
                    index = i;
                    break;
                }

            while(true)
            {
                var blockClassBits = _blocksInfo.GetRange(StartPosition + offset, (int)Math.Floor(Math.Log(_blockLength, 2)) + 1);
                var blockClass = ArrayToInt(blockClassBits);
                if (sum + blockClass >= value)
                {
                    break;
                }
                else
                {
                    sum += blockClass;
                    rez += _blockLength;
                    offset = offset + (int)Math.Floor(Math.Log(_blockLength, 2)) + 1;
                    var permutationsListSize = GetPermutations(blockClass).Count();
                    int bitsNeeded;
                    if (permutationsListSize != 1)
                        bitsNeeded = (int)Math.Floor(Math.Log(permutationsListSize - 1, 2)) + 1;
                    else bitsNeeded = 1;
                    offset = offset + bitsNeeded;
                }
            }
            var blockClassBitsFinal = _blocksInfo.GetRange(StartPosition + offset, (int)Math.Floor(Math.Log(_blockLength, 2)) + 1);
            var blockClassFinal = ArrayToInt(blockClassBitsFinal);

            var permutationsListSizeFinal = GetPermutations(blockClassFinal).Count();
            int bitsNeededFinal;
            if (permutationsListSizeFinal != 1)
                bitsNeededFinal = (int)Math.Floor(Math.Log(permutationsListSizeFinal - 1, 2)) + 1;
            else bitsNeededFinal = 1;
            var permutationIndexArrayFinal = _blocksInfo.GetRange(StartPosition + offset + (int)Math.Floor(Math.Log(_blockLength, 2)) + 1, bitsNeededFinal);
            var permutationIndex = ArrayToInt(permutationIndexArrayFinal);

            var permutationList = GetPermutations(blockClassFinal);
            var permutationCumulative = permutationList[permutationIndex].Item2;
            var position = permutationCumulative.IndexOf(value - sum);
            rez += position;


            var rezzz = this.CalculateSelectDummy(value);

            return rez;
        }

        //uses created structure and calculates rank for given position (when 0 is searched)
        //first it uses superblocks, then blocks, and then specific block info
        public int CalculateSelectZero(int value)
        {
            int rez = 0;
            int offset = 0;
            int index = 0;
            int sum = 0;
            int StartPosition = 0;
            for (int i = 0; i < _superBlocks.Count(); i++)
                if (((i + 1) * _superBlockLength * _blockLength) -  _superBlocks[i].Item1 >= value)
                {
                    if (i > 0)
                    {
                        rez = i * _superBlockLength * _blockLength;
                        StartPosition = _superBlocks[i - 1].Item2;
                        sum += ((i) * _superBlockLength * _blockLength) - _superBlocks[i - 1].Item1;
                    }
                    else
                        rez = 0;
                    index = i;
                    break;
                }


            while (true)
            {
                var blockClassBits = _blocksInfo.GetRange(StartPosition + offset, (int)Math.Floor(Math.Log(_blockLength, 2)) + 1);
                var blockClass = ArrayToInt(blockClassBits);
                if (sum + (_blockLength - blockClass) >= value)
                {
                    break;
                }
                else
                {
                    sum = sum + (_blockLength - blockClass);
                    rez += _blockLength;
                    offset = offset + (int)Math.Floor(Math.Log(_blockLength, 2)) + 1;
                    var permutationsListSize = GetPermutations(blockClass).Count();
                    int bitsNeeded;
                    if (permutationsListSize != 1)
                        bitsNeeded = (int)Math.Floor(Math.Log(permutationsListSize - 1, 2)) + 1;
                    else bitsNeeded = 1;
                    offset = offset + bitsNeeded;
                }
            }
            var blockClassBitsFinal = _blocksInfo.GetRange(StartPosition + offset, (int)Math.Floor(Math.Log(_blockLength, 2)) + 1);
            var blockClassFinal = ArrayToInt(blockClassBitsFinal);

            var permutationsListSizeFinal = GetPermutations(blockClassFinal).Count();
            int bitsNeededFinal;
            if (permutationsListSizeFinal != 1)
                bitsNeededFinal = (int)Math.Floor(Math.Log(permutationsListSizeFinal - 1, 2)) + 1;
            else bitsNeededFinal = 1;
            var permutationIndexArrayFinal = _blocksInfo.GetRange(StartPosition + offset + (int)Math.Floor(Math.Log(_blockLength, 2)) + 1, bitsNeededFinal);
            var permutationIndex = ArrayToInt(permutationIndexArrayFinal);


            var permutationList = GetPermutations(blockClassFinal);
            var permutationCumulative = permutationList[permutationIndex].Item2;
            int tempSum = 0;
            int position = 0;
            for (int i = 0; i < permutationCumulative.Count(); i++)
            {
                if (i > 0)
                {
                    if (permutationCumulative[i] == permutationCumulative[i - 1])
                        tempSum++;
                }
                else
                    if (permutationCumulative[i] == 0)
                        tempSum++;


                if (tempSum + sum == value)
                {
                    position = i;
                    break;
                }
            }


            rez += position;

            var rezzz = this.CalculateSelectDummyZero(value);


            return rez;
        }




    }
}
