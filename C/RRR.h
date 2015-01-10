#ifndef RRR_H
#define RRR_H

#include <stdio.h>
#include <stdlib.h>
#include <stdint.h>
#include "Utils.h"

/** RRRstruct represents RRR structure for Wavelet node.
    It contains bitmap (which will conatain (class_index, offset_index)
    pairs and array of superblocks sums and offsets */
typedef struct {
    BitMap *bitmap;
    int *superblock_offset;
    int *superblock_sum;
} RRRStruct;

/** RRRTableEntry is part of the RRRTable.
    It contains class and list of offsets for that class.
    It also contains a number of bits used to represent
    offset_index */
typedef struct {
    uint8_t RRR_class;
    uint16_t* offsets;
    int offset_count;
    uint8_t offset_bm;
} RRRTableEntry;

/** RRRTable is a table which contains block and superblock sizes.
    It also contains array of table entries (their size is block_size + 1)
    and the number of bits used to represent class_index. */
typedef struct {
    int block_size;
    int superblock_size;
    RRRTableEntry *entries;
    int class_bm;
} RRRTable;

/** Structure used for keeping class -> offsets data */
//RRRTable *global_table;

/** Function which is used for creating RRRTable based on
    block size and superblock size */
RRRTable *buildRRRTable (int block_size, int superblock_size);

void RRRTableToString ();

/** Function which converts regular bitmap to RRR structure */
RRRStruct *bitmapToRRR (BitMap *bm, RRRTable *global_table);

/** Function which calculates popcount (number of bits with value 1) from
    the RRR structure */
int popcountRRR (RRRStruct *rrr, bool c, int i, RRRTable *global_table);

#endif // RRR_H
