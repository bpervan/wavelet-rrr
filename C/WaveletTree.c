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
    tree->rootNode->parent = NULL;

    return tree;
}

/** Function which is used for creating the tree node */
WaveletNode *buildWaveletNode (char* input, int length, Dictionary *dict, int dictLength) {
    int i, firstChildLength = 0, secondChildLength = 0,
            rightDictLength, leftDictLength, block = 0, superblock = 0;
    char *firstChildInput, *secondChildInput, *bitmap;
    WaveletNode *node = (WaveletNode *)calloc (1, sizeof (WaveletNode));
    node->bitmap = (BitMap *)calloc (1, sizeof (BitMap));

    /** Storing dictionary recieved as a parameter */
    node->dict = dict;
    node->dictLength = dictLength;

    /** Bitmap is represented as an array of characters */
    bitmap = (char *)calloc (length/8 + 1, sizeof (char));

     /** Calculating block and superblock sizes based on
        input length */
    calculateBlockSizes(length, &block, &superblock);

    /** Creating the RRR table */
    node->table = buildRRRTable(block, superblock);

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
        node->rrr = bitmapToRRR(node->bitmap, node->table);

        //nodeToString(node);

        /** This node is a leaf - no children nodes*/
        node->leftChild = NULL;
        node->rightChild = NULL;

    /** If node has 3 letters in a dictionary it will have
        only one child node (in this case it will always be
        right child node) */
    } else if (dictLength == 3) {

        secondChildInput = (char *)calloc (length, sizeof (char));

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
        node->rrr = bitmapToRRR(node->bitmap, node->table);

        //nodeToString(node);

        /** Creating dictionaries for children nodes (in this case only right child */
        Dictionary *rightDict = (Dictionary *)calloc (node->dictLength, sizeof (Dictionary));
        splitDictionary(node->dict, NULL, rightDict, node->dictLength, &leftDictLength, &rightDictLength);

        /** Recursive call only on right child */
        node->leftChild = NULL;
        node->rightChild = buildWaveletNode(secondChildInput, secondChildLength, rightDict, rightDictLength);

        node->rightChild->parent = node;
        free(secondChildInput);

    } else {

         firstChildInput = (char *)calloc (length, sizeof (char));
         secondChildInput = (char *)calloc (length, sizeof (char));

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
        node->rrr = bitmapToRRR(node->bitmap, node->table);

        //nodeToString(node);

        /** Creating dictionaries for children nodes (in this case only right child */
        Dictionary *rightDict = (Dictionary *)calloc (node->dictLength, sizeof (Dictionary));
        Dictionary *leftDict = (Dictionary *)calloc (node->dictLength, sizeof (Dictionary));
        splitDictionary(node->dict, leftDict, rightDict, node->dictLength, &leftDictLength, &rightDictLength);

        /** Recursive call on both children nodes */
        node->leftChild = buildWaveletNode(firstChildInput, firstChildLength, leftDict, leftDictLength);
        node->rightChild = buildWaveletNode(secondChildInput, secondChildLength, rightDict, rightDictLength);

        node->leftChild->parent = node;
        node->rightChild->parent = node;

        free(firstChildInput);
        free(secondChildInput);
    }

    return node;


}

/** Function which represents rank operation. Rank (c, i) means
    number of occurances of character c in the first i characters
    of input string */
int rankOperation (WaveletTree *tree, char c, int i) {

    int Rank = i;
    WaveletNode *current = tree->rootNode;

    if (!charInDict(current->dict, current->dictLength, c))
        return 0;

    /** Starting with root node we move down in the hierarchy and caluculate
        popcount which is used in the next popcount operation as a limit.
        The operation is finished when there is no more children nodes */
    while (current != NULL) {
        bool value = getDictionaryValue(current->dict, current->dictLength, c);
        //Rank = popcount (current->bitmap->bm, value, Rank);
        Rank = popcountRRR (current->rrr, value, Rank, current->table);
        if (value) {
            current = current->rightChild;
        } else {
            current = current->leftChild;
        }
    }

    return Rank;
}

int selectOperation (WaveletTree *tree, char c, int i) {
    int Select = i;
    WaveletNode *nextChild, *current;
    nextChild = tree->rootNode;

    /** Checking if the character exists in the dictionary */
    if (!charInDict(tree->rootNode->dict, tree->rootNode->dictLength, c))
        return -1;

    /** Seacrhing for the leaf node with char c */
    do {
        current = nextChild;
        if (getDictionaryValue(current->dict, current->dictLength, c) == 0) {
            nextChild = current->leftChild;
        } else {
            nextChild = current->rightChild;
        }

    } while (nextChild != NULL);

    /** Performing select operation */
    while (current != NULL) {
        bool value = getDictionaryValue(current->dict, current->dictLength, c);
        //Select = selectOnBitmap(current->bitmap->bm, value, Select, current->bitmap->length);
        Select = selectRRR (current->rrr, value, Select, current->table);
        //printf ("Select: %d", Select);
        current = current->parent;
    }

    return Select;
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

/* Bitmap select function */
int selectOnBitmap (char *bitmap, bool c, int i, int length) {
    int j, Rank = 0, Select = 0;

    /* Iterating through string */
    for (j = 0; j < length; ++j) {
	/* Counting 1's or 0's */
        if ((bitmap[j/8] & (0x80 >> (j%8))) != 0x00 && c) {
            Rank++;
        } else if ((bitmap[j/8] & (0x80 >> (j%8))) == 0x00 && !c) {
            Rank++;
        }
        Select++;
	
	/* If boundary reach return result */
        if (Rank == i) {
            return Select;
        }
    }

    return -1;
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
