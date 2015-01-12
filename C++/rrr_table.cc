#include "rrr_table.h"

// Constructor
RRRTable::RRRTable() {
	CreateTable();
}

// Destructor
RRRTable::~RRRTable() {}

// Returns a table instance through it's parameter
void RRRTable::GetTable(RRRTable** table) {
	if (*table == instance_)
		return;

	counter_++;
	if (instance_ == NULL) {
		instance_ = new RRRTable();
	}

	*table = instance_;
}

// Decreases a counter of how many clients are using the table
// If counter reaches 0, then deletes the table instance
void RRRTable::ReleaseTable(RRRTable** table) {
	if (*table != instance_)
		return;

	if (counter_ > 0) {
		counter_--;
	}

	if (counter_ <= 0) {
		delete instance_;
		instance_ = NULL;
	}

	*table = NULL;
}

// Returns block value from RRR table
uint16_t RRRTable::GetBlock(uint8_t popcount, uint16_t offset) {
	return table_[popcount][offset];
}

// Returns index of a block in RRR table
uint16_t RRRTable::GetOffset(uint8_t popcount, uint16_t block) {
	int number_of_permutations = table_[popcount].size();

	for (int i = 0; i < number_of_permutations; ++i) {
		if (block == table_[popcount][i]) {
			return (uint16_t)i;
		}
	}

	return 0;
}

// Generates a table of all 16-bit permutation
void RRRTable::CreateTable() {
	for (int popcount = 0; popcount <= 16; ++popcount) {
		int number_of_permutations = NumberOfPermutations(16, popcount);
		uint16_t permutation = (1 << popcount) - 1;
		std::vector<uint16_t> permutations;

		permutations.push_back(permutation);
		for (int i = 1; i < number_of_permutations; ++i) {
			permutation = NextPermutation(permutation);
			permutations.push_back(permutation);
		}

		table_.push_back(permutations);
	}
}

// Generates next permutation with a given amount of set bits from
// the previous lexicographical value
uint16_t RRRTable::NextPermutation(uint16_t value) {
	uint16_t mask = 1;
	int16_t rightmost_bit = -1;

	for (int i = 0; i < 16; ++i)
	{
		if (value & mask)
		{
			// remember the index of rightmost set bit
			if (rightmost_bit == -1)
				rightmost_bit = (int16_t)i;

			mask = mask << 1;

			// find the rightmost 01 subsequence
			if (!(value & (mask)))
			{
				// change it's value to 10
				value = value | mask;

				mask = mask >> 1;
				mask = ~mask;
				value = value & (mask);

				// shift [0, i] subsequence to the right,
				// so the rightmost set bit is o index 0
				mask = (1 << (i + 1));
				mask = mask - 1;
				uint16_t right = value & mask;
				right = right >> rightmost_bit;

				mask = ~mask;
				value = value & mask;
				value = value | right;

				return value;
			}
		}
		else
			mask = mask << 1;
	}

	return value;
}

// Returns binomial coefficient(bit_length, popcount)
uint32_t RRRTable::NumberOfPermutations(uint32_t bit_length, uint32_t popcount) {
	if (popcount > bit_length) return 0;
	if (popcount == 0) return 1;

	double numerator = 1.0;
	for (uint32_t i = bit_length - popcount + 1; i <= bit_length; ++i)
	{
		numerator *= i;
	}

	double denominator = 1.0;
	for (uint32_t i = 1; i <= popcount; ++i)
	{
		denominator *= i;
	}

	return (uint32_t)(numerator / denominator);
}

// Singleton instance of the table
RRRTable* RRRTable::instance_ = NULL;

// Keeps track of how many clients are using the table
int RRRTable::counter_ = 0;
