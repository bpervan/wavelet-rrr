#include "WavletTree.h"

RRRTable *global_table;

int main (int argc, char* argv[]) {

    FILE *inputFile = NULL;
    char* filename, *input;
    int length = 0, block, superblock;
    char c;
    WaveletTree *tree = NULL;

    global_table = (RRRTable *)calloc (1, sizeof(RRRTable));

    /*
    if (argc != 2) {
        printf ("Wrong number of parameters: WavletTree.exe filename expected\n");
        return -1;
    }
    */
    filename = "input.txt";

    inputFile = fopen(filename, "r");

    if (inputFile == NULL) {
        printf ("%s doesn't exist\n", filename);
        return -1;
    }

    input = (char *)calloc (1000000, sizeof(char));

    while ((c = fgetc(inputFile)) != EOF) {
        input[length] = c;
        length++;
    }

    calculateBlockSizes(length, &block, &superblock);
    buildRRRTable(block, superblock);
    RRRTableToString();

    tree = buildWaveletTree(input, length);

    free(input);

    printf ("First rank op: %d\n", rankOperation(tree, 'A', 588));
    printf ("Second rank op: %d\n", rankOperation(tree, 'T', 588));
    printf ("Third rank op: %d\n", rankOperation(tree, 'G', 588));
    printf ("Fourth rank op: %d\n", rankOperation(tree, 'C', 588));

    scanf ("%d", &length);

    return 0;

}

WaveletTree *buildWaveletTree(char* input, int length) {

    int dictLength;
    WaveletTree *tree = (WaveletTree *)calloc (1, sizeof (WaveletTree));
    Dictionary *dict = extractAlphabet (input, length, &dictLength);

    tree->rootNode = buildWaveletNode(input, length, dict, dictLength);

    return tree;

}

WaveletNode *buildWaveletNode (char* input, int length, Dictionary *dict, int dictLength) {

    int i, firstChildLength = 0, secondChildLength = 0, rightDictLength, leftDictLength;
    char *firstChildInput, *secondChildInput, *bitmap;
    WaveletNode *node = (WaveletNode *)calloc (1, sizeof (WaveletNode));
    node->bitmap = (BitMap *)calloc (1, sizeof (BitMap));

    node->dict = dict;
    node->dictLength = dictLength;

    bitmap = (char *)calloc (length/8 + 1, sizeof (char));

    if (dictLength == 2) {

        for (i = 0; i < length; ++i) {
            if (getDictionaryValue(node->dict, node->dictLength, input[i])) {
                bitmap[i/8] |= 0x80 >> (i%8);
            }
        }

        node->bitmap->bm = bitmap;
        node->bitmap->length = length;

        node->rrr = bitmapToRRR(node->bitmap);

        nodeToString(node);

        node->leftChild = NULL;
        node->rightChild = NULL;

        return node;
    } else if (dictLength == 3) {


        secondChildInput = (char *)calloc (1000000, sizeof (char));

        for (i = 0; i < length; ++i) {
            if (getDictionaryValue(node->dict, node->dictLength, input[i])) {
                secondChildInput[secondChildLength] = input[i];
                secondChildLength++;

                bitmap[i/8] |= 0x80 >> (i%8);
            }
        }

        node->bitmap->bm = bitmap;
        node->bitmap->length = length;

        node->rrr = bitmapToRRR(node->bitmap);

        nodeToString(node);

        Dictionary *rightDict = (Dictionary *)calloc (node->dictLength, sizeof (Dictionary));
        splitDictionary(node->dict, NULL, rightDict, node->dictLength, &leftDictLength, &rightDictLength);

        node->leftChild = NULL;
        node->rightChild = buildWaveletNode(secondChildInput, secondChildLength, rightDict, rightDictLength);

        free(secondChildInput);

        return node;

    } else {

         firstChildInput = (char *)calloc (1000000, sizeof (char));
         secondChildInput = (char *)calloc (1000000, sizeof (char));

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

        node->bitmap->bm = bitmap;
        node->bitmap->length = length;

        node->rrr = bitmapToRRR(node->bitmap);

        nodeToString(node);

        Dictionary *rightDict = (Dictionary *)calloc (node->dictLength, sizeof (Dictionary));
        Dictionary *leftDict = (Dictionary *)calloc (node->dictLength, sizeof (Dictionary));

        splitDictionary(node->dict, leftDict, rightDict, node->dictLength, &leftDictLength, &rightDictLength);

        node->leftChild = buildWaveletNode(firstChildInput, firstChildLength, leftDict, leftDictLength);
        node->rightChild = buildWaveletNode(secondChildInput, secondChildLength, rightDict, rightDictLength);

        free(firstChildInput);
        free(secondChildInput);

        return node;
    }


}

Dictionary *extractAlphabet (char *input, int length, int *dictLength) {
    int i, j;
    bool exist;
    Dictionary *dictionary = (Dictionary *)calloc (256, sizeof (Dictionary));

    *dictLength = 0;
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

    for (i = 0; i < *dictLength; ++i) {
        if (i < *dictLength/2) {
            dictionary[i].value = 0;
        } else {
            dictionary[i].value = 1;
        }
    }

    return dictionary;
}

void splitDictionary (Dictionary *dict, Dictionary *leftDict, Dictionary *rightDict,
                      int dictLength, int *leftLength, int *rightLength) {

    int i;
    *leftLength = 0;
    *rightLength = 0;

    for (i = 0; i < dictLength; ++i) {
        if (dict[i].value == 0) {
            printf ("Left length: %d\n", *leftLength);
            leftDict[*leftLength].character = dict[i].character;
            *leftLength += 1;
        } else {
            printf ("Right length: %d\n", *rightLength);
            rightDict[*rightLength].character = dict[i].character;
            *rightLength += 1;
        }
    }

     for (i = 0; i < *leftLength; ++i) {
        if (i < *leftLength/2) {
            leftDict[i].value = 0;
        } else {
            leftDict[i].value = 1;
        }
    }

    for (i = 0; i < *rightLength; ++i) {
        if (i < *rightLength/2) {
            rightDict[i].value = 0;
        } else {
            rightDict[i].value = 1;
        }
    }
}

bool getDictionaryValue (Dictionary *dict, int dictLength, char c) {
    int i;

    for (i = 0; i < dictLength; ++i) {
        if (dict[i].character == c) {
            return dict[i].value;
        }
    }

    return 0;
}

int rankOperation (WaveletTree *tree, char c, int i) {

    int Rank = i;
    WaveletNode *current = tree->rootNode;

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

int popcountRRR (RRRStruct *rrr, bool c, int i) {
    int j, k, Rank, index, class_index, blocks_rem, offset = 0, block, offset_rem;

    index = i/global_table->superblock_size;
    Rank = rrr->superblock_sum[index];

    j = rrr->superblock_offset[index];
    blocks_rem = i - (global_table->superblock_size * index);
    blocks_rem /= global_table->block_size;

    offset_rem = i - (global_table->superblock_size * index);
    offset_rem -= blocks_rem * global_table->block_size;

    printf ("Length: %d, Superblocks: %d, Blocks: %d, Remaining offset: %d\n", i, index, blocks_rem, offset_rem);

    while (blocks_rem > 0) {
        class_index = 0;
        for (k = j; k < j+global_table->class_bm; ++k) {
            class_index = class_index << 1;
            class_index |= (rrr->bitmap->bm[k/8] >> (7 - (k % 8)) & 0x01);
        }
        Rank += class_index;
        j += global_table->class_bm + global_table->entries[class_index].offset_bm;
        blocks_rem--;
    }
    class_index = 0;
    for (k = j; k < j+global_table->class_bm; ++k) {
        class_index = class_index << 1;
        class_index |= (rrr->bitmap->bm[k/8] >> (7 - (k % 8)) & 0x01);
    }

    for (k = j + global_table->class_bm; k < j+global_table->entries[class_index].offset_bm; ++k) {
        offset = offset << 1;
        offset |= (rrr->bitmap->bm[k/8] >> (7 - (k % 8)) & 0x01);
    }

    block = global_table->entries[class_index].offsets[offset];
    block = (block >> (global_table->block_size - offset_rem));
    Rank += popcountInt(block, true, offset_rem);

    if (c) {
        return Rank;
    } else {
        return i - Rank;
    }
}

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

void calculateBlockSizes (int length, int *block, int *superblock) {
    int log2 = 0, length_pom = length;

    while (length_pom != 0) {
        length_pom = (length_pom >> 1);
        log2++;
    }

    *block = log2 / 2;
    *superblock = *block * log2;
}

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
       // printf ("I: %d, Class: %d, Offset_count: %d\n", i, index, global_table->entries[index].offset_count);
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

    char *bitmap = (char *)calloc (1000000, sizeof (char));
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
