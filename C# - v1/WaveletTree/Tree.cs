using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace WaveletTree
{
    public class Tree
    {
        private string _text;
        private BitArray _bitArrayText = new BitArray(0);

        public Tree(string text)
        {
            SortChars(text);
            if (_sortedChars.Count() > 2)
            {
                _text = text;
                string leftText = "";
                string rightText = "";
                if (text.Count() < 4)
                {
                    foreach (var chr in text)
                    {
                        if (GoesLeft(chr))
                        {
                            leftText = leftText + chr.ToString();
                            _bitArrayText.Length = _bitArrayText.Length + 1;
                            _bitArrayText.Set(_bitArrayText.Length - 1, false);
                        }
                        else
                        {
                            rightText = rightText + chr.ToString();
                            _bitArrayText.Length = _bitArrayText.Length + 1;
                            _bitArrayText.Set(_bitArrayText.Length - 1, true);
                        }
                    }
                }
                else
                {
                    foreach (var chr in text)
                    {
                        if (GoesLeft(chr))
                        {
                            leftText = leftText + chr.ToString();
                            _bitArrayText.Length = _bitArrayText.Length + 1;
                            _bitArrayText.Set(_bitArrayText.Length - 1, false);
                        }
                        else
                        {
                            rightText = rightText + chr.ToString();
                            _bitArrayText.Length = _bitArrayText.Length + 1;
                            _bitArrayText.Set(_bitArrayText.Length - 1, true);
                        }
                    }
                    rrrStruct = new RRRStruct(_bitArrayText);
                }

                leftNode = new Tree(leftText);
                rightNode = new Tree(rightText);
            }
            else
            {
                _text = text;
                if (text.Count() < 4)
                {
                    foreach (var chr in text)
                    {
                        if (GoesLeft(chr))
                        {
                            _bitArrayText.Length = _bitArrayText.Length + 1;
                            _bitArrayText.Set(_bitArrayText.Length - 1, false);
                        }
                        else
                        {
                            _bitArrayText.Length = _bitArrayText.Length + 1;
                            _bitArrayText.Set(_bitArrayText.Length - 1, true);
                        }
                    }
                }
                else
                {
                    foreach (var chr in text)
                    {
                        if (GoesLeft(chr))
                        {
                            _bitArrayText.Length = _bitArrayText.Length + 1;
                            _bitArrayText.Set(_bitArrayText.Length - 1, false);
                        }
                        else
                        {
                            _bitArrayText.Length = _bitArrayText.Length + 1;
                            _bitArrayText.Set(_bitArrayText.Length - 1, true);
                        }
                    }
                    rrrStruct = new RRRStruct(_bitArrayText);
                }
            }
        }
        public string Text
        {
            get { return _text; }
        }

        public BitArray BitArrayText
        {
            get
            {
                return _bitArrayText;
            }
        }

        private Tree leftNode;
        private Tree rightNode;

        private RRRStruct rrrStruct;

        List<char> _sortedChars = new List<char>();

        private void SortChars(string str)
        {
            _sortedChars = String.Concat(str.OrderBy(c => c)).Distinct().ToList();
        }

        private bool GoesLeft(char chr)
        {
            if (_sortedChars.IndexOf(chr) < (_sortedChars.Count() / 2))
                return true;
            else
                return false;
        }

        public int GetRank(char letter, int position)
        {
            
            int result = 0;
            bool direction;
            if (GoesLeft(letter))
                direction = false;
            else
                direction = true;

            int rank = 0;
            if (direction == false)
            {
                if (Text.Count() > 3)
                    rank = position - rrrStruct.CalculateRank(position) + 1;
                else
                    rank = position - this.CalculateRankDummy(position) + 1;

                if (_sortedChars.Count() > 2)
                    result = leftNode.GetRank(letter, rank - 1);
                else return rank;
            }
            else
            {
                if (Text.Count() > 3)
                    rank = rrrStruct.CalculateRank(position);
                else
                    rank = this.CalculateRankDummy(position);

                if (_sortedChars.Count() > 2)
                    result = rightNode.GetRank(letter, rank - 1);
                else
                    return rank;
            }
            return result;
        }

        public int CalculateRankDummy(int positition)
        {
            var array = _bitArrayText;
            int rank = 0;
            for (int i = 0; i <= positition; i++)
                if (array.Get(i) == true)
                    rank++;

            return rank;
        }
    }
}
