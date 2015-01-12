#ifndef RRR_H_
#define RRR_H_
#include <math.h>
#include <stdint.h>

#include <vector>
#include "rrr_table.h"
#include "rrr_block.h"

class RRR {

public:
	RRR(uint32_t sequence_length);
	~RRR();

	bool AddBlock(uint16_t block, int block_length);
	int Access(uint32_t index);
	uint32_t Rank1(uint32_t index);
	uint32_t Rank0(uint32_t index);
	uint32_t Select1(uint32_t n);
	uint32_t Select0(uint32_t n);

	uint32_t length();
	int bits_per_block();
	int blocks_per_superblock();

private:
	std::vector<RRRBlock> blocks_;
	std::vector<uint32_t> superblocks_;
	RRRTable* table_;

	uint32_t length_;
	int bits_per_block_;
	int blocks_per_superblock_;
	int last_block_length_;
};

#endif /* RRR_BLOCK_H_*/
