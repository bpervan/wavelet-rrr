#include "rrr.h"

// Constructor
RRR::RRR(uint32_t sequence_length) {
	// Calculate block length from input sequence length
	// block length = log2(sequence_length) / 2
	bits_per_block_ = (int)(log2((double)(sequence_length)) / 2.0);
	if (bits_per_block_ == 0) {
		bits_per_block_ = 1;
	}

	// Get number of block in superblock
	// superblock length = block_length * floor(log2(sequence_length))
	blocks_per_superblock_ =
	bits_per_block_ * (int)(floor(log2((double)(sequence_length))));

	// Get instance of RRRTable
	RRRTable::GetTable(&table_);

	length_ = 0;
	last_block_length_ = 0;
}

// Destructor
RRR::~RRR() {
	RRRTable::ReleaseTable(&table_);
}

// Adds new block into RRR sequence
// It returns false if block_length is bigger than calculated block
// length for this sequence
bool RRR::AddBlock(uint16_t block, int block_length) {
	if (block_length <= 0 || block_length > bits_per_block_) {
		return false;
	}

	// Get popcount of the block
	uint8_t popcount = 0;
	for (int i = 0; i < 16; ++i) {
		if ((block >> i) & 1) {
			popcount++;
		}
	}

	// Get offset of the block
	uint16_t offset = table_->GetOffset(popcount, block);

	// If last superblock is full then add a new one, otherwise add popcount
	// of new block to the last superblock popcount value
	if (!(blocks_.size() % blocks_per_superblock_)) {
		if (superblocks_.size() == 0) {
			superblocks_.push_back(popcount);
		} 
		else {
			superblocks_.push_back(superblocks_[superblocks_.size() - 1] + popcount);
		}
	}
	else {
		superblocks_[superblocks_.size() - 1] =
			superblocks_[superblocks_.size() - 1] + popcount;
	}

	// Add new block into the sequence
	blocks_.push_back(RRRBlock(popcount, offset));
	length_ += block_length;
	last_block_length_ = block_length;

	return true;
}

// Return a value of bit on a given index in encoded bitmap
// It returns -1 if given index is larger than bitmap
int RRR::Access(uint32_t index) {
	if (index >= length_) {
		return -1;
	}

	// Determine index of block in which is given index
	uint32_t block_index = index / bits_per_block_;

	// Determine index of bit on given index inside a block
	int index_in_block = index % bits_per_block_;

	// Get block value
	uint16_t block = table_->GetBlock(	blocks_[block_index].GetClass(),
								blocks_[block_index].GetOffset());

	// Create a mask for & operation
	uint16_t mask = 1 << (bits_per_block_ - index_in_block - 1);

	if (block & mask)
		return 1;
	else
		return 0;
}

// Returns number of appearance of bits with value 1 inside bitmap encoded
// with RRR structure from index 0 to index given as parameter included
uint32_t RRR::Rank1(uint32_t index) {
	if (index >= length_) {
		return length_ + 1;
	}

	// Determine index of block which contains bit with index given as parameter
	uint32_t block_index = index / bits_per_block_;

	uint32_t start_block_index = 0;
	uint32_t rank = 0;

	// Get popcount of previous superblock
	if (block_index >= (uint32_t)blocks_per_superblock_) {
		uint32_t superblock_index = (block_index / blocks_per_superblock_) - 1;
		rank = superblocks_[superblock_index];

		start_block_index = (superblock_index + 1) * blocks_per_superblock_;
	}

	// Iterate through superblock and add popcounts of it's blocks until
	// block which contain bit with given index is reached
	for (uint32_t i = start_block_index; i < block_index; ++i) {
		rank += blocks_[i].GetClass();
	}

	// Determine index of bit inside a block
	int index_in_block = index % bits_per_block_;
	uint16_t block = table_->GetBlock(	blocks_[block_index].GetClass(),
						blocks_[block_index].GetOffset());

	// Iterate through block and count number of bits with value 1 until
	// bit with given index is reached
	for (int i = 0; i <= index_in_block; ++i) {
		if ((block >> (bits_per_block_ - i - 1)) & 1)
			rank++;
	}

	return rank;
}

// Returns number of appearance of bits with value 0 inside bitmap encoded
// with RRR structure from index 0 to index given as parameter included
uint32_t RRR::Rank0(uint32_t index) {
	if (index >= length_) {
		return length_ + 1;
	}

	// Determine index of block which contains bit with index given as parameter
	uint32_t block_index = index / bits_per_block_;

	uint32_t start_block_index = 0;
	uint32_t rank = 0;

	// Get number of bits with value 0 from beginning till the end of previous
	// superblock
	if (block_index >= (uint32_t)blocks_per_superblock_) {
		uint32_t superblock_index = (block_index / blocks_per_superblock_) - 1;

		rank =  ((superblock_index + 1) * blocks_per_superblock_ * bits_per_block_)
				- superblocks_[superblock_index];

		start_block_index = (superblock_index + 1) * blocks_per_superblock_;
	}

	// Iterate through superblock and add (block_length - block_popcount) until
	// block which contain bit with given index is reached
	for (uint32_t i = start_block_index; i < block_index; ++i) {
		rank += (bits_per_block_ - blocks_[i].GetClass());
	}

	// Determine index of bit inside a block
	int index_in_block = index % bits_per_block_;
	uint16_t block = table_->GetBlock(	blocks_[block_index].GetClass(),
						blocks_[block_index].GetOffset());

	// Iterate through block and count number of bits with value 0 until
	// bit with given index is reached
	for (int i = 0; i <= index_in_block; ++i) {
		if (!((block >> (bits_per_block_ - i - 1)) & 1))
			rank++;
	}

	return rank;
}

// Returns index of n-th bit in bitmap with value 1
uint32_t RRR::Select1(uint32_t n) {
	if ((n >= length_) || (n > superblocks_[superblocks_.size() - 1])) {
		return length_ + 1;
	}

	uint32_t index = 0;
	uint32_t counter = 0;
	uint32_t block_index = 0;

	// Find superblock whose value is lower than n, but value of next is
	// bigger than n
	if (n > superblocks_[0]) {
		uint32_t first = 0;
		uint32_t last = superblocks_.size() - 1;
		uint32_t middle = (first + last) / 2;
		uint32_t superblock_index;
		while (true) {
			if (n == superblocks_[middle]) {
				superblock_index = middle - 1;
				break;
			}
			else if (n < superblocks_[middle]) {
				if (middle == 0) {
					superblock_index = 0;
					break;
				}
				else if (n > superblocks_[middle - 1]) {
					superblock_index = middle - 1;
					break;
				}

				last = middle;
			}
			else {
				if (n >= superblocks_[last]) {
					superblock_index = last - 1;
					break;
				}
				else if (n < superblocks_[middle + 1]) {
					superblock_index = middle;
					break;
				}

				first = middle;

			}

			middle = (first + last) / 2;
		}

		// Add value of previous superblock to counter
		counter = superblocks_[superblock_index];
		block_index = (superblock_index + 1) * blocks_per_superblock_;
	}

	// Iterate through blocks and add their popcounts to counter until value of
	// (counter + block_popcount) is lower than n
	while ((counter + blocks_[block_index].GetClass()) < n) {
		counter += blocks_[block_index].GetClass();
		block_index++;
	}

	index = bits_per_block_ * block_index;
	uint16_t block = table_->GetBlock(	blocks_[block_index].GetClass(),
										blocks_[block_index].GetOffset());

	// Iterate through the block and increase value of counter until n is reached
	uint16_t mask = 1 << (bits_per_block_ - 1);
	while (counter < n) {
		if (block & mask) {
			counter++;
			if (counter == n)
				break;
		}

		mask = mask >> 1;
		index++;
	}

	// Return index on which n is reached
	return index;
}

// Returns index of n-th bit in bitmap with value 0
uint32_t RRR::Select0(uint32_t n) {
	if ((n >= length_) || (n > (length_ - superblocks_[superblocks_.size() - 1])))
	{
		return length_ + 1;
	}

	uint32_t index = 0;
	uint32_t counter = 0;
	uint32_t block_index = 0;

	if (n > ((bits_per_block_ * blocks_per_superblock_) - superblocks_[0])) {
		uint32_t first = 0;
		uint32_t last = superblocks_.size() - 1;
		uint32_t middle = (first + last) / 2;
		uint32_t superblock_index;
		uint32_t bits_per_superblock = bits_per_block_ * blocks_per_superblock_;
		while (true) {
			if (n == (((middle + 1) * bits_per_superblock) - superblocks_[middle])) {
				superblock_index = middle - 1;
				break;
			}
			else if (n < (((middle + 1) * bits_per_superblock) - superblocks_[middle]))
			{
				if (middle == 0) {
					superblock_index = 0;
					break;
				}
				else if (n >(((middle) * 	bits_per_superblock) -
						superblocks_[middle - 1])) {
					superblock_index = middle - 1;
					break;
				}

				last = middle;
			}
			else {
				if (n >= ((last + 1 * bits_per_superblock) - superblocks_[last])) {
					superblock_index = last - 1;
					break;
				}
				else if (n < (((middle + 2) * bits_per_superblock) -
							superblocks_[middle + 1])) {
					superblock_index = middle;
					break;
				}

				first = middle;

			}

			middle = (first + last) / 2;
		}

		counter =  ((superblock_index + 1) * bits_per_superblock) -
				superblocks_[superblock_index];
		block_index = (superblock_index + 1) * blocks_per_superblock_;
	}

	while ((counter + (bits_per_block_ - blocks_[block_index].GetClass())) < n) {
		counter += (bits_per_block_ - blocks_[block_index].GetClass());
		block_index++;
	}

	index = bits_per_block_ * block_index;
	uint16_t block = table_->GetBlock(blocks_[block_index].GetClass(),
		blocks_[block_index].GetOffset());

	uint16_t mask = 1 << (bits_per_block_ - 1);
	while (counter < n) {
		if (!(block & mask)) {
			counter++;
			if (counter == n)
				break;
		}

		mask = mask >> 1;
		index++;
	}

	return index;
}

// Returns length of bitmap encoded with RRR structure
uint32_t RRR::length() {
	return length_;
}

// Returns number of bits inside a block
int RRR::bits_per_block() {
	return bits_per_block_;
}

// Returns number of blocks inside a superblock
int RRR::blocks_per_superblock() {
	return blocks_per_superblock_;
}
