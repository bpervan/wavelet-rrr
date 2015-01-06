#ifndef WAVELETTREE_H
#define WAVELETTREE_H

#include <stdio.h>
#include <stdlib.h>
#include "RRR.h"
#include "Utils.h"

struct WaveletNode {
    RRRStruct *rrr;
    BitMap *bitmap;
    Dictionary* dict;
    int dictLength;
    struct WaveletNode *leftChild;
    struct WaveletNode *rightChild;
};

typedef struct WaveletNode WaveletNode;

typedef struct  {
    WaveletNode *rootNode;
} WaveletTree;

WaveletTree *buildWaveletTree (char *input, int length);

WaveletNode *buildWaveletNode (char *input, int length, Dictionary *dict, int dictLenght);

Dictionary *extractAlphabet (char *input, int length, int *dictLength);

bool getDictionaryValue (Dictionary *dict, int dictLength, char c);

int rankOperation (WaveletTree *tree, char c, int i);

int popcount (char *bitmap, bool c, int i);

int popcountInt (int bitmap, bool c, int i);

void nodeToString(WaveletNode *node);

void splitDictionary (Dictionary *dict, Dictionary *leftDict, Dictionary *rightDict,
                      int dictLength, int *leftLength, int *rightLength);

void calculateBlockSizes (int length, int *block, int *superblock);

#endif // WAVELETTREE_H
