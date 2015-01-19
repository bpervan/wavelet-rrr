#include <iostream>
#include <fstream>
#include <string>
#include <sys/time.h>
#include <stdint.h>

#include "wavelet_tree.h"
#include "memory_usage.h"

using namespace std;

int main(int argc, char** argv)
{

	struct timeval start_time, end_time;
	char rank_or_select, character;
	string filename;
	uint32_t bound;
	long us;
	bool success;

	WaveletTree* wt = NULL;
	uint32_t result = 0;

	if (argc != 5) {
		cout <<"Wrong number of parameters: \n./WavletTree filename rank_or_select[R or S] char bound \n";
		return -1;
    }

	filename = argv[1];
	rank_or_select = argv[2][0];
	character = argv[3][0];
	bound = atoi(argv[4]);

	if (rank_or_select == 'R') {
		// Creating wavelet tree
		cout <<"Creating tree: ";
		gettimeofday(&start_time,NULL);
		wt = WaveletTree::CreateTreeFromFASTAFile(filename);
		gettimeofday(&end_time,NULL);

		if (wt == NULL) {
			cout <<"FAILED" <<endl;
			cout <<"Could not open file " <<filename <<endl;
			return 1;
		}

		us = (long)((1000000 * end_time.tv_sec + end_time.tv_usec) -
                       (1000000 * start_time.tv_sec + start_time.tv_usec));
		cout <<us <<"us" <<endl;

		// Rank operation
		cout <<"Rank("<<character <<", " <<bound <<"): ";
		gettimeofday(&start_time,NULL);
		success = wt->Rank(character, bound, result);
		gettimeofday(&end_time,NULL);

		us = (long)((1000000 * end_time.tv_sec + end_time.tv_usec) -
        	(1000000 * start_time.tv_sec + start_time.tv_usec));
		if (success) {
			cout <<result <<" [" <<us <<" us]" <<endl;
		}
		else {
			cout <<"Failed. Wrong character or index" <<endl;
		}
	}
	else if (rank_or_select == 'S') {
		// Creating wavelet tree
		cout <<"Creating tree: ";
		gettimeofday(&start_time,NULL);
		wt = WaveletTree::CreateTreeFromFASTAFile(filename);
		gettimeofday(&end_time,NULL);

		if (wt == NULL) {
			cout <<"FAILED" <<endl;
			cout <<"Could not open file " <<filename <<endl;
			return 1;
		}

		us = (long)((1000000 * end_time.tv_sec + end_time.tv_usec) -
                       (1000000 * start_time.tv_sec + start_time.tv_usec));
		cout <<us <<"us" <<endl;

		// Rank operation
		cout <<"Select("<<character <<", " <<bound <<"): ";
		gettimeofday(&start_time,NULL);
		success = wt->Select(character, bound, result);
		gettimeofday(&end_time,NULL);

		us = (long)((1000000 * end_time.tv_sec + end_time.tv_usec) -
        	(1000000 * start_time.tv_sec + start_time.tv_usec));

		if (success) {
			cout <<result <<" [" <<us <<" us]" <<endl;
		}
		else {
			cout <<"Failed. Wrong character or index" <<endl;
		}
	}
	else {
		cout <<"Value of 3rd parameter must be 'R' or 'S'" <<endl;
	}

	cout <<"Memory usage: " <<CalculateMemoryUsage(filename, 3, 1, 131072) <<" B" <<endl;

	delete wt;
	return 0;
}
