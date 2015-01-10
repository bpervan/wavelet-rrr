#include <stdio.h>
#include <stdlib.h>
#include <sys/time.h>

int main (int argc, char* argv[]) {

    FILE *inputFile = NULL;
    char* filename, *input;
    int length = 0, bound, Rank, i;
    char c, s_char;
    int header_line = 0;

    //global_table = (RRRTable *)calloc (1, sizeof(RRRTable));

    if (argc != 4) {
        printf ("Wrong number of parameters: char_counter.exe filename char bound expected\n");
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
    printf ("Expected Rank value is %d\n", Rank);

    free(input);
}
