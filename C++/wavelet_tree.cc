#include "wavelet_tree.h"

WaveletTree::WaveletTree() {
	root_ = NULL;
	alphabet_ = "";
	description_ = "";
	length_ = 0;
}

WaveletTree::~WaveletTree() {
	if (root_ != NULL) {
		delete root_;
		root_ = NULL;
	}
}

WaveletTree* WaveletTree::CreateTreeFromString(const std::string& input_string) {
	//TODO
	return NULL;
}

WaveletTree* WaveletTree::CreateTreeFromFASTAFile(std::string file) {
	//TODO
	return NULL;
}

uint32_t WaveletTree::Rank(char character, uint32_t index)
{
	return root_->GetCharRank(character, index, alphabet_);
}

char WaveletTree::GetCharAt(uint32_t index) {
	return root_->GetCharAt(index);
}

std::string WaveletTree::description() {
	return description_;
}

std::string WaveletTree::alphabet() {
	return alphabet_;
}

uint32_t WaveletTree::length() {
	return length_;
}

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

void WaveletTree::SetLength(const std::string& input_string) {
	length_ = input_string.length();
}

void WaveletTree::SetDescription(std::string description) {
	description_ = description;
}

void WaveletTree::CreateTree(const std::string& input_string) {
	if (alphabet_.length() < 1) {
		return;
	}
	else {
		root_ = CreateTreeRecursion(input_string, alphabet_);
	}
}

WTNode* WaveletTree::CreateTreeRecursion(const std::string& input_string, std::string alphabet) {
	//TODO
	return NULL;
}
