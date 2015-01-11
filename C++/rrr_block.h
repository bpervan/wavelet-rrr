#ifndef RRR_BLOCK_H_
#define RRR_BLOCK_H_
#include <stdint.h>

// This class represents a block in RRR sequence.
// In RRR sequence, each block contains two informations
// in it: it's class (number of bits with value 1) and it's
// offset (index of block in RRR table). Both informations are
// needed to retrieve block value from RRR table.
class RRRBlock {

public:
	// Constructor
	RRRBlock(uint8_t number_of_ones, uint16_t offset) {
		class_ = number_of_ones;
		offset_ = offset;
	}

	// Destructor
	~RRRBlock() {}

	// Returns block class
	uint8_t GetClass() const { return class_; }

	// Returns block offset
	uint16_t GetOffset() const { return offset_; }

private:
	// Block class
	uint8_t class_;

	// Block offset
	uint16_t offset_;
};

#endif // RRR_BLOCK_H_
