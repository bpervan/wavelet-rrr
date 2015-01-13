#ifndef WAVELET_TREE_H_
#define WAVELET_TREE_H_
#include <stdint.h>

#include <fstream>
#include <string>
#include "wavelet_tree_node.h"
#include "rrr.h"

// This class represents wavelet tree
// Sample usage:
// WaveletTree* wt = WaveletTree::CreateTreeFromString(str);
// or
// WaveletTree* wt = WaveletTree::CreateTreeFromFASTAFile(filename);
// ...
// delete wt;
class WaveletTree {
	
public:
	// Destructor
	~WaveletTree();

	// Returns a wavelet tree created from input string
	static WaveletTree* CreateTreeFromString(const std::string& input_string);

	// Returns a wavelet tree created from sequence in FASTA file
	static WaveletTree* CreateTreeFromFASTAFile(std::string file);

	// Returns number of appearance of  given character from beginning of
	// the string till the given index included through it's last parametar.
	// Method returns true if succeeded, false otherwise
	bool Rank(char character, uint32_t index, uint32_t& appearance);

	// 
	bool Select(char character, uint32_t appearance, uint32_t& index);

	// Returns character of a string on given index
	char GetCharAt(uint32_t index);

	// Stores a description of sequence encoded with wavelet tree
	void SetDescription(std::string description);

	// Returns description of a string
	std::string description();

	// Returns the alphabet of a string encoded with wavelet tree
	std::string alphabet();

	// Returns the length of a string encoded with wavelet tree
	uint32_t length();

private:
	// Constructor
	WaveletTree();

	// Gets and stores the alphabet of the input string
	void SetAlphabet(const std::string& input_string);

	// Stores the length of the input string
	void SetLength(const std::string& input_string);

	// Should be called to create tree
	void CreateTree(const std::string& input_string);

	// Recursive method which creates wavelet tree
	WTNode* CreateTreeRecursion(const std::string& input_string, std::string alphabet);

	// Root node of wavelet tree
	WTNode* root_;

	// Alphabet of a string encoded with wavelet tree
	std::string alphabet_;

	// Description of an encoded string
	std::string description_;

	// Length of the encoded string
	uint32_t length_;
};

#endif // WAVELET_TREE_NODE_H_
