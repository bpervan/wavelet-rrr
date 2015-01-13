#include "Utils.h"
#include <stdio.h>
#include <stdlib.h>

/** This function is used for extracting different characters from string
    and storing them in the Dictionary structure */
Dictionary *extractAlphabet (char *input, int length, int *dictLength) {
    int i, j;
    bool exist;
    Dictionary *dictionary = (Dictionary *)calloc (DICTIONARY_SIZE, sizeof (Dictionary));
    *dictLength = 0;

    /** Iterating through the input string and checking if
        the character already exists in the dictionary. If not
        it is added */
    for (i = 0; i < length; ++i) {
        exist = false;
        for (j = 0; j < *dictLength; ++j) {
            if (input[i] == dictionary[j].character) {
                exist = true;
                break;
            }
        }

        if (!exist) {
            dictionary[*dictLength].character = input[i];
            (*dictLength)++;
        }
    }

    /** Setting values for each character in the dictionary.
        First half of the characters have value 0 and the other
        half has 1. */
    for (i = 0; i < *dictLength; ++i) {
        if (i < *dictLength/2) {
            dictionary[i].value = 0;
        } else {
            dictionary[i].value = 1;
        }
    }
    dictionary = realloc (dictionary, *dictLength * sizeof (Dictionary));
    return dictionary;
}

/** Function which is used for creating two dictionaries from one. Those are the
    dictionaries which are passed to left and right child node */
void splitDictionary (Dictionary *dict, Dictionary *leftDict, Dictionary *rightDict,
                      int dictLength, int *leftLength, int *rightLength) {

    int i;
    *leftLength = 0;
    *rightLength = 0;

    /** If the character value is 0 it is added to a left node's dictionary.
        If the character value is 1 it is added to a right node's dictionary. */
    for (i = 0; i < dictLength; ++i) {
        if (dict[i].value == 0 && leftDict != NULL) {
            leftDict[*leftLength].character = dict[i].character;
            *leftLength += 1;
        } else if (dict[i].value == 1 && rightDict != NULL) {
            rightDict[*rightLength].character = dict[i].character;
            *rightLength += 1;
        }
    }

    /** Setting values for the left dictionary */
    for (i = 0; i < *leftLength; ++i) {
        if (i < *leftLength/2) {
            leftDict[i].value = 0;
        } else {
            leftDict[i].value = 1;
        }
    }

    /** Setting values for the right dictionary */
    for (i = 0; i < *rightLength; ++i) {
        if (i < *rightLength/2) {
            rightDict[i].value = 0;
        } else {
            rightDict[i].value = 1;
        }
    }
}

/** Function which returns true or false based on character value in
    the dictionary */
bool getDictionaryValue (Dictionary *dict, int dictLength, char c) {
    int i;

    for (i = 0; i < dictLength; ++i) {
        if (dict[i].character == c) {
            return dict[i].value;
        }
    }

    return 0;
}

/** Function which checks if the character is in dictionary */
bool charInDict (Dictionary *dict, int dictLength, char c) {
    int i;

    for (i = 0; i < dictLength; ++i) {
        if (dict[i].character == c) {
            return true;
        }
    }

    return false;
}
