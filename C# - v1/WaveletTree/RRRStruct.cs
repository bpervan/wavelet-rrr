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

        public int GetBlockLength()
        {
            if (_blockLength <= 0)
            {
                _blockLength = (int)Math.Floor(Math.Log(WordLength, 2) / 2);
                return _blockLength;
            }
            else
                return _blockLength;
        }

        public int GetSuperBlockLength()
        {
            if (_superBlockLength <= 0)
            {
                _superBlockLength = GetBlockLength() * (int)Math.Floor(Math.Log(WordLength, 2));
                return _superBlockLength;
            }
            else
                return _superBlockLength;
        }

    }
}
