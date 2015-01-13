#ifndef WAVELET_TREE_NODE_H_
#define WAVELET_TREE_NODE_H_
#include <stdint.h>

#include <string>
#include "rrr.h"

// This class represents interface for nodes in wavelet tree
class WTNode
{
public:
	// Destructor
	virtual ~WTNode() = 0;

	// Returns character on index which is given as parameter
	virtual char GetCharAt(uint32_t index) = 0;

	// Sets left child of the node
	virtual void SetLeftChild(WTNode* node) = 0;

	// Sets right child of the node
	virtual void SetRightChild(WTNode* node) = 0;

	// Returns number of appearance of character given as parameter from beggining
	// of string till the index specified as second parameter included
	virtual uint32_t GetCharRank(char character, uint32_t index, std::string alphabet) = 0;

		virtual uint32_t GetCharSelect(char character, uint32_t n, std::string alphabet) = 0;
};

// This class represents internal node in wavelet tree. It contains it's
// RRR sequence and pointers to it's left and right child.
class WTInternalNode : public WTNode
{
public:
	WTInternalNode(RRR* rrr);
	virtual ~WTInternalNode();
	virtual char GetCharAt(uint32_t index);
	virtual void SetLeftChild(WTNode* node);
	virtual void SetRightChild(WTNode* node);
	virtual uint32_t GetCharRank(char character, uint32_t index, std::string alphabet);
	virtual uint32_t GetCharSelect(char character, uint32_t n, std::string alphabet);

private:
	// RRR sequence
	RRR* rrr_sequence_;

	// Left child
	WTNode* left_;

	// Right child
	WTNode* right_;
};

// This class represents leaf node in wavelet tree. It contains only one
// character of the alphabet from the input string
class WTLeafNode : public WTNode
{
public:
	WTLeafNode(char character);
	virtual ~WTLeafNode();
	virtual char GetCharAt(uint32_t index);
	virtual void SetLeftChild(WTNode* node);
	virtual void SetRightChild(WTNode* node);
	virtual uint32_t GetCharRank(char character, uint32_t index, std::string alphabet);
	virtual uint32_t GetCharSelect(char character, uint32_t n, std::string alphabet);

private:
	// Character
	char character_;
};

#endif // WAVELET_TREE_NODE_H_
