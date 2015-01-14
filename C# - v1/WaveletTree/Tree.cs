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
        private List<bool> _bitArrayText = new List<bool>();

        public Tree(string text)
        {
            SortChars(text);
            if (_sortedChars.Count() > 2)
            {
                _text = text;
                string leftText = "";
                string rightText = "";
                //_bitArrayText.Length = text.Count();
                //int bitsAdded = 0;
                if (text.Count() < 4)
                {
                    foreach (var chr in text)
                    {
                        if (GoesLeft(chr))
                        {
                            leftText = leftText + chr.ToString();
                            _bitArrayText.Add(false);
                            //_bitArrayText.Length = _bitArrayText.Length + 1;
                            //_bitArrayText.Set(bitsAdded, false);
                        }
                        else
                        {
                            rightText = rightText + chr.ToString();
                            _bitArrayText.Add(true);
                            //_bitArrayText.Length = _bitArrayText.Length + 1;
                            //_bitArrayText.Set(bitsAdded, true);
                        }
                        //bitsAdded++;
                    }
                }
                else
                {
                    //int h = 0;
                    var leftStringBuilder = new StringBuilder();
                    var rightStringBuilder = new StringBuilder();
                    foreach (var chr in text)
                    {
                        //h++;
                        //if (h % 10000 == 0)
                        //    Console.WriteLine(h);
                        if (GoesLeft(chr))
                        {
                            leftStringBuilder.Append(chr);
                            //leftText.Insert(leftText.Count(), chr.ToString());
                            //leftText = leftText + chr.ToString();
                            //_bitArrayText.Set(bitsAdded, false);
                            _bitArrayText.Add(false);
                        }
                        else
                        {
                            rightStringBuilder.Append(chr);
                            //rightText.Insert(rightText.Count(), chr.ToString());
                            //rightText = rightText + chr.ToString();
                            //_bitArrayText.Set(bitsAdded, true);
                            _bitArrayText.Add(true);
                        }
                        //bitsAdded++;
                    }
                    leftText = leftStringBuilder.ToString();
                    rightText = rightStringBuilder.ToString();
                    rrrStruct = new RRRStruct(_bitArrayText);
                }
                leftNode = new Tree(leftText);
                rightNode = new Tree(rightText);
            }
            else
            {
                //int bitsAdded = 0;
                //_bitArrayText.Length = text.Count();
                _text = text;
                if (text.Count() < 4)
                {
                    foreach (var chr in text)
                    {
                        if (GoesLeft(chr))
                        {
                            //_bitArrayText.Length = _bitArrayText.Length + 1;
                            //_bitArrayText.Set(_bitArrayText.Length - 1, false);
                            _bitArrayText.Add(false);
                        }
                        else
                        {
                            //_bitArrayText.Length = _bitArrayText.Length + 1;
                            //_bitArrayText.Set(_bitArrayText.Length - 1, true);
                            _bitArrayText.Add(true);
                        }
                    }
                }
                else
                {
                    //int h = 0;
                    foreach (var chr in text)
                    {
                        //h++;
                        //if (h % 10000 == 0)
                        //    Console.WriteLine(h);
                        if (GoesLeft(chr))
                        {
                            //leftText = leftText + chr.ToString();
                           // _bitArrayText.Set(bitsAdded, false);
                            _bitArrayText.Add(false);
                            //_bitArrayText.Length = _bitArrayText.Length + 1;
                            //_bitArrayText.Set(_bitArrayText.Length - 1, false);
                        }
                        else
                        {
                            _bitArrayText.Add(true);
                            //_bitArrayText.Set(bitsAdded, true);
                            //_bitArrayText.Length = _bitArrayText.Length + 1;
                            //_bitArrayText.Set(_bitArrayText.Length - 1, true);
                        }
                        //bitsAdded++;
                    }
                    rrrStruct = new RRRStruct(_bitArrayText);
                }
            }
        }
        public string Text
        {
            get { return _text; }
        }

        public List<bool> BitArrayText
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
            _sortedChars = str.Distinct().OrderBy(c => c).ToList();
            //_sortedChars = String.Concat(str.OrderBy(c => c)).Distinct().ToList();
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

        public int GetSelect(char letter, int position)
        {
            if (!this._sortedChars.Contains(letter))
            {
                return 0;
            }
            if (this._sortedChars.Count() < 3) //leaf
            {
                var select = 0;
                if (GoesLeft(letter))
                {
                    if (Text.Count() > 3)
                        select = rrrStruct.CalculateSelectZero(position);
                    else
                        select = this.CalculateSelectDummyZero(position);
                }
                else 
                {
                    if (Text.Count() > 3)
                        select = rrrStruct.CalculateSelect(position);
                    else
                        select = this.CalculateSelectDummy(position);               
                }
                return select;
            }
            else
            {
                var childResult = this.leftNode.GetSelect(letter, position) + this.rightNode.GetSelect(letter, position);
                childResult++;
                var select = 0;
                if (GoesLeft(letter))
                {
                    if (Text.Count() > 3)
                        select = rrrStruct.CalculateSelectZero(childResult);
                    else
                        select = this.CalculateSelectDummyZero(childResult);
                }
                else
                {
                    if (Text.Count() > 3)
                        select = rrrStruct.CalculateSelect(childResult);
                    else
                        select = this.CalculateSelectDummy(childResult);             
                }
                return select;
            }
        }

        public int CalculateRankDummy(int positition)
        {
            var array = _bitArrayText;
            int rank = 0;
            for (int i = 0; i <= positition; i++)
                if (array[i] == true)
                    rank++;

            return rank;
        }

        public int CalculateSelectDummy(int value)
        {
            var array = _bitArrayText;
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

        public int CalculateSelectDummyZero(int value)
        {
            var array = _bitArrayText;
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
    }
}
