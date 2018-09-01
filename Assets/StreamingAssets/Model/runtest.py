#!/usr/bin/env python
import socket, struct
import csv, sys, time, os

def GetEmptyMap(allNames, width):
	emptyMap = []
	for x in range(0, width):
		column = []
		for y in range(0, 15):
			column.append([0]*len(allNames))
		emptyMap.append(column)
	return emptyMap

def FlattenMap(mapInput, allNames):
	flatMap = []
	for y in range(0, 15):
		for x in range(0, 40):
			for z in range(0, len(allNames)):
				flatMap.append(mapInput[x][y][z])
	return flatMap

def UnflattenMap(mapInput, allNames):
    emptyMap = GetEmptyMap(allNames)
    for x in range(0, 40):
        for y in range(0, 15):
            for z in range(0, len(allNames)):
                emptyMap[x][y][z] = mapInput[y*40*len(allNames)+x*len(allNames)+z]
    return emptyMap

def ListToString(listVal):
	strVal = ""
	for i in range(0, len(listVal)):
		strVal+=str(listVal[i])
	return strVal

def GetSprites(listVal):
	spriteString = ""
	for x in range(0, 40):
		for y in range(0, 15):
			for z in range(0, len(listVal[x][y])):
				if listVal[x][y][z]>0:
					spriteString+=str(x)+"*"+str(y)+"*"+str(z)+"-"
	return spriteString

def StringToRows(strVal, actions, startX):
	finalRows = []
	if "*" in strVal:
		spriteVals = strVal.split("*")
		for spriteVal in spriteVals:
			if len(spriteVal)>0:
				splits2 = spriteVal.split(",")
				if len(splits2)==4 and len(splits2[3])>0:
					print (splits2)
					finalRows.append([actions[int(splits2[2])], int(splits2[0])+startX, int(splits2[1]), float(splits2[3])])
	else:
		splits2 = strVal.split(",")
		print (splits2)
		if len(splits2)>=4:	
			finalRows.append([actions[int(splits2[2])], int(splits2[0])+startX, int(splits2[1]), float(splits2[3])])
	return finalRows



#Remove old additions
if os.path.isfile("./additions.csv"):
    os.remove("./additions.csv")
#lawn-128-61-65-211.lawn.gatech.edu
TCP_IP = '73.137.201.7'#morrible.cc.gt.atl.ga.us 127.0.0.1
TCP_PORT = 8080
BUFFER_SIZE = 8112


allNames = ['Ground', 'Stair', 'Treetop', 'Block', 'Bar', 'Koopa', 'Koopa 2', 'PipeBody', 'Pipe', 'Question', 'Coin', 'Goomba', 'CannonBody', 'Cannon', 'Lakitu', 'Bridge', 'Hard Shell', 'SmallCannon', 'Plant', 'Waves', 'Hill', 'Castle', 'Snow Tree 2', 'Cloud 2', 'Cloud', 'Bush', 'Tree 2', 'Bush 2', 'Tree', 'Snow Tree', 'Fence', 'Bark', 'Flag', 'Mario']
actions = ['Ground', 'Stair', 'Treetop', 'Block', 'Bar', 'Koopa', 'Koopa 2', 'PipeBody', 'Pipe', 'Question', 'Coin', 'Goomba', 'CannonBody', 'Cannon', 'Lakitu', 'Bridge', 'Hard Shell', 'SmallCannon', 'Plant', 'Waves', 'Hill', 'Castle', 'Snow Tree 2', 'Cloud 2', 'Cloud', 'Bush', 'Tree 2', 'Bush 2', 'Tree', 'Snow Tree', 'Fence', 'Bark', 'Nothing']

emptyMap = GetEmptyMap(allNames, 100)
#Load Sprites
source = open (sys.argv[1], "rb")
reader = csv.reader(source)
readRow = False
for row in reader:
	if readRow:
		emptyMap[int(row[1])][int(row[2])][allNames.index(row[0])] = 1
		finalPosX = int(row[1])
	else:
		readRow = True

startX = finalPosX-20

if startX<0:
	startX = 0

if startX+40>=100:
	startX-=(startX+40)-99

emptyMapReal = GetEmptyMap(allNames, 40)
for x in range(0, 40):
	emptyMapReal[x] = emptyMap[x+startX]

mapSprites = GetSprites(emptyMapReal)
mapSprites+= "|"+str(startX)
#: "+str(mapString))
#print (len(mapSprites))
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect((TCP_IP, TCP_PORT))
s.send(mapSprites)

#time.sleep(2) 
rows= {}
numAdded=0
gotSomething = False
while True:
	reply = s.recv(BUFFER_SIZE)
	
	if reply=="" and gotSomething:
		print ("break")
		break
	else:
		gotSomething = True
		thisList= StringToRows(reply, actions, startX)

		for spriteVal in thisList:
			if numAdded<30:#40 is overwhelming
				if not spriteVal[1] in rows.keys():
					rows[spriteVal[1]] ={}
				if not spriteVal[2] in rows[spriteVal[1]].keys():
					rows[spriteVal[1]][spriteVal[2]] = ["",0]
				if rows[spriteVal[1]][spriteVal[2]][1]<spriteVal[3]:
					numAdded+=1
					rows[spriteVal[1]][spriteVal[2]] = [spriteVal[0], spriteVal[3]]

target = open('./additions.csv', 'wb')
writer = csv.writer(target)
xKeysSorted = sorted(list(rows.keys()))
for rowKey in xKeysSorted:
	for rowKey2 in rows[rowKey].keys():
		if sum(emptyMap[rowKey][rowKey2])==0 or actions.index(rows[rowKey][rowKey2][0])<len(actions)-15 and sum(emptyMap[rowKey][rowKey2][0:-15])==0:#Empty or only decoration
			writer.writerow([rows[rowKey][rowKey2][0], rowKey, rowKey2, rows[rowKey][rowKey2][1]])
s.close()
target.close()
