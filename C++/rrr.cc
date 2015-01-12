#include "rrr.h"

RRR::RRR(uint32_t sequence_length) {
	bits_per_block_ = (int)(log2((double)(sequence_length)) / 2.0);
	if (bits_per_block_ == 0) {
		bits_per_block_ = 1;
	}

	blocks_per_superblock_ = bits_per_block_ * (int)(floor(log2((double)(sequence_length))));
	RRRTable::GetTable(&table_);

	length_ = 0;
	last_block_length_ = 0;
}

RRR::~RRR() {
	RRRTable::ReleaseTable(&table_);
}

uint32_t RRR::length() {
	return length_;
}

int RRR::bits_per_block() {
	return bits_per_block_;
}

int RRR::blocks_per_superblock() {
	return blocks_per_superblock_;
}
