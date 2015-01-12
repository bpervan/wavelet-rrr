#ifndef RRR_TABLE_H_
#define RRR_TABLE_H_
#include <stdint.h>
#include <math.h>

#include <vector>
#include <cstdlib>

// This class contains table of all possible 16-bit permutations,
// for all popcounts (number of bits with value 1). Because there
// is no need for all RRR sequences in program to have it's own table,
// this class is implemented using singleton design pattern.
// Sample usage:
// RRRTable* table;
// RRRTable::GetTable(&table);
// ...
// RRRTable::ReleaseTable(&table);
class RRRTable {
	
public:
	// Returns an instance of table through it's parameter
	static void GetTable(RRRTable** table);

	// Must be called at end of usage to properly release allocated
	// space for table instance
	static void ReleaseTable(RRRTable** table);

	// Returns block value from the table
	uint16_t GetBlock(uint8_t popcount, uint16_t offset);

	// Returns index of block in the table
	uint16_t GetOffset(uint8_t popcount, uint16_t block);

private:
	// Constructor
	RRRTable();

	// Destructor
	~RRRTable();

	// Generates a RRR table
	void CreateTable();

	// Generates next permutation with a given amount of set bits from
	// the previous lexicographical value
	uint16_t NextPermutation(uint16_t value);

	// Returns binomial coefficient(bit_length, popcount)
	uint32_t NumberOfPermutations(uint32_t bit_length, uint32_t popcount);

	// RRR table. First index is block popcount,
	// second is it's offset in table
	std::vector<std::vector<uint16_t> > table_;

	// Singleton instance of the table
	static RRRTable* instance_;

	// Keeps track of how many clients are using the table
	static int counter_;
};

#endif // RRR_TABLE_H_
