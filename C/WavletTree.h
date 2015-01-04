#include <stdio.h>
#include <stdlib.h>

typedef enum {false, true} bool;

typedef struct {
    char character;
    bool value;
} Dictionary;

typedef struct {
    char *bm;
    int length;
} BitMap;

typedef struct {
    BitMap *bitmap;
    int *superblock_offset;
    int *superblock_sum;
} RRRStruct;

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

typedef struct {
    int RRR_class;
    int* offsets;
    int offset_count;
    int offset_bm;
} RRRTableEntry;

typedef struct {
    int block_size;
    int superblock_size;
    RRRTableEntry *entries;
    int class_bm;
} RRRTable;

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

void buildRRRTable (int block_size, int superblock_size);

void RRRTableToString ();

RRRStruct *bitmapToRRR (BitMap *bm);



