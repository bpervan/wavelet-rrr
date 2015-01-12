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

/** This function is used for extracting different characters from string
    and storing them in the Dictionary structure */
Dictionary *extractAlphabet (char *input, int length, int *dictLength);

/** Function which returns true or false based on character value in
    the dictionary */
bool getDictionaryValue (Dictionary *dict, int dictLength, char c);

/** Function which checks if the character is in dictionary */
bool charInDict (Dictionary *dict, int dictLength, char c);

/** Function which is used for creating two dictionaries from one. Those are the
    dictionaries which are passed to left and right child node */
void splitDictionary (Dictionary *dict, Dictionary *leftDict, Dictionary *rightDict,
                      int dictLength, int *leftLength, int *rightLength);


#endif // UTILS_H
