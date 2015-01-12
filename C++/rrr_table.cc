#include "rrr_table.h"

RRRTable::RRRTable() {
	CreateTable();
}

RRRTable::~RRRTable() {}

void RRRTable::GetTable(RRRTable** table) {
	if (*table == instance_)
		return;

	counter_++;
	if (instance_ == NULL) {
		instance_ = new RRRTable();
	}

	*table = instance_;
}

void RRRTable::ReleaseTable(RRRTable** table) {
	if (*table != instance_)
		return;

	if (counter_ > 0) {
		counter_--;
	}

	if (counter_ <= 0) {
		delete instance_;
		instance_ = 0;
	}

	*table = 0;
}

uint16_t RRRTable::GetBlock(uint8_t popcount, uint16_t offset) {
	return table_[popcount][offset];
}

uint16_t RRRTable::GetOffset(uint8_t popcount, uint16_t block) {
	int number_of_permutations = table_[popcount].size();

	for (int i = 0; i < number_of_permutations; ++i) {
		if (block == table_[popcount][i]) {
			return (uint16_t)i;
		}
	}

	return 0;
}

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

uint16_t RRRTable::NextPermutation(uint16_t value) {
	//TODO

	return 0;
}

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

RRRTable* RRRTable::instance_ = 0;
int RRRTable::counter_ = 0;
