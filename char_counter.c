#include <stdio.h>
#include <stdlib.h>
#include <sys/time.h>

int main (int argc, char* argv[]) {

    FILE *inputFile = NULL;
    char* filename, *input, *rank_or_select;
    int length = 0, bound, Rank, Select, i;
    char c, s_char;
    int header_line = 0;
    struct timeval start_time, end_time;
    long us;

    //global_table = (RRRTable *)calloc (1, sizeof(RRRTable));

    if (argc != 5) {
        printf ("Wrong number of parameters: char_counter.exe filename rank_or_select char bound expected\n");
        return -1;
    }
    /** Arguments input */
    filename = argv[1];
    rank_or_select = argv[2];
    s_char = argv[3][0];
    bound = atoi(argv[4]);

    /** Opening input file */
    inputFile = fopen(filename, "r");

    if (inputFile == NULL) {
        printf ("%s doesn't exist\n", filename);
        return -1;
    }

    input = (char *)calloc (10000000, sizeof(char));

    /** Reading through file and skipping newline characters */
    while ((c = fgetc(inputFile)) != EOF) {
        if (c == '>') {
            header_line = 1;
        }
        if (c == '\n') {
            header_line = 0;
            continue;
        }
        if (header_line == 0) {
            input[length] = c;
            length++;
        }
    }
    length--;

    if (!strcmp (rank_or_select, "rank")) {
        gettimeofday(&start_time,NULL);

        /** Checking if the upper boundary is too high */
        if (bound > length) {
            printf ("Boundary too high: input length is %d\n", length);
            return -1;
        }

        /** Counting character occurance directly from input */
        Rank = 0;
        for (i = 0; i < bound; ++i) {
            if (s_char == input[i]) {
                Rank++;
            }
        }
        gettimeofday(&end_time,NULL);
        us = (long)((1000000 * end_time.tv_sec + end_time.tv_usec) -
                           (1000000 * start_time.tv_sec + start_time.tv_usec));

        /** Result of the rank operation */
        printf ("Expected Rank value is %d\n", Rank);
        printf ("Calculating rank operation took %ld us\n", us);
    } else if (!strcmp (rank_or_select, "select")) {

        gettimeofday(&start_time,NULL);
        Select = 0;
        Rank = 0;

        for (i = 0; i < length; ++i) {
            if (s_char == input[i]) {
                Rank++;
            }
            Select++;
            if (Rank == bound) {
                break;
            }
        }
        gettimeofday(&end_time,NULL);
        us = (long)((1000000 * end_time.tv_sec + end_time.tv_usec) -
                           (1000000 * start_time.tv_sec + start_time.tv_usec));

        /** Result of the select operation */
        printf ("Expected Select value is %d\n", Select);
        printf ("Calculating Select operation took %ld us\n", us);
    } else {
        printf ("Second argument can be either rank or select\n");
        return -1;
    }

    free(input);
}
