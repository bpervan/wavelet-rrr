#include "wavelet_tree_node.h"

// WTNode destructor.
WTNode::~WTNode() {}

// WTInternalNode constructor
WTInternalNode::WTInternalNode(RRR* rrr) {
	rrr_sequence_ = rrr;
	left_ = NULL;
	right_ = NULL;
}

// WTInternal node destructor
WTInternalNode::~WTInternalNode() {
	if (rrr_sequence_ != NULL) {
		delete rrr_sequence_;
		rrr_sequence_ = NULL;
	}

	if (left_ != NULL) {
		delete left_;
		left_ = NULL;
	}

	if (right_ != NULL) {
		delete right_;
		right_ = NULL;
	}
}

// Returns character on given index from wavelet tree.
char WTInternalNode::GetCharAt(uint32_t index) {
	if (rrr_sequence_->Access(index) == 0) {
		uint32_t rank = rrr_sequence_->Rank0(index);
		return left_->GetCharAt(rank - 1);
	}
	else {
		uint32_t rank = rrr_sequence_->Rank1(index);
		return right_->GetCharAt(rank - 1);
	}
}

// Sets left child
void WTInternalNode::SetLeftChild(WTNode* node) {
	left_ = node;
}

// Sets right child
void WTInternalNode::SetRightChild(WTNode* node) {
	right_ = node;
}

// Recursive method which returns number of appereance of specified character
// from beginning till the specified index
uint32_t WTInternalNode::GetCharRank(	char character,
																			uint32_t index,
																			std::string alphabet) {
	if (alphabet.length() == 2) {
		if (character == alphabet[0]) {
			return rrr_sequence_->Rank0(index);
		}
		else {
			return rrr_sequence_->Rank1(index);
		}
	}

	// All characters less or equal 'limit' are coded with 0, others are 1
	char limit = alphabet[(alphabet.length() - 1) / 2];

	if (character <= limit) {
		uint32_t rank = rrr_sequence_->Rank0(index);

		if ((rank == 0) ||
				(alphabet.substr(0, ((alphabet.length() - 1) / 2) + 1).length() == 1))
			return rank;
		else
			return left_->GetCharRank(character, rank - 1,
																alphabet.substr(0, ((alphabet.length() - 1) / 2) + 1));
	}
	else {
		uint32_t rank = rrr_sequence_->Rank1(index);

		if (	(rank == 0) ||
					(alphabet.substr(((alphabet.length() - 1) / 2) + 1).length() == 1))
			return rank;
		else
			return right_->GetCharRank (
			character, rank - 1, alphabet.substr(((alphabet.length() - 1) / 2) + 1));
	}
}

// WTLeafNode constructor
WTLeafNode::WTLeafNode(char character) {
	character_ = character;
}

// WTLeafNode destructor
WTLeafNode::~WTLeafNode() {}

// Returns character it contains
char WTLeafNode::GetCharAt(uint32_t index) {
	return character_;
}

// Does nothing (leaf node does not have and childs)
void WTLeafNode::SetLeftChild(WTNode* node) {}

// Does nothing (Leaf node does not have any child)
void WTLeafNode::SetRightChild(WTNode* node) {}

// Does nothing (it should never be called from leaf node)
uint32_t WTLeafNode::GetCharRank(	char character,
																	uint32_t index,
																	std::string alphabet) {
	return 0;
}
