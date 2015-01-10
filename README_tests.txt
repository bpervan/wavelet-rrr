#Upute za testiranje
U direktoriju se nalaze 3 testne datoteke:
input.txt - duljina: 138
input.fa - duljna genoma: 502
input1.fa - duljina genoma: 4 570 937
input2.fas - duljina genoma: 5 524 970

Svaka FASTA datoteka sastoji se od linije zaglavlja (počinje s >) te genoma. 
Newline znakovi unutar genoma ignoriraju se (abeceda se sastoji od A, T, G i C znakova). 

Tekstualna datoteka sadrži razne znakove (veličina abecede je 16).

U direktoriju se nalazi i program za provjeru ispravnosti operacije rank:
char_counter.c

koji se pozivom make naredbe prevodi u char_counter.exe

Poziv rank operacije nad implementacijom stabla valića s RRR strukturom: 
WaveletTree.exe ime_datoteke znak granica

Dakle poziv: WaveletTree.exe input1.fa A 238
će iz datoteke input1.fa konstruirati Wavelet stablo te će nad njim pozvati rank ('A', 238)

Na isti način poziva se i program za provjeru:
char_counter.exe input1.fa A 238

#Primjeri testova
WaveletTree.exe input.txt t 100
Očekivani rezultat: 9

WaveletTree.exe input.txt e 138
Očekivani rezultat: 10

WaveletTree.exe input.txt w 20
Očekivani rezultat: 2

WaveletTree.exe input.fa A 234
Očekivani rezultat: 58

WaveletTree.exe input.fa C 432
Očekivani rezultat: 92

WaveletTree.exe input.fa T 432
Očekivani rezultat: 114

WaveletTree.exe input1.fa G 12345
Očekivani rezultat: 3383

WaveletTree.exe input1.fa A 123456
Očekivani rezultat: 30103

WaveletTree.exe input1.fa C 1234567
Očekivani rezultat: 305484

WaveletTree.exe input2.fas t 4000000
Očekivani rezultat: 990024

WaveletTree.exe input2.fas g 5500000
Očekivani rezultat: 1384977

WaveletTree.exe input2.fas a 543210
Očekivani rezultat: 132287











