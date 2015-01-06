#ifndef RRR_H
#define RRR_H

#include <stdio.h>
#include <stdlib.h>
#include "Utils.h"

typedef struct {
    BitMap *bitmap;
    int *superblock_offset;
    int *superblock_sum;
} RRRStruct;

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

/** Structure used for keeping class -> offsets data */
RRRTable *global_table;

void buildRRRTable (int block_size, int superblock_size);

void RRRTableToString ();

RRRStruct *bitmapToRRR (BitMap *bm);

#endif // RRR_H
