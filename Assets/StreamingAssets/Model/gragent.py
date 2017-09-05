import pickle, glob
from agent import *
from GenerateLNodes import *

def stringTranslation(strVal):
    numValues = [" ", "1", "2", "3", "4", "5", "6", "7", "8", "9"]

    for num in numValues:
        index = strVal.find(num)
        if not index==-1:
            strVal = strVal[:index]
    return strVal

def GetAllowedPosition(xy):
    newX = int(round(xy[0]/26.0))
    newY = int(round(xy[1]/26.0))
    newX = newX *26
    newY = newY *26
    newX = max(0,newX)
    newY = max(52,newY)
    newY = min(newY, 360)



    return tuple((newX, newY))

def vectorDifference(obj1, obj2, xMin, yMin):
		return (abs(obj1.x-obj2.x)<xMin) and (abs(obj1.y-obj2.y)<yMin)

#Returns a list of unique sprites 
def GetUniqueSpritesAndCounts(gNodes):
	uniqueSprites = []
	spriteCounts = []

	for gNode in gNodes:
		for fObj in gNode.pattern:
			if not fObj.name in uniqueSprites:
				uniqueSprites.append(fObj.name)
				spriteCounts.append(1)
			else:
				spriteCounts[uniqueSprites.index(fObj.name)]+=1
	return uniqueSprites, spriteCounts

def GetVectorToTopLeft(typeString, gNode):
    widthToUse = gNode.width
    heightToUse = gNode.height

    pointToReturn = [0,0]
    if typeString=="Center":
        pointToReturn[0] = -0.5*widthToUse
        pointToReturn[1] = -0.5*heightToUse
    elif typeString=="North":
        pointToReturn[0] = -0.5*widthToUse
    elif typeString=="East":
        pointToReturn[0] = -1*widthToUse
        pointToReturn[1] = -0.5*heightToUse
    elif typeString == "South":
        pointToReturn[0] = -0.5*widthToUse
        pointToReturn[1] = -1*heightToUse
    elif typeString == "West":
        pointToReturn[1] = -0.5*heightToUse

    return pointToReturn

#HELPER FUNCTIONS
def GetProbabilityOfLNode(uniqueList, lNode):
	probability = len(uniqueList)
	for sprite in uniqueList:
		#print "Sprite! "+str(sprite)+" vs "+str(lNode.probabilityTable.keys())
		if sprite in lNode.probabilityTable.keys():
			probability -= (1.0-lNode.probabilityTable[sprite])
		else:
			probability -= 1.0

	#Prefer a simpler solution
	if len(lNode.probabilityTable)>0:
	    probability /= float(len(lNode.probabilityTable))
	return probability

def GetProbabilityOfSNode(uniqueList, sNode):
    probability = len(uniqueList)
    for sprite in uniqueList:
        if sprite in sNode.cooccurenceRate.keys():
            probability -= (1.0-sNode.cooccurenceRate[sprite])
        else:
            probability -= 1.0
            #Remove for creativity
            return 0
    
    #Prefer a simpler solution
    if len(sNode.cooccurenceRate):
        probability /= float(len(sNode.cooccurenceRate))
    return probability

def NameConversion(spriteName):
	spriteName = stringTranslation(spriteName)
	allSpriteNames = ['cloud', 'flag', 'goomba', 'questionBlock', 'block', 'stair', 'questionUnblock', 'pipeTop', 'hill', 'pipeBody', 'koopa', 'bush', 'ground', 'wall', 'bar', 'coin', 'treetop', 'bark', 'vine', 'tree', 'shortTree', 'spring', 'fence', 'drawbridge', 'hammerbro', 'snowTreeTall', 'snowTree', 'wave', 'crank', 'greebo', 'spike', 'hardshell', 'mushroom', 'mushroomTop', 'mushroomBeam', 'bullet', 'cannonshort', 'cannon']
	if spriteName.lower() in allSpriteNames:
		return spriteName.lower()
	else:
		if spriteName=="Pipe":
			return "pipeTop"
		elif spriteName=="PipeBody":
			return "pipeBody"
		elif spriteName=="Lakitu":
			return "greebo"
		elif spriteName=="Bridge":
			return "drawbridge"
		elif spriteName=="Hammer":
			return "hammerbro"
		elif spriteName=="Question":
			return "questionBlock"
		elif spriteName=="Snow":
			return "snowTreeTall"
		elif spriteName=="Waves":
			return "wave"
		elif "Flag" in spriteName:
			return "flagPoleTop"
		elif "Cannon" in spriteName:
			return "cannon"
	return ""

def OppositeNameConversion(spriteName):
	allSpriteNames = ["Ground", "Block", "Stair", "Pipe", "PipeBody", "Treetop", "Bridge", "Coin", "Question", "Cannon 1", "Bar", "Spring", "Bush", "Hill", "Cloud", "Tree", "Snow Tree", "Fence", "Bark", "Castle", "Waves", "Goomba", "Koopa", "Hard Shell", "Hammer Bro", "Lakitu"]

	if spriteName.title() in allSpriteNames:
		return spriteName.title()
	else:
		if spriteName=="pipeTop":
			return "Pipe"
		elif spriteName=="pipeBody":
			return "PipeBody"
		elif spriteName=="greebo":
			return "Lakitu"
		elif spriteName=="drawbridge":
			return "Bridge"
		elif spriteName=="hammerbro":
			return "Hammer Bro"
		elif "question" in spriteName:
			return "Question"
		elif "snow" in spriteName:
			return "Snow Tree"
		elif "cannon" in spriteName:
			return "Cannon 1"
		elif spriteName=="wave":
			return "Waves"
		elif spriteName=="ground":
			return "Ground"
		elif spriteName=="mushroomTop":
			return "Treetop"
		elif spriteName=="mushroom":
			return "Bark"
		elif spriteName=="hardShell":
			return "Hard Shell"

	return "" 


#Base Agent Class
class GRAgent:

	#Set Agent Name
	def __init__(self):
		object.__init__("GRAgent")
		self.lNodes = []

	#Load the LNodes
	def LoadModel(self):
		for filename in glob.glob("./GRmodel/*.p"):
			print "Loading: "+str(filename)
			lNodeFile = open(filename, "rb")
			lNode = pickle.load(lNodeFile)
			lNodeFile.close()

			self.lNodes.append(lNode)

	#Turn list of sprites into dictionaries of lists of gnodes
	def ConvertToAgentRepresentation(self, objectsList, levelWidth, levelHeight):
		#Separate everything into chunks
		chunks = {}
		maxKey = 0
		for sprite in objectsList:
			thisX = int(sprite.x/16)
			if thisX*16<levelWidth:
				if not thisX in chunks.keys():
					chunks[thisX] = []

				spriteName = NameConversion(sprite.name)
				chunks[thisX].append(FrameObject(spriteName, sprite.x*26,412-sprite.y*26,sprite.w*26,sprite.h*26))
				if thisX>maxKey:
					maxKey = thisX

		for x in range(0, maxKey):
			if not x in chunks.keys():
				chunks[x] = []

		level = {}
		#Combine the different sections of objects by proximity and by spritename
		for chunkKey in chunks.keys():
			gNodes = [] 

			spriteList = list(chunks[chunkKey])
			spriteList.sort(key=lambda fObj: (fObj.x, fObj.y))

			listOfListsOfPatterns = []

			#Difference of adjacency to use
			heightToUse = 0
			widthToUse = 0
			if len(spriteList)>0:
				heightToUse = spriteList[0].h*2
				widthToUse = spriteList[0].w*2

			#split the spriteList into seperate listOfListPatterns
			while len(spriteList)>0:
				#Try creating a pattern around the first object in the list
				newList = [spriteList[0]]
				spriteList.remove(newList[0])

				index = 0
				while index<len(newList):
					sourcelist = list(spriteList)
					for sprite in sourcelist:
						if newList[index].name==sprite.name and vectorDifference(newList[index], sprite, widthToUse, heightToUse):
							newList.append(sprite)
							spriteList.remove(sprite)
					index+=1	
				listOfListsOfPatterns.append(newList)

			#write out pattern to G Node list
			for pattern in listOfListsOfPatterns:
				thisNode = GNode(pattern[0].name, "test", pattern)
				gNodes.append(thisNode)

			#DNode
			for gNode in gNodes:
				for gNode2 in gNodes:
					if not gNode==gNode2:
						gNode.AddDifference(gNode2)

			#Set gNodes to piece of level
			level[chunkKey] = gNodes

		return level


	#Convert from internal level representation to lists of Sprites (opposite of above)
	def ConvertToSpriteRepresentation(self, level):
		listOfSprites = []
		for gNodeTuple in level:
			for pObj in gNodeTuple[0].pattern:
				x = gNodeTuple[1]
				y = gNodeTuple[2]
				newSpriteName = OppositeNameConversion(pObj.name)
				if len(newSpriteName)>0:
					listOfSprites.append(Sprite(newSpriteName, (pObj.x+x)/26, int((412-(pObj.y+y))/26), pObj.w/26, pObj.h/26))
					if listOfSprites[-1].name=="Ground":
						listOfSprites.append(Sprite(newSpriteName, (pObj.x+gNodeTuple[1])/26, int((412-(pObj.y+gNodeTuple[2]))/26)-1, pObj.w/26, pObj.h/26))
		return listOfSprites


	#Find the best LNode, get the best NNode, and find the max prediction
	def RunModel(self, level):
		additions = []
		print "Run Model Hit"
		for chunkKey in level.keys():
			print "Chunk Key: "+str(chunkKey)
			#FIND L Node
			maxLNode = None
			maxPossibilities = []
			
			maxProbability = 0
			uniqueSpriteList, spriteCounts = GetUniqueSpritesAndCounts(level[chunkKey])

			for lNode in self.lNodes:

				thisProbability = GetProbabilityOfLNode(uniqueSpriteList, lNode)
				#print "LNode: "+str(lNode.sNames) +". probability: "+str(thisProbability)
				if thisProbability > maxProbability:
					maxProbability = thisProbability
					maxPossibilities = [lNode]
				elif thisProbability==maxProbability:
					maxPossibilities.append(lNode)

			#Choose randomly among equivalent options
			if len(maxPossibilities)>0:
				maxLNode = random.choice(maxPossibilities)
			else:
				maxLNode = random.choice(self.lNodes)
			
			#FIND CLOSEST N NODE
			closestNNode = None
			closestNDist = float("inf")
			for nNode in maxLNode.nNodeIds:
				currDistance = 0
				for pair in nNode.pairs:
					if pair[0] in uniqueSpriteList:
						currDistance+= abs(pair[1]-spriteCounts[uniqueSpriteList.index(pair[0])])
					else:
						currDistance+=pair[1]

				if currDistance<closestNDist:
				    closestNDist = currDistance
				    closestNNode = nNode


			#Find most needed next type
			mostNeededNext = "None"
			maxNeed = 0
			maxPossibilities = []

			#CHECK THROUGH N NODE NEED
			for pair in closestNNode.pairs:
				thisValue = 1.0
				if pair[0] in uniqueSpriteList:
					thisValue -= float(spriteCounts[uniqueSpriteList.index(pair[0])]/pair[1])

				if thisValue>maxNeed:
				    maxNeed = thisValue
				    maxPossibilities = [pair[0]]
				    print "New Connection from n Node "+str(pair[0])
				elif thisValue==maxNeed:
				    maxPossibilities.append(pair[0])
				    print "Append Connection from n Node "+str(pair[0])

			if len(maxPossibilities)>0:
				mostNeededNext = random.choice(maxPossibilities)

				#FIND MAX S NODE FOR mostNeededNext
				if not maxLNode==None:
					maxSNode = None
					maxPossibilities = []
					maxProbability = -0.01
					for sNode in maxLNode.sNodes:
					    if sNode.name == mostNeededNext:
					        thisProbability = GetProbabilityOfSNode(uniqueSpriteList, sNode)
					        if thisProbability > maxProbability:
					            maxProbability = thisProbability
					            maxPossibilities = [sNode]
					        elif thisProbability == maxProbability:
					            maxPossibilities.append(sNode)
					#Choose randomly among equivalent options
					if len(maxPossibilities)>0:
					    maxSNode = random.choice(maxPossibilities)

					#print "CONNECTION FIND"
					maxConnecton = None
					maxProbability = 0
					maxGNode = None
					maxConnectionPossibilities = []
					maxGNodePossibilities = []

					if not maxSNode==None:
					    #Find best node
					    for gNode in level[chunkKey]:
					        #Find best connection for this node, then see if best overall
					        for connection in maxSNode.allConnections:
					            #Do the connections match up
					            if connection.name1==mostNeededNext and connection.name2 == gNode.name: 
					                thisProbability = maxSNode.probabilities[connection] 
					                
					                distBetweenGNode = distanceBetweenGDNode(gNode, connection.otherNode)
					                #print "    This probability1 "+str(thisProbability)
					                if distBetweenGNode>0:
					                    thisProbability/=distBetweenGNode

					                if thisProbability>maxProbability:
					                    maxProbability = thisProbability
					                    maxConnectionPossibilities = [connection]
					                    maxGNodePossibilities = [gNode]
					                elif thisProbability==maxProbability:
					                    maxGNodePossibilities.append(gNode)
					                    maxConnectionPossibilities.append(connection)
					    #Choose randomly among equivalent options
					    if len(maxConnectionPossibilities)>0:
					    	maxConnecton = random.choice(maxConnectionPossibilities)
					    	maxGNode = maxGNodePossibilities[maxConnectionPossibilities.index(maxConnecton)]
					    print "Max Connection probability: "+str(maxProbability)
					    if not maxConnecton == None and maxProbability>0.001:
					    	print "Max GNode Chosen: "+str(maxGNode.name)
					    	vecBetweenPointAndCorner = GetVectorToTopLeft(maxConnecton.type2, maxGNode)
					    	originalPoint = [maxGNode.xMin-vecBetweenPointAndCorner[0], maxGNode.yMin-vecBetweenPointAndCorner[1]]#TO FIX
					    	toPoint = [originalPoint[0]+maxConnecton.xDiff*2, originalPoint[1]+maxConnecton.yDiff*2]
					    	toPlacePointToCorner = GetVectorToTopLeft(maxConnecton.type1, maxConnecton.myNode)
					    	topLeftCorner = [toPoint[0]+toPlacePointToCorner[0], toPoint[1]+toPlacePointToCorner[1]]
					    	finalAllowablePosition = GetAllowedPosition(topLeftCorner)

					    	additions.append(tuple((maxConnecton.myNode, finalAllowablePosition[0], finalAllowablePosition[1]+maxConnecton.myNode.height/2)))
					    elif maxConnecton == None and len(maxConnectionPossibilities)==0:
					    	randomGNode = random.choice(maxSNode.gNodes)
					    	print "Adding random node: "+str(randomGNode.name)
					    	print "ChunkKey: "+str([chunkKey, randomGNode.xMin])




					    	tupleVal=tuple((randomGNode, chunkKey*16*26+randomGNode.xMin, randomGNode.yMin+randomGNode.height/2+26))
					    	print "Tuple Val: "+str(tupleVal)
					    	additions.append(tupleVal)

		return additions

