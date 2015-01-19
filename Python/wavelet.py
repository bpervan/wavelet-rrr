#!/usr/bin/python
import sys
import re
import itertools 
import math

#Funkcija koja kreira LookUptablice za RRR strukturu
def CreateLookUp():
	#ideja funkcije ja da za fiksne vrijednosti stvori tablicu na temelju svih permutacija
	#napravljena je na fiksni nacin radi brzeg izvodnjenja
	helperDict = {}
	BlockSize = 5
	SuperBlockSize = 4 * BlockSize
	Numclasses = BlockSize + 1 	
	for it in range(0,BlockSize+1):
		LookUp[it] = ''
	for it in range(0,int(math.pow(2,5))):
		sidekick = '{0:05b}'.format(it)
		sidecount = PopCount(sidekick, BlockSize, '1')
		LookUp[sidecount] = ''.join(LookUp[sidecount])+','+str(sidekick)
		
	for helper in LookUp.keys():
		helplist = LookUp[helper].split(',')	
		LookUp[helper] = helplist[1:len(helplist)]
	return
#funkcija za prebrojavanje nula ili jedinica nad nizom
def PopCount(Bitmap, boundary, cond):
	#ideja je da iz liste prebrojava znakove i posprema u varijablu koju vraca
	counter = 0
	charachters = list(Bitmap)
	
	for calc in range (0, boundary):
		if (charachters[calc] == str(cond)):
			counter = counter + 1
			
	return counter

#funkcija koja stvara RRR strukturu nad bitmapom
def CreateRRR(nodekey,preRRR):
	
	#funkcija koristi fiksne vrijednosti zbog brzeg izvodjenja
	lenghtsize = len(preRRR) % 5
	BlockSize = 5
	SuperBlockSize = 4 * BlockSize
	if(lenghtsize >= 0):
		for i in range(0, BlockSize - lenghtsize):
			preRRR = preRRR + '0'
	divided = list(preRRR)
	block = []
	superblockoffset = []
	superblocksum = []
	offsetblocks = 0
	sumblocks = 0
	for i in range(0,len(preRRR),5):
		if ((len(block) % 4) == 0):
			superblocksum.append(sumblocks)
			superblockoffset.append(offsetblocks)
		classcount = PopCount(preRRR[i:i+5], BlockSize, '1')
		#print classcount	
		sumblocks += classcount
		classcountthree = '{0:03b}'.format(classcount)
			
		offset = list(LookUp[classcount]).index(preRRR[i:i+5])
		#print superblocksum
		#print superblockoffset		
		offsetSize = int(math.ceil(math.log(len(LookUp[classcount]), 2)))		
		offsetValueString = '{0:0' + str(offsetSize) + 'b}'		
		offsetValue = offsetValueString.format(offset)		
		offsetblocks += 3 + offsetSize	
		block.append(str(classcountthree)+str(offsetValue))
		
		
	instance = RRR()
	instance.RRR = ''.join(block)
	instance.SuperBlockSums = superblocksum
	instance.SuperBlockOffsets = superblockoffset	

	stablo[nodekey] = instance
	return 
#klasa RRR koja predstavlja bitmape RRR strukture
class RRR:
	
	def __init__(self):
		self.RRR = ''
		self.SuperBlockSums = []
		self.SuperBlockOffsets = []			

#funkcija koja rekurzivno stvara stablo valica iz ulaznog niza
def InitTree(niz):
	#pronalazenje unikatnih znakova za abecedu	
	words = re.findall('[A-Z]+', niz.upper())	
	unique = list(set(''.join(words)))
	
	#pomocne vrijednosti za kljuc, pospremanje podataka i za lijevi i desni list koji nastaju
	#pozivom rekurzije
	key = {}
	data = []
	left = []
	right = []
	
	#odredimo koji znakovi primaju koju vrijednost te tako slozimo pomocni niz
	limit = len(unique)/2	
	for num in range (0,len(unique)):
		if (num < limit): 
			key[unique[num]] = '0'
		else:
			key[unique[num]] = '1'
		
	for ch in list(words[0]):
		data.append(key[ch])
		if (key[ch] == '0'):
			left.append(ch)
		else:
			right.append(ch)	
	#print left
	#print right
	#print key
	#provjeravamo je li ostao samo jedan znak i ako nije stvaramo novu podjelu 
	#i idemo dalje u rekuzriju, u suprotnoom pospremamo znak 
	#dodatne oznake su za stranu na koju pospremamo sto je ekvivalent nuli za desnu i
	# jedinica za lijevu stranu te dodatno pospremamo na kojoj razini je podatak
	if (len(unique) > 1):
		stablo[counter[0]] = ''.join(data) +','+ counter[1] +','+ str(counter[2])
		stablo[str(counter[1]) + str(counter[2])] = key 
		counter[0] = counter[0] + 1
			
		if(len(left) > 1):
			counter[1] = 'L'
			counter[2] = counter [2] + 1	
			#stablo[str(counter[1]) + str(counter[2])] = key
			InitTree(''.join(left))
				
			counter[2] = counter[2] - 1
			
			#stablo[str(counter[1]) + str(counter[2])] = key
		#i = i +	1
		if(len(right) > 1):
			counter[1] = 'R'	
			counter[2] = counter [2] + 1	
			
			InitTree(''.join(right))
			
			counter[2] = counter[2] - 1
			#stablo[str(counter[1]) + str(counter[2])] = key	
		#i = i +1	
	else:	
		
		stablo[counter[0]] = ''.join(unique) +','+ counter[1] +','+ str(counter[2])
		counter[0] = counter[0] + 1
			
	
	
	
	return

def rank(char, limit):

	unsorted = []
	sortedlist = [] 
	if (Rankhelper[0] == '0'):
		keys = stablo['0']		
		localkeys = {}		
		where = keys[char]
		Rankhelper[2] = PopCount(stablo[1],limit,int(where))
		Rankhelper[0] = str(int(Rankhelper[0])+1)
		if (where == '0'):
			Rankhelper[1] = 'L'
		else:	
			Rankhelper[1] = 'R'
		rank(char,Rankhelper[2])
	else:
		for numeror in stablo.keys():
			if (isinstance(numeror,int)):
				unsorted.append(numeror)
		print unsorted
		unsorted.sort()
		
		for number in range(0,len(unsorted)):
			numeror = unsorted[number]
			if (isinstance(numeror,int)):
			
				splitted = stablo[numeror].split(',')
				if((splitted[1] == Rankhelper[1]) and (splitted[2]==Rankhelper[0])):
					if any(str(splitted[0]) in s for s in uniqueKey):
						break
					localkeys = stablo[str(splitted[1])+str(splitted[2])]
					print localkeys
					where = localkeys[char]
					
					Rankhelper[2] = PopCount(stablo[numeror],limit,int(where))
					#Rankhelper[0] = str(int(Rankhelper[0])+1)
				
	return

#main kod
#citanje niza
nizovi = [line.strip(' \t\n\r') for line in open(sys.argv[1])]

niz = ''.join(nizovi)

#poziv funkcije za stvaranje LookUp tablica za RRR klase
LookUp = {}
CreateLookUp()

#pomocne varijable
counter = [1,'root',0]
Rankhelper = ['0','',0]
Selecthelper = ['0']

uniqueKey = {}
wordsALL = re.findall('[A-Z]+', niz.upper())	
uniqueALL = list(set(''.join(wordsALL)))


delimit = len(uniqueALL)/2	
for num in range (0,len(uniqueALL)):
	if (num < delimit): 
		uniqueKey[uniqueALL[num]] = '0'
	else:
		uniqueKey[uniqueALL[num]] = '1'

#Ideja zapisa stabla valica je jedan kompleksan dictionary u kojem su zapisani s integer kljucevima
#cvorovi stabla te s kljucevima koji su kombinacija razine i strane abecede za odredjenu razinu

stablo = {}
stablo['0'] = uniqueKey

#poziv i inijcijalizacija stabla valica
InitTree(niz)
print stablo


#stvaranje RRR strukture nad stablom valica
rank(sys.argv[2],int(sys.argv[3]))
print Rankhelper[2]
Rankhelper[0] = '0'
for numeror in stablo.keys():
	if (isinstance(numeror,int)):
		splitted = stablo[numeror].split(',')
		if any(splitted[0] in s for s in uniqueALL):
			continue
		else:
			CreateRRR(numeror, ''.join(splitted[0]))
			continue
#print stablo
#('T',10)
#test = CreateRRR(30, '11101101110000010101111110000011111000000')

