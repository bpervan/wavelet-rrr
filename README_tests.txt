#Upute za testiranje

U direktoriju se nalaze 3 testne datoteke:
input.fa - duljna genoma: 502
input1.fa - duljina genoma: 4 570 937 - E.Coli
input2.fas - duljina genoma: 5 524 970

Prve dvije FASTA datoteke sastoji se od linije zaglavlja (počinje s >) te genoma. 
Newline znakovi unutar genoma ignoriraju se (abeceda se sastoji od A, T, G i C znakova). 

Zadnja FASTA datoteka sastoji se od 16 znakova (prošireni FASTA)

U direktoriju se nalazi i program za provjeru ispravnosti operacija rank i select:
char_counter.c

koji se pozivom make naredbe prevodi u char_counter

Poziv operacije nad implementacijom stabla valića s RRR strukturom: 
WaveletTree ime_datoteke operacija znak granica

Dakle poziv: WaveletTree input1.fa rank A 238
će iz datoteke input1.fa konstruirati Wavelet stablo te će nad njim pozvati rank ('A', 238)

Na isti način poziva se i program za provjeru:
char_counter input1.fa rank A 238

Napomena: implementacije u Javi, C#-V2, i C++-u umjesto rank i select parametra koriste R ili S
dakle: WaveletTree R A 234 ili WaveletTree S C 23

#Primjeri testova

WaveletTree input.fa rank A 234
Očekivani rezultat: 58

WaveletTree input.fa select C 23
Očekivani rezultat: 107

WaveletTree input.fa rank T 432
Očekivani rezultat: 114

WaveletTree input.fa select G 100
Očekivani rezultat: 389

WaveletTree input1.fa rank G 12345
Očekivani rezultat: 3383

WaveletTree input1.fa select A 12345
Očekivani rezultat: 52553

WaveletTree input1.fa rank C 1234567
Očekivani rezultat: 305484

WaveletTree input1.fa select T 1000000
Očekivani rezultat: 4053864

WaveletTree input2.fas rank T 4000000
Očekivani rezultat: 990024

WaveletTree input2.fas select A 40000
Očekivani rezultat: 167264

WaveletTree input2.fas rank A 543210
Očekivani rezultat: 132287

WaveletTree input2.fas select C 543210
Očekivani rezultat: 2226555

#napomena - rezultati select operacija mogu se razlikovati za -1 jer su neke implementacije
vraćale indeks znaka, a neke broj znakova, uključivo s tim znakom. Isto vrijedi i za rank operacije
samo za +1 zbog razlike [0,i> i [0, i]. Testovi su izvedeni s rank [0,i> i select broj elemenata.











