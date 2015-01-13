#include "wavelet_tree.h"

// Constructor
WaveletTree::WaveletTree() {
	root_ = NULL;
	alphabet_ = "";
	description_ = "";
	length_ = 0;
}

// Destructor
WaveletTree::~WaveletTree() {
	if (root_ != NULL) {
		delete root_;
		root_ = NULL;
	}
}

// Returns a pointer to a wavelet tree created from input string given as parameter
WaveletTree* WaveletTree::CreateTreeFromString(const std::string& input_string) {
	WaveletTree* wt = new WaveletTree();
	wt->SetAlphabet(input_string);
	wt->SetLength(input_string);
	wt->CreateTree(input_string);

	return wt;
}

// Returns a pointer to a wavelet tree created from sequence in FASTA file
WaveletTree* WaveletTree::CreateTreeFromFASTAFile(std::string file) {
	std::ifstream fin(file.c_str());
	if (!fin.is_open())
		return NULL;

	std::string description = "";
	std::string line = "";
	std::string input_string = "";

	while (std::getline(fin, line)) {
		if (line[0] == '>')
			description = line.substr(1);
		else
			input_string += line;
	}

	fin.close();
	
	WaveletTree* wt = WaveletTree::CreateTreeFromString(input_string);
	wt->SetDescription(description);
	return wt;
}

// Returns a number of appearance of a given character from beginning of
// a string till the given index included
bool WaveletTree::Rank(char character, uint32_t index, uint32_t& appearance)
{
	if ((index >= length_) || (alphabet_.find(character) == std::string::npos)){
		appearance = length_ + 1;
		return false;
	}

	if (alphabet_.length() == 1) {
		appearance = index;
		return true;
	}

	appearance = root_->GetCharRank(character, index, alphabet_);
	return true;
}

//
bool WaveletTree::Select(char character, uint32_t appearance, uint32_t& index) {
	if ((appearance >= length_)|| (appearance == 0) || (alphabet_.find(character) == std::string::npos)){
		index = length_ + 1;
		return false;
	}

	if (alphabet_.length() == 1) {
		index = appearance - 1;
		return true;
	}

	index = root_->GetCharSelect(character, appearance, alphabet_);

	if (index == (length_ + 1)) {
		return false;
	}

	return true;
}

// Returns character of a string on given index
char WaveletTree::GetCharAt(uint32_t index) {
	return root_->GetCharAt(index);
}

// Returns description of a string
std::string WaveletTree::description() {
	return description_;
}

// Returns the alphabet of a string encoded with wavelet tree
std::string WaveletTree::alphabet() {
	return alphabet_;
}

// Returns the length of a string encoded with wavelet tree
uint32_t WaveletTree::length() {
	return length_;
}

// Gets and stores the alphabet of the input string
void WaveletTree::SetAlphabet(const std::string& input_string) {
	bool chars[256];
	for (int i = 0; i < 256; ++i) {
		chars[i] = false;
	}

	uint32_t string_length = input_string.size();
	for (uint32_t i = 0; i < string_length; ++i) {
		chars[(int)input_string[i]] = true;
	}

	for (int i = 0; i < 256; i++) {
		if (chars[i] == true) {
			alphabet_ += (char)i;
		}
	}
}

// Stores the length of the input string
void WaveletTree::SetLength(const std::string& input_string) {
	length_ = input_string.length();
}

// Stores a description of sequence encoded with wavelet tree
void WaveletTree::SetDescription(std::string description) {
	description_ = description;
}

// Should be called to create tree
void WaveletTree::CreateTree(const std::string& input_string) {
	if (alphabet_.length() < 1) {
		return;
	}
	else {
		root_ = CreateTreeRecursion(input_string, alphabet_);
	}
}

// Recursive method which creates wavelet tree
WTNode* WaveletTree::CreateTreeRecursion(const std::string& input_string, std::string alphabet) {
	WTNode* node;
	if (alphabet.length() == 1) {
		node = new WTLeafNode(alphabet[0]);
		return node;
	}

	RRR* rrr = new RRR(input_string.length());
	int bits_per_block = rrr->bits_per_block();
	std::string left_substring = "";
	std::string right_substring = "";
	char limit = alphabet[(alphabet.length() - 1) / 2];

	uint16_t block = 0;
	uint16_t mask = 1 << (bits_per_block - 1);
	int block_length = 0;

	uint32_t string_length = input_string.length();
	for (uint32_t i = 0; i < string_length; ++i) {
		if (input_string[i] <= limit) {
			left_substring += input_string[i];
		}
		else {
			right_substring += input_string[i];
			block = block | mask;
		}

		block_length++;
		mask = mask >> 1;
		if ((mask == 0) || (i + 1) >= string_length) {
			rrr->AddBlock(block, block_length);
			block = 0;
			block_length = 0;
			mask = 1 << (bits_per_block - 1);
		}
	}

	node = new WTInternalNode(rrr);
	node->SetLeftChild(CreateTreeRecursion(left_substring, alphabet.substr(0, ((alphabet.length() - 1) / 2) + 1)));
	node->SetRightChild(CreateTreeRecursion(right_substring, alphabet.substr(((alphabet.length() - 1) / 2) + 1)));

	return node;
}
