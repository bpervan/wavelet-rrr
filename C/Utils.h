#ifndef UTILS_H
#define UTILS_H

/** Memory size is a max length of the
    input string and dictonary size is a
    max different characters allowed */
#define MEMORY_SIZE 10000000
#define DICTIONARY_SIZE 256

/** boolean type */
typedef enum {false, true} bool;

/** Dictionary is a strcuture used by WavletNode. It
    contains a character and its value in that node */
typedef struct {
    char character;
    bool value;
} Dictionary;

/** Bitmap structure represents bitmap. It conatins
    char array and its length */
typedef struct {
    char *bm;
    int length;
} BitMap;

#endif // UTILS_H
