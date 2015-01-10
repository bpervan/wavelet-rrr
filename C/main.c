#include "WaveletTree.h"
#include "RRR.h"
#include "Utils.h"
#include <sys/time.h>

int main (int argc, char* argv[]) {

    FILE *inputFile = NULL;
    char* filename, *input;
    int length = 0, block, superblock, bound, Rank, i;
    char c, s_char;
    bool header_line = false;
    WaveletTree *tree = NULL;
    struct timeval start_time, end_time;
    long us;

    //global_table = (RRRTable *)calloc (1, sizeof(RRRTable));

    if (argc != 4) {
        printf ("Wrong number of parameters: WavletTree.exe filename char bound expected\n");
        return -1;
    }
    /** Arguments input */
    filename = argv[1];
    s_char = argv[2][0];
    bound = atoi(argv[3]);

    /** Opening input file */
    inputFile = fopen(filename, "r");

    if (inputFile == NULL) {
        printf ("%s doesn't exist\n", filename);
        return -1;
    }

    input = (char *)calloc (MEMORY_SIZE, sizeof(char));

    /** Reading through file and skipping newline characters */
    while ((c = fgetc(inputFile)) != EOF) {
        if (c == '>') {
            header_line = true;
        }
        if (c == '\n') {
            header_line = false;
            continue;
        }
        if (!header_line) {
            input[length] = c;
            length++;
        }
    }
    length--;

    /** Checking if the upper boundary is too high */
    if (bound > length) {
        printf ("Boundary too high: input length is %d\n", length);
        return -1;
    }

    /** Calculating block and superblock sizes based on
        input length */
    //calculateBlockSizes(length, &block, &superblock);

    /** Creating the RRR table */
    //buildRRRTable(block, superblock);

    //RRRTableToString();

    /** Building the wavelet tree */
    gettimeofday(&start_time,NULL);
    tree = buildWaveletTree(input, length);
    gettimeofday(&end_time,NULL);
    us = (long)((1000000 * end_time.tv_sec + end_time.tv_usec) -
                       (1000000 * start_time.tv_sec + start_time.tv_usec));
    printf ("Building wavelet tree from input length %d took %d us\n", length, us);

    /** Counting character occurance directly from input */
    Rank = 0;
    gettimeofday(&start_time,NULL);
    for (i = 0; i < bound; ++i) {
        if (s_char == input[i]) {
            Rank++;
        }
    }
    gettimeofday(&end_time,NULL);
    us = (long)((1000000 * end_time.tv_sec + end_time.tv_usec) -
                       (1000000 * start_time.tv_sec + start_time.tv_usec));
    printf ("Expected Rank value is %d\n", Rank);
    printf ("Counting characters took %d us\n", us);

    free(input);

    /** Calculating rank operation */
    gettimeofday(&start_time,NULL);
    Rank = rankOperation(tree, s_char, bound);
    gettimeofday(&end_time,NULL);
    us = (long)((1000000 * end_time.tv_sec + end_time.tv_usec) -
                       (1000000 * start_time.tv_sec + start_time.tv_usec));

    /** Result of the rank operation */
    printf ("Rank(%c, %d) is: %d\n", s_char, bound, Rank);
    printf ("Calculating rank operation took %d us\n", us);

    return 0;

}
