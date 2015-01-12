#ifndef RRR_TABLE_H_
#define RRR_TABLE_H_
#include <stdint.h>
#include <math.h>

#include <vector>
#include <cstdlib>

// Sample usage:
// RRRTable* table;
// RRRTable::GetTable(&table);
// ...
// RRRTable::ReleaseTable(&table);
class RRRTable
{
public:
	static void GetTable(RRRTable** table);

	static void ReleaseTable(RRRTable** table);

	uint16_t GetBlock(uint8_t popcount, uint16_t offset);

	uint16_t GetOffset(uint8_t popcount, uint16_t block);

private:
	RRRTable();

	~RRRTable();

	void CreateTable();

	uint16_t NextPermutation(uint16_t value);

	uint32_t NumberOfPermutations(uint32_t bit_length, uint32_t popcount);

	std::vector<std::vector<uint16_t> > table_;
	static RRRTable* instance_;
	static int counter_;
};

#endif // RRR_TABLE_H_
