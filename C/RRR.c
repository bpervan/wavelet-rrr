#include "RRR.h"

void buildRRRTable (int block_size, int superblock_size) {
    int length = 1, i, index, j, class_bm = 0;

    global_table->entries = (RRRTableEntry *) calloc (block_size + 1, sizeof (RRRTableEntry));

    for (i = 0; i < block_size + 1; ++i) {
        global_table->entries[i].offset_count = 0;
        global_table->entries[i].RRR_class = i;
        global_table->entries[i].offsets = (int *)calloc(1, sizeof(int));
    }

    global_table->block_size = block_size;
    global_table->superblock_size = superblock_size;

    j = global_table->block_size;
    do {
        class_bm++;
        j = j >> 1;
    } while (j != 0);

    global_table->class_bm = class_bm;
    length = (length << (block_size));

    for (i = 0; i < length; ++i) {
        index = popcountInt(i, true, block_size);
        //printf ("I: %d, Class: %d, Offset_count: %d\n", i, index, global_table->entries[index].offset_count);
        global_table->entries[index].offsets[global_table->entries[index].offset_count] = i;
        global_table->entries[index].offset_count+=1;
        global_table->entries[index].offsets = realloc (global_table->entries[index].offsets, (global_table->entries[index].offset_count + 1)*sizeof(int));
    }

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

RRRStruct *bitmapToRRR (BitMap *bm) {
    int length = 0, i, j, block, index, offset = 0, superblock_sum = 0;

    RRRStruct *rrr = (RRRStruct *)calloc (1, sizeof(RRRStruct));
    rrr->bitmap = (BitMap *) calloc (1, sizeof (BitMap));

    char *bitmap = (char *)calloc (MEMORY_SIZE, sizeof (char));
    rrr->superblock_sum = (int *) calloc (bm->length / global_table->superblock_size + 1, sizeof(int));
    rrr->superblock_offset = (int *) calloc (bm->length / global_table->superblock_size + 1, sizeof(int));

    for (i = 0; i < bm->length; i+=global_table->block_size) {
        block = 0;

        if (i % global_table->superblock_size == 0) {
            rrr->superblock_sum[i/global_table->superblock_size] = superblock_sum;
            rrr->superblock_offset[i/global_table->superblock_size] = length;
        }

        for (j = i; j < i + global_table->block_size; ++j) {
            block = block << 1;
            block |= ((bm->bm[j/8] >> (7-(j%8))) & 0x01);
        }

        index = popcountInt(block, true, global_table->block_size);
        superblock_sum += index;

        for (j = 0; j < global_table->entries[index].offset_count; ++j) {
            if (global_table->entries[index].offsets[j] == block) {
                offset = j;
                break;
            }
        }

        for (j = 0; j < global_table->class_bm; ++j) {
            bitmap[(length+j)/8] |= ((index >> (global_table->class_bm - 1 - j) & 0x01) << (7-((length+j)%8)));
        }

        length += global_table->class_bm;

        for (j = 0; j < global_table->entries[index].offset_bm; ++j) {
            bitmap[(length+j)/8] |= ((offset >> (global_table->entries[index].offset_bm - 1 - j) & 0x01) << (7-((length+j)%8)));
        }

        length += global_table->entries[index].offset_bm;
    }

    rrr->bitmap->bm = bitmap;
    rrr->bitmap->length = length;

    return rrr;

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

