#include "RRR.h"

/** Function which is used for creating RRRTable based on
    block size and superblock size */
void buildRRRTable (int block_size, int superblock_size) {
    int length = 1, i, index, j, class_bm = 0;

    /** Initialization */
    global_table->entries = (RRRTableEntry *) calloc (block_size + 1, sizeof (RRRTableEntry));

    for (i = 0; i < block_size + 1; ++i) {
        global_table->entries[i].offset_count = 0;
        global_table->entries[i].RRR_class = i;
        global_table->entries[i].offsets = (int *)calloc(1, sizeof(int));
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
                    (global_table->entries[index].offset_count + 1)*sizeof(int));
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
}

/** Function which converts regular bitmap to RRR structure */
RRRStruct *bitmapToRRR (BitMap *bm) {
    int length = 0, i, j, block, index, offset = 0, superblock_sum = 0;

    /** Initialization */
    RRRStruct *rrr = (RRRStruct *)calloc (1, sizeof(RRRStruct));
    rrr->bitmap = (BitMap *) calloc (1, sizeof (BitMap));
    char *bitmap = (char *)calloc (MEMORY_SIZE, sizeof (char));
    rrr->superblock_sum = (int *) calloc (bm->length / global_table->superblock_size + 1, sizeof(int));
    rrr->superblock_offset = (int *) calloc (bm->length / global_table->superblock_size + 1, sizeof(int));

    /** Iterating over bitmap, stepping over blocks */
    for (i = 0; i < bm->length; i+=global_table->block_size) {
        block = 0;

        /** If the superblock is reached, store sum and offset */
        if (i % global_table->superblock_size == 0) {
            rrr->superblock_sum[i/global_table->superblock_size] = superblock_sum;
            rrr->superblock_offset[i/global_table->superblock_size] = length;
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

    /** Setting the bitmap and its length into RRR structure */
    rrr->bitmap->bm = bitmap;
    rrr->bitmap->length = length;

    return rrr;

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

void RRRTableToString (){
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

