using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveletTree
{
    public class RRRStruct
    {
        public RRRStruct (string word)
	    {
            _word = word;
	    }

        private string _word;
        private int _wordLength = 0;
        private int _blockLength = 0;
        private int _superBlockLength = 0;

        public string Word
        {
            get
            {
                return _word;
            }
        }

        public int WordLength
        {
            get
            {
                if (_wordLength <= 0)
                {
                    _wordLength = _word.Count();
                    return _wordLength;
                }
                else
                    return _wordLength;
            }
        }

        public int BlockLength
        {
            get
            {
                if (_blockLength <= 0)
                {
                    _blockLength = (int)Math.Floor(Math.Log(WordLength, 2) / 2);
                    return _blockLength;
                }
                else
                    return _blockLength;
            }
        }

        public int SuperBlockLength
        {
            get
            {
                if (_superBlockLength <= 0)
                {
                    _superBlockLength = BlockLength * (int)Math.Floor(Math.Log(WordLength, 2));
                    return _superBlockLength;
                }
                else
                    return _superBlockLength;
            }
        }

        private List<string> _blocks = new List<string>();

        public List<string> Blocks
        {
            get
            {
                if (_blocks.Count() == 0)
                {
                    int i = 0;
                    while (i < WordLength - BlockLength)
                    {
                        _blocks.Add(Word.Substring(i, BlockLength));
                        i = i + _blockLength;
                    }
                    _blocks.Add(Word.Substring(i));
                    return _blocks;
                }
                else 
                    return _blocks;
            }
        }

        private List<int> _blocksInfo = new List<int>();

        public List<int> BlocksInfo 
        {
            get 
            {
                if (_blocksInfo.Count() == 0)
                {
                    foreach(var block in Blocks)
                    {
                        _blocksInfo.Add(GetRank(block));
                      
                    }
                }
                else
                    return _blocksInfo;
            }
        }

        private int GetRank(string query)
        {
            return query.Count(x => x == '1');
        }

        private List<string> _permutations = new List<string>();

        




    }
}
