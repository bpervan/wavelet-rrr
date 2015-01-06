#include "WaveletTree.h"

/** This function is used for building the wavelet tree.
    It takes input string and it's length as parameters. */
WaveletTree *buildWaveletTree(char* input, int length) {
    int dictLength;
    WaveletTree *tree = (WaveletTree *)calloc (1, sizeof (WaveletTree));

    /** Extracting the alphabet from the input and creating
        a 'dictionary' */
    Dictionary *dict = extractAlphabet (input, length, &dictLength);

    /** Calling the recursive function which creates tree nodes */
    tree->rootNode = buildWaveletNode(input, length, dict, dictLength);

    return tree;
}

/** Function which is used for creating the tree node */
WaveletNode *buildWaveletNode (char* input, int length, Dictionary *dict, int dictLength) {
    int i, firstChildLength = 0, secondChildLength = 0, rightDictLength, leftDictLength;
    char *firstChildInput, *secondChildInput, *bitmap;
    WaveletNode *node = (WaveletNode *)calloc (1, sizeof (WaveletNode));
    node->bitmap = (BitMap *)calloc (1, sizeof (BitMap));

    /** Storing dictionary recieved as a parameter */
    node->dict = dict;
    node->dictLength = dictLength;

    /** Bitmap is represented as an array of characters */
    bitmap = (char *)calloc (length/8 + 1, sizeof (char));

    /** If there are only 2 letters in the dictionary this node
        is a leaf */
    if (dictLength == 2) {

        /** Creating bitmap from input string based on letter value
            stred in dictionary (0 or 1) */
        for (i = 0; i < length; ++i) {
            if (getDictionaryValue(node->dict, node->dictLength, input[i])) {
                bitmap[i/8] |= 0x80 >> (i%8);
            }
        }

        /** Storing bitmap */
        node->bitmap->bm = bitmap;
        node->bitmap->length = length;

        /** Creating RRR structure from bitmap */
        node->rrr = bitmapToRRR(node->bitmap);

        //nodeToString(node);

        /** This node is a leaf - no children nodes*/
        node->leftChild = NULL;
        node->rightChild = NULL;

    /** If node has 3 letters in a dictionary it will have
        only one child node (in this case it will always be
        right child node) */
    } else if (dictLength == 3) {

        secondChildInput = (char *)calloc (MEMORY_SIZE, sizeof (char));

        /** Input string to bitmap, and also storing the letters with
            value 1 as input string for the right child node */
        for (i = 0; i < length; ++i) {
            if (getDictionaryValue(node->dict, node->dictLength, input[i])) {
                secondChildInput[secondChildLength] = input[i];
                secondChildLength++;

                bitmap[i/8] |= 0x80 >> (i%8);
            }
        }

        /** Storing the bitmap */
        node->bitmap->bm = bitmap;
        node->bitmap->length = length;

        /** Creating RRR structure from bitmap */
        node->rrr = bitmapToRRR(node->bitmap);

        //nodeToString(node);

        /** Creating dictionaries for children nodes (in this case only right child */
        Dictionary *rightDict = (Dictionary *)calloc (node->dictLength, sizeof (Dictionary));
        splitDictionary(node->dict, NULL, rightDict, node->dictLength, &leftDictLength, &rightDictLength);

        /** Recursive call only on right child */
        node->leftChild = NULL;
        node->rightChild = buildWaveletNode(secondChildInput, secondChildLength, rightDict, rightDictLength);

        free(secondChildInput);

    } else {

         firstChildInput = (char *)calloc (MEMORY_SIZE, sizeof (char));
         secondChildInput = (char *)calloc (MEMORY_SIZE, sizeof (char));

         /** Input string to bitmap, and also storing the letters with
            values 0 and 1 as input string for the left and right child nodes */
        for (i = 0; i < length; ++i) {
            if (getDictionaryValue(node->dict, node->dictLength, input[i])) {
                secondChildInput[secondChildLength] = input[i];
                secondChildLength++;

                bitmap[i/8] |= 0x80 >> (i%8);
            } else {
                firstChildInput[firstChildLength] = input[i];
                firstChildLength++;
            }
        }

        /** Storing the bitmap */
        node->bitmap->bm = bitmap;
        node->bitmap->length = length;

         /** Creating RRR structure from bitmap */
        node->rrr = bitmapToRRR(node->bitmap);

        //nodeToString(node);

        /** Creating dictionaries for children nodes (in this case only right child */
        Dictionary *rightDict = (Dictionary *)calloc (node->dictLength, sizeof (Dictionary));
        Dictionary *leftDict = (Dictionary *)calloc (node->dictLength, sizeof (Dictionary));
        splitDictionary(node->dict, leftDict, rightDict, node->dictLength, &leftDictLength, &rightDictLength);

        /** Recursive call on both children nodes */
        node->leftChild = buildWaveletNode(firstChildInput, firstChildLength, leftDict, leftDictLength);
        node->rightChild = buildWaveletNode(secondChildInput, secondChildLength, rightDict, rightDictLength);

        free(firstChildInput);
        free(secondChildInput);
    }

    return node;


}

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

/** Function which represents rank operation. Rank (c, i) means
    number of occurances of character c in the first i characters
    of input string */
int rankOperation (WaveletTree *tree, char c, int i) {

    int Rank = i;
    WaveletNode *current = tree->rootNode;

    /** Starting with root node we move down in the hierarchy and caluculate
        popcount which is used in the next popcount operation as a limit.
        The operation is finished when there is no more children nodes */
    while (current != NULL) {
        //Rank = popcount (current->bitmap->bm, getDictionaryValue(current->dict,current->dictLength,c), Rank);
        Rank = popcountRRR (current->rrr, getDictionaryValue(current->dict,current->dictLength,c), Rank);
        if (getDictionaryValue(current->dict, current->dictLength, c)) {
            current = current->rightChild;
        } else {
            current = current->leftChild;
        }
    }

    return Rank;
}

/** Function which calculates popcount (number of bits with value 1) from
    the RRR structure */
int popcountRRR (RRRStruct *rrr, bool c, int i) {
    int j, k, Rank, index, class_index, blocks_rem, offset = 0, block, offset_rem;

    /** Calculating starting superblock and adding
        previous superblocks' sum */
    index = i/global_table->superblock_size;
    Rank = rrr->superblock_sum[index];
    j = rrr->superblock_offset[index];

    /** Calculating number of blocks we need to iterate through */
    blocks_rem = i - (global_table->superblock_size * index);
    blocks_rem /= global_table->block_size;

    /** Calculating the number of bits in last block we need
        to iterate through */
    offset_rem = i - (global_table->superblock_size * index);
    offset_rem -= blocks_rem * global_table->block_size;

    /** Iterating through blocks and adding their value
        to sum (Rank) */
    while (blocks_rem > 0) {
        class_index = 0;

        /** Getting class index from RRR block */
        for (k = j; k < j+global_table->class_bm; ++k) {
            class_index = class_index << 1;
            class_index |= (rrr->bitmap->bm[k/8] >> (7 - (k % 8)) & 0x01);
        }

        /** Class index is popcount for that block so we add it to overall sum */
        Rank += class_index;
        j += global_table->class_bm + global_table->entries[class_index].offset_bm;
        blocks_rem--;
    }

    class_index = 0;

    /** Getting class index from the lest block */
    for (k = j; k < j+global_table->class_bm; ++k) {
        class_index = class_index << 1;
        class_index |= (rrr->bitmap->bm[k/8] >> (7 - (k % 8)) & 0x01);
    }

    /** Getting offset index from the last block */
    for (k = j + global_table->class_bm; k < j+ global_table->class_bm +global_table->entries[class_index].offset_bm; ++k) {
        offset = offset << 1;
        offset |= ((rrr->bitmap->bm[k/8] >> (7 - (k % 8))) & 0x01);
    }

    /** Getting the last block and calculating popcount */
    block = global_table->entries[class_index].offsets[offset];
    block = (block >> (global_table->block_size - offset_rem));
    Rank += popcountInt(block, true, offset_rem);

    /** If c is true we are counting 1's and Rank is returned,
        if c is false we are counting 0's and i - Rank is returned */
    if (c) {
        return Rank;
    } else {
        return i - Rank;
    }
}

/** Popcount used for counting 1's in bitmap from 0 to i (bitmap is char array) */
int popcount(char *bitmap, bool c, int i) {
    int j, Rank = 0;

    for (j = 0; j < i; ++j) {
        if ((bitmap[j/8] & (0x80 >> (j%8))) != 0x00) {
            Rank++;
        }
    }

    if (c) {
        return Rank;
    } else {
        return i - Rank;
    }
}

/** Popcount used for counting 1's in bitmap from 0 to i (bitmap is int)*/
int popcountInt(int bitmap, bool c, int i) {
    int j, Rank = 0;

    for (j = 0; j < i; ++j) {
        if ((bitmap >> j) & 0x01) {
            Rank++;
        }
    }

    if (c) {
        return Rank;
    } else {
        return i - Rank;
    }
}

/** Debug function */
void nodeToString (WaveletNode *node) {
    int i;

    printf ("Bitmap: ");
    for (i = 0; i < node->bitmap->length/8 + 1; ++i) {
        printf ("0x%02x ", node->bitmap->bm[i] & 0xff);
    }

    printf ("\n");

    printf ("RRR bitmap: ");
    for (i = 0; i < node->rrr->bitmap->length/8 + 1; ++i) {
        printf ("0x%02x ", node->rrr->bitmap->bm[i] & 0xff);
    }

    printf ("\n");

    for (i = 0; i < node->dictLength; ++i) {
        printf ("Dict %c : %d\n", node->dict[i].character, node->dict[i].value);
    }

    printf ("\n");
}

/** Function which calculates block's and superblock's sizes based on
    input string length */
void calculateBlockSizes (int length, int *block, int *superblock) {
    int log2 = 0, length_pom = length;

    while (length_pom != 0) {
        length_pom = (length_pom >> 1);
        log2++;
    }

    *block = log2 / 2;
    *superblock = *block * log2;
}
