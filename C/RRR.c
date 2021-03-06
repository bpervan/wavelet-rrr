#include "RRR.h"

/** Function which is used for creating RRRTable based on
    block size and superblock size */
RRRTable *buildRRRTable (int block_size, int superblock_size) {
    int length = 1, i, index, j, class_bm = 0;
    RRRTable *global_table = (RRRTable *)calloc (1, sizeof(RRRTable));

    /** Initialization */
    global_table->entries = (RRRTableEntry *) calloc (block_size + 1, sizeof (RRRTableEntry));

    for (i = 0; i < block_size + 1; ++i) {
        global_table->entries[i].offset_count = 0;
        global_table->entries[i].RRR_class = i;
        global_table->entries[i].offsets = (uint16_t *)calloc(1, sizeof(uint16_t));
    }

    global_table->block_size = block_size;
    global_table->superblock_size = superblock_size;

    /** Calculating number of bits needed to represent class */
    j = global_table->block_size;
    do {
        class_bm++;
        j = j >> 1;
    } while (j != 0);

    global_table->class_bm = class_bm;
    length = (length << (block_size));

    /** Iterating from 0 to 2^block_size which is the
        highest number needed to create all possible blocks */
    for (i = 0; i < length; ++i) {
        index = popcountInt(i, true, block_size);

        /** Storing blocks (offsets) based on their popcount (class) */
        global_table->entries[index].offsets[global_table->entries[index].offset_count] = i;
        global_table->entries[index].offset_count+=1;
        global_table->entries[index].offsets =
                    realloc (global_table->entries[index].offsets,
                    (global_table->entries[index].offset_count + 1)*sizeof(uint16_t));
    }

    /** Calculating number of bits needed to store and
        offset for each table entry (class)*/
    for (i = 0; i < block_size + 1; ++i) {
        int j = global_table->entries[i].offset_count;
        int offset_bm = 0;
        do {
            offset_bm++;
            j = (j >> 1);
        } while (j != 0);
        global_table->entries[i].offset_bm = offset_bm;

    }

    return global_table;
}

/** Function which converts regular bitmap to RRR structure */
RRRStruct *bitmapToRRR (BitMap *bm, RRRTable *global_table) {
    int length = 0, i, j, block, index, offset = 0, superblock_sum = 0;

    /** Initialization */
    RRRStruct *rrr = (RRRStruct *)calloc (1, sizeof(RRRStruct));
    rrr->bitmap = (BitMap *) calloc (1, sizeof (BitMap));
    char *bitmap = (char *)calloc (MEMORY_SIZE, sizeof (char));
    rrr->superblock_sum = (int *) calloc (bm->length / global_table->superblock_size + 1, sizeof(int));
    rrr->superblock_offset = (int *) calloc (bm->length / global_table->superblock_size + 1, sizeof(int));
    rrr->superblock_num = 0;

    /** Iterating over bitmap, stepping over blocks */
    for (i = 0; i < bm->length; i+=global_table->block_size) {
        block = 0;

        /** If the superblock is reached, store sum and offset */
        if (i % global_table->superblock_size == 0) {
            rrr->superblock_sum[i/global_table->superblock_size] = superblock_sum;
            rrr->superblock_offset[i/global_table->superblock_size] = length;
            rrr->superblock_num++;
        }

        /** Storing block to an auxiliry variable */
        for (j = i; j < i + global_table->block_size; ++j) {
            block = block << 1;
            block |= ((bm->bm[j/8] >> (7-(j%8))) & 0x01);
        }

        /** Calculating popcount of that block, the result gives a class */
        index = popcountInt(block, true, global_table->block_size);
        superblock_sum += index;

        /** Getting the offset index by iterating through a table entry
            with class = index */
        for (j = 0; j < global_table->entries[index].offset_count; ++j) {
            if (global_table->entries[index].offsets[j] == block) {
                offset = j;
                break;
            }
        }

        /** Putting the class index into the RRR structure bitmap */
        for (j = 0; j < global_table->class_bm; ++j) {
            bitmap[(length+j)/8] |= ((index >> (global_table->class_bm - 1 - j) & 0x01) << (7-((length+j)%8)));
        }

        length += global_table->class_bm;

        /** Putting the offset index into the RRR structure bitmap */
        for (j = 0; j < global_table->entries[index].offset_bm; ++j) {
            bitmap[(length+j)/8] |= ((offset >> (global_table->entries[index].offset_bm - 1 - j) & 0x01)
                                        << (7-((length+j)%8)));
        }

        length += global_table->entries[index].offset_bm;
    }

    bitmap = (char *)realloc (bitmap, length * sizeof(char));

    /** Setting the bitmap and its length into RRR structure */
    rrr->bitmap->bm = bitmap;
    rrr->bitmap->length = length;

    return rrr;

}

/** Function which calculates popcount (number of bits with value 1) from
    the RRR structure */
int popcountRRR (RRRStruct *rrr, bool c, int i, RRRTable *global_table) {
    int j, k, Rank, index, class_index, blocks_rem, offset = 0, block, offset_rem;

    /** Calculating starting superblock and adding
        previous superblocks' sum */
    index = i/global_table->superblock_size;
    Rank = rrr->superblock_sum[index];
    j = rrr->superblock_offset[index];

    //printf ("Rank after supreblocks: %d\n", Rank);

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
        //printf ("Rank after block %d: %d\n", blocks_rem,  Rank);
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

/** Performs select operation on RRR bitmap */
int selectRRR (RRRStruct *rrr, bool c, int i, RRRTable *global_table) {
    int superblock_index, j, Select, sum = 0, index, class_index, block_sum, k, block;
    int bit = 0, offset = 0, superblock_sum;

    /** Searching for starting superblock */
    for (j = 0; j < rrr->superblock_num; ++j) {
        if (c) {
            superblock_sum = rrr->superblock_sum[j];
        } else {
            superblock_sum = j * global_table->superblock_size - rrr->superblock_sum[j];
        }

        if (sum + superblock_sum > i || j == rrr->superblock_num - 1) {
            index = rrr->superblock_offset[j-1];
            Select = (j - 1)* global_table->superblock_size;
            break;
        } else {
            sum = superblock_sum;
        }
    }

    /** Iterating through blocks */
    while (1) {
        class_index = 0;

        if (index > rrr->bitmap->length) return -1;

        /** Getting class index from RRR block */
        for (k = index; k < index+global_table->class_bm; ++k) {
            class_index = class_index << 1;
            class_index |= (rrr->bitmap->bm[k/8] >> (7 - (k % 8)) & 0x01);
        }

        /** Calcuclating block sum */
        if (c) {
            block_sum = class_index;
        } else {
            block_sum = global_table->block_size - class_index;
        }

        /** If there ar still more blocks add to Select and sum */
        if (block_sum + sum < i) {
            Select += global_table->block_size;
            index += global_table->class_bm + global_table->entries[class_index].offset_bm;
            sum += block_sum;
        } else {
            break;
        }

    }

    class_index = 0;

    /** Getting class index from RRR block */
    for (k = index; k < index+global_table->class_bm && k < rrr->bitmap->length; ++k) {
        class_index = class_index << 1;
        class_index |= (rrr->bitmap->bm[k/8] >> (7 - (k % 8)) & 0x01);
    }

    /** Getting offset index from the last block */
    for (k = index + global_table->class_bm; k < index+ global_table->class_bm +
            global_table->entries[class_index].offset_bm && k < rrr->bitmap->length; ++k) {
        offset = offset << 1;
        offset |= ((rrr->bitmap->bm[k/8] >> (7 - (k % 8))) & 0x01);
    }

     /** Getting the last block */
    block = global_table->entries[class_index].offsets[offset];
    for (k = global_table->block_size - 1; k >= 0; --k) {
        if (sum == i) break;
        Select++;
        bit = ((block >> k) & 0x01);
        if ((c & bit == 1) || (!c & bit == 0)) {
            sum++;
        }
    }

    return Select;

}

/** Debug function */
void RRRTableToString (RRRTable *global_table){
    int i, j;

    printf ("Block size: %d\n", global_table->block_size);
    printf ("Superblock size: %d\n", global_table->superblock_size);
    printf ("Class bm: %d\n", global_table->class_bm);

    for (i = 0; i < global_table->block_size + 1; ++i) {
        printf ("\n");
        printf ("Class: %d\n", global_table->entries[i].RRR_class);
        printf ("Offset_bm: %d\n", global_table->entries[i].offset_bm);
        printf ("Offsets: ");
        for (j = 0; j < global_table->entries[i].offset_count; ++j) {
            printf ("0x%04x, ", global_table->entries[i].offsets[j]);
        }
        printf ("\n");
    }

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
    if (*block == 0) *block = 1;
    *superblock = *block * log2;
}

