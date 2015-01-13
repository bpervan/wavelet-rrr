//Functions in this header are used to calculate memory usage of created Wavelet Tree
#include <math.h>
#include <string>
#include <fstream>

// Returns alphabet of input string
std::string GetAlphabet(std::string& input_string) {
  bool chars[256];
  for (int i = 0; i < 256; ++i) {
    chars[i] = false;
  }

  long string_length = input_string.size();
  for (long i = 0; i < string_length; ++i) {
    chars[(int)input_string[i]] = true;
  }

  std::string alphabet = "";
  for (int i = 0; i < 256; i++) {
    if (chars[i] == true) {
      alphabet += (char)i;
    }
  }

  return alphabet;
}

// Recursive function which calculates number of blocks and superblocks for input string
// and calculates memory usage
long CalcMemUsageRecursion(std::string& input_string, std::string& alphabet, long block_space, long superblock_space)
{
  if (alphabet.length() == 1) {
    return 1;
  }

  char limit = alphabet[(alphabet.length() - 1) / 2];
  std::string left_alphabet = alphabet.substr(0, ((alphabet.length() - 1) / 2) + 1);
  std::string right_alphabet = alphabet.substr(((alphabet.length() - 1) / 2) + 1);

  std::string left = "";
  std::string right = "";
  for (long i=0; i<(long)input_string.length(); i++) {
    if (input_string[i] <= limit) {
      left += input_string[i];
    }
    else {
      right += input_string[i];
    }
  }

  long bits_per_block = (long)(log2((double)(input_string.length())) / 2.0);
  if (bits_per_block == 0) {
    bits_per_block = 1;
  }

  long blocks_per_superblock = bits_per_block * (int)(floor(log2((double)(input_string.length()))));

  long number_of_blocks = input_string.length() / bits_per_block;
  long number_of_superblocks = number_of_blocks / blocks_per_superblock;

  long space = (number_of_blocks * block_space) + (number_of_superblocks * superblock_space);

  space += CalcMemUsageRecursion(left, left_alphabet, block_space, superblock_space);
  space += CalcMemUsageRecursion(right, right_alphabet, block_space, superblock_space);

  return space;
}

// Returns memory usage of wavelet tree
long CalculateMemoryUsage(std::string filename, long block_space, long superblock_space, long table_space)
{
  std::ifstream fin(filename.c_str());
  if (!fin.is_open())
    return 0;

    std::string input_string = "";
    std::string line = "";

    while (std::getline(fin, line)) {
      if (line[0] == '>')
        continue;
      else
        input_string += line;
    }

    fin.close();

    std::string alphabet = GetAlphabet(input_string);
    long size = CalcMemUsageRecursion(input_string, alphabet, block_space, superblock_space);
    return size + table_space;
}
