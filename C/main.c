#include "WaveletTree.h"
#include "RRR.h"
#include "Utils.h"

int main (int argc, char* argv[]) {

    FILE *inputFile = NULL;
    char* filename, *input;
    int length = 0, block, superblock, low, high;
    char c, s_char;
    WaveletTree *tree = NULL;

    global_table = (RRRTable *)calloc (1, sizeof(RRRTable));

    if (argc != 5) {
        printf ("Wrong number of parameters: WavletTree.exe filename char low_bound high_bound expected\n");
        return -1;
    }
    /** Arguments input */
    filename = argv[1];
    s_char = argv[2][0];
    low = atoi(argv[3]);
    high = atoi(argv[4]);

    /** Checking boundaries */
    if (low >= high) {
        printf ("Wrong boundaries!\n");
        return -1;
    }

    if (low < 0) {
        printf ("Lower boundary cannot be a negative number: %d\n", low);
        return -1;
    }

    /** Opening input file */
    inputFile = fopen(filename, "r");

    if (inputFile == NULL) {
        printf ("%s doesn't exist\n", filename);
        return -1;
    }

    input = (char *)calloc (MEMORY_SIZE, sizeof(char));

    /** Reading through file and skipping newline characters */
    while ((c = fgetc(inputFile)) != EOF) {
        if (c == '\n') continue;
        input[length] = c;
        length++;
    }
    length--;

    /** Checking if the upper boundary is too high */
    if (high > length) {
        printf ("Upper boundary too high: input length is %d\n", length);
        return -1;
    }

    /** Calculating block and superblock sizes based on
        input length */
    calculateBlockSizes(length, &block, &superblock);

    /** Creating the RRR table */
    buildRRRTable(block, superblock);

    //RRRTableToString();

    /** Building the wavelet tree */
    tree = buildWaveletTree(input, length);

    free(input);

    /** Result of the rank operation */
    printf ("Rank(%c, %d) - Rank (%c, %d) is: %d\n", s_char, high, s_char, low, rankOperation(tree, s_char, high) - rankOperation(tree, s_char, low));

    return 0;

}
