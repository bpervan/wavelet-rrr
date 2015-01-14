#!/usr/bin/python
import sys
import re
import itertools 
import math

#citamo niz i stvaramo abecedu, neka petlja koja cita znak po 		#znak i uzima razlicite, dijeli po pola (length -1) i desni 1 
	# i lijevi 0 te ta dva niza svaki poziva rekurzivno funkciju
#sad trebam napravit abecedu
def CreateLookUp():
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

def PopCount(Bitmap, boundary, cond):
	counter = 0
	charachters = list(Bitmap)
	
	for calc in range (0, boundary):
		if (charachters[calc] == str(cond)):
			counter = counter + 1
			
	return counter

def CreateRRR(nodekey,preRRR):
	
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
class RRR:
	
	def __init__(self):
		self.RRR = ''
		self.SuperBlockSums = []
		self.SuperBlockOffsets = []			

def MakeBranch():
	
	left = []
	right = []
	return

def InitTree(niz):
		
	words = re.findall('[A-Z]+', niz.upper())	
	unique = list(set(''.join(words)))
	
	key = {}
	data = []
	left = []
	right = []
	
	
		
	#print unique
	limit = len(unique)/2	
	for num in range (0,len(unique)):
		if (num < limit): 
			key[unique[num]] = '0'
		else:
			key[unique[num]] = '1'
		#	print "a"
		#print key
	for ch in list(words[0]):
		data.append(key[ch])
		if (key[ch] == '0'):
			left.append(ch)
		else:
			right.append(ch)	
	#print left
	#print right
	#print key
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
	value = 0
	#for char in 
	if (Rankhelper[0] == '0'):
		keys = stablo['0']		
		localkeys = {}		
		where = keys[char]
		value = PopCount(stablo[1],limit,int(where))
		Rankhelper[0] = str(int(Rankhelper[0])+1)
		if (where == '0'):
			Rankhelper[1] = 'L'
		else:	
			Rankhelper[1] = 'R'
		rank(char,value)
	else:
		for numeror in stablo.keys():
			if (isinstance(numeror,int)):
				splitted = stablo[numeror].split(',')
				if((splitted[1] == Rankhelper[1]) and (splitted[2]==Rankhelper[0])):
					localkeys = stablo[str(splitted[1])+str(int(splitted[2])+1)]
					print localkeys
					where = localkeys[char]
					
					value = PopCount(stablo[numeror],limit,int(where))
					Rankhelper[0] = str(int(Rankhelper[0])+1)
				
	return value


file_object = open('niz.txt', 'r')

niz = file_object.read()

LookUp = {}
CreateLookUp()
#print LookUp
counter = [1,'root',0]
Rankhelper = ['0','']
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
#print uniqueKey
stablo = {}
stablo['0'] = uniqueKey

InitTree(niz)
print stablo
#brojejdinica = PopCount('010101010',4,'1')
#print stablo
#print brojejdinica
rank('A',3)
for numeror in stablo.keys():
	if (isinstance(numeror,int)):
		splitted = stablo[numeror].split(',')
		if any(splitted[0] in s for s in uniqueALL):
			continue
		else:
			CreateRRR(numeror, ''.join(splitted[0]))

#test = CreateRRR(30, '11101101110000010101111110000011111000000')

#print stablo

#print stablo
#select i rank funkcije
#rank ide odzgor, select od dolje
