#ifndef RRR_H_
#define RRR_H_
#include <math.h>
#include <stdint.h>
#include <vector>
#include "rrr_table.h"
#include "rrr_block.h"

// This class represents RRR structure
class RRR {

public:
	// Constructor
	RRR(uint32_t sequence_length);

	// Destructor
	~RRR();

	// Adds a new block in RRR sequence
	bool AddBlock(uint16_t block, int block_length);

	// Returns a value of bit on index on given index
	int Access(uint32_t index);

	// Returns number of bits with value 1 from beginning of sequence till
	// the bit with given index included
	uint32_t Rank1(uint32_t index);

	// Returns number of bits with value 0 from beginning of sequence till
	// the bit with given index included
	uint32_t Rank0(uint32_t index);

	// Returns index of n-th appearance of bit with value 1
	uint32_t Select1(uint32_t n);

	// Returns index of n-th appearance of bit with value 0
	uint32_t Select0(uint32_t n);

	// Returns length of bitmap encoded with RRR
	uint32_t length();

	// Returns number of bits inside one RRR block
	int bits_per_block();

	// Returns number of blocks inside one RRR superblock
	int blocks_per_superblock();

private:
	// RRR blocks
	std::vector<RRRBlock> blocks_;

	// RRR superblocks
	std::vector<uint32_t> superblocks_;

	// Instance of RRR table
	RRRTable* table_;

	// Length of bitmap encoded with RRR
	uint32_t length_;

	// Number of bits in one RRR block
	int bits_per_block_;

	// Number of blocks in one RRR superblock
	int blocks_per_superblock_;

	// Length of the last block in RRR sequence
	// (it could be less than bits_per_block)
	int last_block_length_;
};

#endif /* RRR_BLOCK_H_*/
