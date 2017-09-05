import csv, shutil, sys, os, copy, pickle
import random, math
import numpy as np
from KClusterer import KClusterer

#These things could potentially change
gameplaysToUse = [1, 2, 3, 4, 5, 6, 7, 9, 10]
clusteringCSV = "clustering.csv"
gameplayFolderName = "frames"
frameByFrameDescription = "frameDescriptions.csv"
sys.setrecursionlimit(4500)
clusterSplit = "-Frames"

ListOfSections =[]

bucketSize = float(412/50)

class Tile:
	def __init__(self, name, x,y, width, height):
		self.name = name
		self.x = x
		self.y = y
		self.width = width
		self.height = height
		self.active = False

	#Determine if the passed in frameObject is "inside" this one.
	def inside(self, frameObject):
		p1 = (self.x, self.y)
		p2 = (self.x+self.width, self.y)
		p3 = (self.x+self.width, self.y+self.height)
		p4 = (self.x, self.y+self.height)

		p21 = (p2[0]-p1[0], p2[1]-p1[1])
		p41 = (p4[0]-p1[0], p4[1]-p1[1])

		p21mag_squared = math.pow(p21[0], 2)+ math.pow(p21[1],2)
		p41mag_squared = math.pow(p41[0], 2)+ math.pow(p41[1],2)

		listOfPointsToTest = [(frameObject.x, frameObject.y), (frameObject.x+frameObject.w/2, frameObject.y+frameObject.h/2),  (frameObject.x+frameObject.w, frameObject.y),  (frameObject.x, frameObject.y+frameObject.h), (frameObject.x+frameObject.w, frameObject.y+frameObject.h)]

		for x,y in listOfPointsToTest:
			p = (x - p1[0], y - p1[1])
			if 0 <= p[0] * p21[0] + p[1] * p21[1] <= p21mag_squared:
				if 0 <= p[0] * p41[0] + p[1] * p41[1] <= p41mag_squared:
					return True

		return False

class FrameObject:
	def __init__(self, name, positionX, positionY, w, h):
		self.name = name
		self.x = int(positionX)
		self.y = int(positionY)
		self.w = int(w)
		self.h = int(h)
	def Equals(self, obj):
		return (self.name==obj.name and self.x==obj.x and self.y ==obj.y)

	def GetString(self):
		return "FrameObj "+str(self.name)+" at ("+str(self.x)+", "+str(self.y)+", "+str(self.w)+", "+str(self.h)+")"

	#must be of same name/type
	def distance(self, obj):
		dist = math.sqrt(math.pow(obj.x-self.x, 2)+math.pow(obj.y-self.y, 2))
		return dist

	def vectorDifference(self, obj, xMin, yMin):
		return (abs(obj.x-self.x)<xMin) and (abs(obj.y-self.y)<yMin)


def unit_vector(vector):
    """ Returns the unit vector of the vector.  """
    return vector / np.linalg.norm(vector)

def angle_between(v1, v2):
    v1_u = unit_vector(v1)
    v2_u = unit_vector(v2)
    angle = np.arccos(np.dot(v1_u, v2_u))
    if np.isnan(angle):
        if (v1_u == v2_u).all():
            return 0.0
        else:
            return np.pi
    return angle

#A single one of the connections held in the D Node
class DNodeConnection():
	def __init__(self, gNode1, gNode2, typeOfConnection1, typeOfConnection2, distance, xDiff, yDiff):
		global bucketSize
		self.name1 = None
		self.myId = None
		self.name2 = None
		self.otherId = None
		self.used = False
		self.myNode = None
		self.otherNode = None

		self.type1 = typeOfConnection1
		self.type2 = typeOfConnection2

		self.distance = distance #Use this for calculating size
		self.categoricalDistance = int(round(self.distance/bucketSize))#Use this for distance of DNodeConnection
		self.xDiff = xDiff
		self.yDiff = yDiff

		#Necessary due to fake DNodeConnections for SNode clustering
		if not gNode1==None and not gNode2==None:
			self.name1 = gNode1.name
			self.myId = gNode1.id
			self.name2 = gNode2.name
			self.otherId = gNode2.id

			self.myNode = gNode1
			self.otherNode = gNode2


	#TODO; Change this constant to be 5% diff for max diff of whatever thing is piped in
	def __eq__(self, other):
		return (isinstance(other, self.__class__) and self.name1 == other.name1 and self.name2==other.name2 and self.type1== other.type1 and self.type2 == other.type2 and abs(self.categoricalDistance-other.categoricalDistance)<100 )

	def __ne__(self, other):
		return not self.__eq__(other)

	def __hash__(self):
		return hash(self.name1+self.name2+self.type1+self.type2+str(self.categoricalDistance))

	def GetDistance(self):
		return self.distance

	def copy(self):
		newDNode = DNodeConnection(None, None, self.type1, self.type2, self.distance, self.xDiff, self.yDiff)
		newDNode.name1 = self.name1
		newDNode.myId = self.myId
		newDNode.name2 = self.name2
		newDNode.otherId = self.otherId

		#I CHANGED THIS
		newDNode.type1 = self.type1
		newDNode.type2 = self.type2

		newDNode.xDiff = self.xDiff
		newDNode.yDiff = self.yDiff

		newDNode.otherNode = self.otherNode
		newDNode.myNode = self.myNode

		newDNode.used = self.used

		return newDNode


class Template:
	def __init__(self, frameObjects):
		self.objects = frameObjects

	def AddFrameObject(self, newObject):
		inside = False
		for f in self.objects:
			if f.Equals(newObject):
				inside = True
				break

		if not inside:
			f.objects.append(newObject)

	def Equals(self, frameList, difference = 0):
		maxLength = len(frameList)
		numSame = 0
		for i in frameList:
			for j in self.objects:
				if j.Equals(FrameObject(i.name, i.x+difference, i.y+difference, i.w, i.h)):
					numSame = numSame+1
		percentageSame = ((float(numSame)/float(maxLength)))
		return (percentageSame>0.5)

class SNodeProbability():
	def __init__(self, probability, initialDNodeConnection):
		self.probability = probability
		self.dNodeConnections = [initialDNodeConnection]

class LNode():
	#Takes in an initial list of the sNodes from this LNode and all nNodes so it can pick out it's own
	def __init__(self, sNodes, medianSNode):
		self.sNodes = sNodes
		self.medianSNode = medianSNode
		self.sNames = []
		self.nNodeIds = []

		uniqueSeenList = []
		countsSeenList = []

		for sNode in self.sNodes:
			#Append name
			self.sNames.append(sNode.name)
			#Grab the nNode ids from the sNodes
			for n in sNode.nNodes:
				if not n in self.nNodeIds:
					self.nNodeIds.append(n)

			for seen in sNode.seenSprites:
				if seen in uniqueSeenList:
					countsSeenList[uniqueSeenList.index(seen)] +=1.0
				else:
					uniqueSeenList.append(seen)
					countsSeenList.append(1.0)

		#Construct probability table
		self.probabilityTable = {}

		for index in range(0, len(countsSeenList)):
			countsSeenList[index] = countsSeenList[index]/float(len(self.sNodes))
			self.probabilityTable[uniqueSeenList[index]] = countsSeenList[index]

	def AddSNode(self, sNode):
		self.sNodes.append(sNode)
		#Redetermine medianSNode and sNodes
		sNodeDict = {}
		sNodeDict[self.medianSNode] = self.sNodes
		newMu = reevaluateCentersMedoidsS(sNodeDict)
		self.medianSNode = newMu[0]

		self.sNames = []

		for s in self.sNodes:
			#Append name
			self.sNames.append(s.name)

		print "After sNames: "+str(self.sNames)

	def PrintOut(self):
		print "LNode"
		print "sNames: "+str(self.sNames)
		print "Probability: "
		for key in self.probabilityTable.keys():
			print str(key)+": "+str(self.probabilityTable[key])

		for nNode in self.nNodeIds:
			print "nNode: "+nNode.id


class SNode():
	def __init__(self, _gNodes):
		self.seenSprites = []
		if len(_gNodes)>0:

			self.gNodes = _gNodes
			self.name = self.gNodes[0].name
			
			self.seenSprites = []# the sprites name that this SNode "sees"
			
			uniqueConnections= []
			connectionCounts = []

			seenCounts = []

			self.nNodes = [] #The Templates associated with this Node
			self.nNodeIds = [] #The ids of all these nNodes
			self.allConnections = []

			#Hook the G Node up to its S Node and begin to set up probability distribution
			for gNode in self.gNodes:
				gNode.SNode = self
				for connection in gNode.DNode.connections:

					#Update our probabilistic connection info
					self.allConnections.append(connection)
					if not connection in uniqueConnections:
						uniqueConnections.append(connection)
						connectionCounts.append(1.0)
					else:
						connectionCounts[uniqueConnections.index(connection)]+=1.0

					#Update our probabilistic coocurrence info
					if not connection.name2 in self.seenSprites:
						self.seenSprites.append(connection.name2)
						seenCounts.append(1.0)
					else:
						seenCounts[self.seenSprites.index(connection.name2)]+=1.0

				#Update our probabilistic 
				if not gNode.NNode in self.nNodes:
					self.nNodes.append(gNode.NNode)

					if not gNode.NNode.id in self.nNodeIds:
						self.nNodeIds.append(gNode.NNode.id)

			#Update the probabilities of the connections
			#How does this type of shape probabilistically suggest certain connections
			self.probabilities = {}
			for index in range(0, len(connectionCounts)):
				connectionCounts[index] = connectionCounts[index]/float(len(self.gNodes))
				self.probabilities[uniqueConnections[index]] = connectionCounts[index]
			
			#Update the probabilities of the seen sprites
			#How does this type of shape probabilistically suggest the presence of other sprites
			self.cooccurenceRate ={}
			for index in range(0, len(seenCounts)):
				seenCounts[index] = seenCounts[index]/float(len(self.gNodes))
				self.cooccurenceRate[self.seenSprites[index]] = seenCounts[index]

			if not self.name in self.seenSprites:
				self.seenSprites.append(self.name)

			self.cooccurenceRate[self.name] = 1.0
				
	def ToString(self):
		print ""
		print "S Node: "
		print "Size of S Node: "+str(len(self.gNodes))
		for gNode in self.gNodes:
			gNode.ToString()


		for nNode in self.nNodes:
			print "NNode: "+str(nNode.id)

		print "My S Node Probabilities: "
		for key in self.probabilities.keys():
			print "Between: "+str(key.name1)+" and "+str(key.name2)+" connect "+str(key.type1)+"->"+str(key.type2)+" probability: "+str(self.probabilities[key])



#This node stores count information 
class NNode():
	#pairs are tuples of the form (spritenameName, number)
	def __init__(self, myId, pairs):
		self.id = myId
		self.pairs = pairs


def GetPoints(gNode):
	return [gNode.CenterPoint(), gNode.NorthPoint(), gNode.EastPoint(), gNode.SouthPoint(), gNode.WestPoint()]

class DNode():
	def __init__(self, gNode1, gNode2):
		self.connections=[]
		self.gNode = gNode1
		self.probabilityDistribution = {}
		if not gNode1==None and not gNode2==None and not gNode1==gNode2:
			self.AddConnection(gNode1, gNode2)

	def copy(self):
		newDNode = DNode(self.gNode, None)
		for c in self.connections:
			newConnection = c.copy()
			newDNode.connections.append(newConnection)

		newProbabilityDistribution = {}

		for key in self.probabilityDistribution.keys():
			newProbabilityDistribution[key] = self.probabilityDistribution[key]

		newDNode.probabilityDistribution = newProbabilityDistribution

		return newDNode

	def AddConnection(self, gNode1, gNode2):
		if not gNode1==gNode2 and self.gNode.name == gNode1.name:
			types = ["Center", "North", "East", "South", "West"]
			gNodePoints1 = GetPoints(gNode1)
			gNodePoints2 = GetPoints(gNode2)

			minPoint1 = None
			minPoint2 = None
			minDist = float("inf")

			for point1 in gNodePoints1:
				for point2 in gNodePoints2:
					thisDist = math.pow((point1[0]-point2[0]), 2) + math.pow((point1[1]-point2[1]), 2)

					if thisDist<minDist:
						minDist = thisDist
						minPoint1 = point1
						minPoint2 = point2

			index1 = gNodePoints1.index(minPoint1)
			index2 = gNodePoints2.index(minPoint2)

			type1 = types[index1]
			type2 = types[index2]

			xDiff = minPoint1[0]-minPoint2[0]
			yDiff = minPoint1[1]-minPoint2[1]

			self.connections.append(DNodeConnection(gNode1, gNode2, type1, type2, minDist, xDiff, yDiff))

	def SetConnections(self, connections):
		self.connections = connections

	def ToString(self):
		strVal = "D Node with Connections: "

		for connection in self.connections:
			d = connection.GetDistance()
			strVal = strVal + ("Connection "+str(connection.name1)+" to "+str(connection.name2)+" with distance "+str(d)+", ")

		return strVal


class GNode():
	def __init__(self, name, id, objects):
		self.objects = objects
		self.name = name
		self.pattern = []
		self.id = id
		self.DNode = None

		self.xMin = 0
		self.yMin = 0

		if not self.objects == None:
			self.GeneratePattern()

		self.width = 0
		self.height = 0

		self.DNode = DNode(self, None)
		self.SNode = None
		self.NNode = None

		for p in self.pattern:
			if (p.x+p.w)>self.width:
				self.width = float(p.x+p.w)
			if (p.y+p.h)>self.height:
				self.height = float(p.y+p.h)

	def CenterPoint(self):
		return tuple((self.xMin+self.width/2.0, self.yMin+self.height/2.0))

	def NorthPoint(self):
		return tuple((self.xMin+self.width/2.0, self.yMin))

	def SouthPoint(self):
		return tuple((self.xMin+self.width/2.0, self.yMin+self.height))

	def WestPoint(self):
		return tuple((self.xMin, self.yMin+self.height/2.0))

	def EastPoint(self):
		return tuple((self.xMin+self.width, self.yMin+self.height/2.0))

	def copy(self):
		newGNode = GNode(self.name, self.id, self.objects)

		#CHANGED THIS LINE 8:58 AM on Tuesday
		newGNode.DNode = self.DNode.copy()

		newGNode.width = self.width
		newGNode.height = self.height
		newGNode.SNode = self.SNode
		return newGNode

	def SetNNode(self, nNode):
		self.NNode = nNode

	def SetPatternConnection(self, pattern, connections):
		self.pattern = pattern

		for p in self.pattern:
			if (p.x+p.w)>self.width:
				self.width = float(p.x+p.w)
			if (p.y+p.h)>self.height:
				self.height = float(p.y+p.h)

		self.DNode = DNode(self, None)
		self.DNode.connections = connections

	def OfType(self, otherName):
		return name==otherName

	def AddObject(self, newObj):
		self.objects.append(newObj)

	def GeneratePattern(self):
		minY = float("inf")
		minX = float("inf")

		for o in self.objects:
			if o.x<minX:
				minX = o.x
			if o.y<minY:
				minY = o.y
		self.xMin = minX
		self.yMin = minY
		for o in self.objects:
			self.pattern.append(FrameObject(o.name, o.x-minX, o.y-minY, o.w, o.h))
		#Sort pattern
		self.pattern.sort(key=lambda obj: (obj.x, obj.y))


	def distanceBetween(self, otherGNode):
		totalDist = 0		
		for p1 in self.pattern:
			for p2 in otherGNode.pattern:
				totalDist+= p1.distance(p2)
		return totalDist

	def AddDifference(self, gNode2):
		if self.DNode==None:
			self.DNode = DNode(self, gNode2)
		else:
			self.DNode.AddConnection(self, gNode2)

	def CanCoexist(self, gNode2):
		bestMatch = None
		minDiff = float("inf")
		for connection in self.DNode.connections:

			for connection2 in gNode2.DNode.connections:
				if connection.name2== gNode2.name and connection2.name2==self.name and not connection.used:
					#These connections should cancel
					dist = (connection.xDiff+connection2.xDiff)+(connection.yDiff+connection2.yDiff)
					if dist<minDiff:
						bestMatch = connection
						minDiff = dist
		#Note: Can't do this here or you mark as used when it hasn't been
		return bestMatch, minDiff

	#Looks for maximum similarity above some threshold
	def CanCoexist2(self, gNode2, percentRequired):
		bestMatch = None
		maxPercent = 0
		for connection in self.DNode.connections:
			for connection2 in gNode2.DNode.connections:
				if connection.name2== gNode2.name and connection2.name2==self.name and not connection.used:
					percent = 0
					if not connection.distance==0:
						percent =1.0-( angle_between([connection.xDiff/connection.distance, connection.yDiff/connection.distance], [-1*connection2.xDiff/connection.distance, -1*connection2.yDiff/connection.distance])/3.141592653589793)
					percent2 = 1.0-(abs(connection.distance-connection2.distance)/float(max(connection.distance,connection2.distance)))
					totalPercent = (percent+percent2)/2.0
					if totalPercent>percentRequired and totalPercent>maxPercent:
						bestMatch = connection
						maxPercent = totalPercent
		#Note: Can't do this here or you mark as used when it hasn't been
		return bestMatch, maxPercent

	#Diff must be smaller percentage-wise than percentRequired and also must be further away than min of both widths
	def HasMatch(self, matchName, potentialPos, width, height, percentRequired):
		for connection in self.DNode.connections:
			if connection.name2 ==matchName:
				newXDiff = potentialPos[0]-self.xMin
				newYDiff = potentialPos[1]-self.yMin
				percent =1.0-( angle_between([newXDiff, newYDiff], [connection.xDiff, connection.yDiff])/3.141592653589793)

				if percent>percentRequired:
					return True
		return False

	def ToString(self):
		print ""
		print "GNode of type "+str(self.name) +" from id "+str(self.id)
		print "I have a pattern of: "
		for p in self.pattern:
			print p.GetString()
		print self.DNode.ToString()




#Translate the names of the sprite images into a more usable form
def stringTranslation(strVal):
	numValues = ["1", "2", "3", "4", "5", "6", "7", "8", "9", "L", "R", "Wave"]

	for num in numValues:
		index = strVal.find(num)

		if not index==-1:
			strVal = strVal[:index]

	if ".png" in strVal:
		index = strVal.find(".png")

		if not index==-1:
			strVal = strVal[:index]
	return strVal

#########################################################
### K Means Methods
#########################################################

#Find the difference between a G and D node pair
def distanceBetweenGDNode(gNode1, gNode2):
	distance = 0
	
	#G Node Distance
	gDist = 0
	maxToUse = max(len(gNode1.pattern), len(gNode2.pattern))


	for i in range(maxToUse):
		if i<len(gNode1.pattern) and i<len(gNode2.pattern):
			if not gNode1.pattern[i].x==gNode2.pattern[i].x or not gNode1.pattern[i].y==gNode2.pattern[i].y:
				gDist+=1
		else:
			gDist+=1


	minConnections = None
	maxConnections = None

	if len(gNode1.DNode.connections)<len(gNode1.DNode.connections):
		minConnections = gNode1.DNode.connections
		maxConnections = gNode2.DNode.connections
	else:
		minConnections = gNode2.DNode.connections
		maxConnections = gNode1.DNode.connections

	dDist = len(maxConnections)-len(minConnections)

	#D Node Distance
	for connection1 in minConnections:
		toAdd =1
		if connection1 in maxConnections:
			toAdd = 0

		dDist += toAdd
		
	distance = float(gDist+dDist)

	return distance

#Reevaluate centers to the most average center
def reevaluateCentersGD(clusters):
	#Newmu is the new centers
	newmu = []
	keys = sorted(clusters.keys())
	for k in keys:
		#1. Grab the average pattern length and grab connections size
		patternLengthAverage = 0
		connectionsSizeAverage = 0
		for node in clusters[k]:
			patternLengthAverage+= len(node.pattern)
			connectionsSizeAverage +=len(node.DNode.connections)
		patternLengthAverage = patternLengthAverage/len(clusters[k])
		connectionsSizeAverage = connectionsSizeAverage/len(clusters[k])

		#2. Make the pattern
		pattern = []
		
		for i in range(patternLengthAverage):
			patternX = 0
			patternY = 0
			for node in clusters[k]:
				if i<len(node.pattern):
					patternX+=node.pattern[i].x
					patternY+=node.pattern[i].y
			patternX = patternX/float(patternLengthAverage)
			patternY = patternY/float(patternLengthAverage)

			frameObj = FrameObject("TEST", patternX, patternY, 1, 1)
			pattern.append(frameObj)

		#3. Grab connections
		connections= []
		for i in range(connectionsSizeAverage):
			otherNames = []
			typeOurs = []
			typeOthers = []
			distances =0

			for node in clusters[k]:
				if i<len(node.DNode.connections):
					distances += node.DNode.connections[i].distance
					otherNames.append(node.DNode.connections[i].name2)
					typeOurs.append(node.DNode.connections[i].type1)
					typeOthers.append(node.DNode.connections[i].type2)
			thisDNode = DNodeConnection(None, None, max(set(typeOurs), key=typeOurs.count), max(set(typeOthers), key=typeOthers.count), distances/float(connectionsSizeAverage), 0,0)
			thisDNode.name2 = max(set(otherNames), key=otherNames.count)
			connections.append(thisDNode)
		

		#Make new GNode
		newCenter = GNode("None", "None", None)
		newCenter.SetPatternConnection(pattern, connections)

		newmu.append(newCenter)

	return newmu



#Determines if convergence has occured by checking if there is a match for each one of the old and new centers
def equivalentGDNode(node1, node2):
	#C Check
	if len(node1.pattern)==len(node2.pattern):
		numShared = 0
		for p1 in node1.pattern:
			for p2 in node2.pattern:
				if p1.x==p2.x and p1.y==p2.y:
					numShared+=1
		#D Check
		if numShared==len(node1.pattern) and len(node1.DNode.connections)==len(node2.DNode.connections):
			numShared = 0
			for d in node1.DNode.connections:
				for d2 in node2.DNode.connections:
					if d==d2:
						numShared+=1

			if numShared==len(node1.DNode.connections):
				return True
	return False

##########
## S Node
##########

def equivalentSNode(node1, node2):
	#Check to see if the two are just equivalent since we're using median
	#return set(node1.seenSprites)==set(node2.seenSprites)
	#return set(node1.nNodeIds) == set(node2.nNodeIds)
	return node1==node2


def distanceBetweenSNode(sNode1, sNode2):
	#Number of differences starts as difference in size between the two 
	return float( len((set(sNode1.seenSprites)-set(sNode2.seenSprites)))+len((set(sNode1.nNodeIds)-set(sNode2.nNodeIds))))


'''
def distanceBetweenSNode(sNode1, sNode2):
	numberOfDifferences = 0
	for key1 in sNode1.allConnections:
		match = False
		for key2 in sNode2.allConnections:
			if key1.name1 == key2.name1 and key1.name2 == key2.name2:
				match = True
				break

		if not match:
			numberOfDifferences+=1
	return float(numberOfDifferences)
'''

def reevaluateCentersMedoidsS(clusters):
	newmu = []
	keys = sorted(clusters.keys())
	for k in keys:
		newCenter = None
		minDist = float("inf")
		for node in clusters[k]:
			dist = 0
			for node2 in clusters[k]:
				dist = dist+ distanceBetweenSNode(node, node2)
			if dist<minDist:
				minDist = dist
				newCenter = node
		newmu.append(newCenter)
	return newmu

def reevaluateCentersS(clusters):
	newmu = []
	keys = sorted(clusters.keys())
	for k in keys:
		newCenter = SNode([])
		allLengths = []
		uniqueList = []
		countList = []

		for node in clusters[k]:
			allLengths.append(len(node.seenSprites))

			for sprite in node.seenSprites:
				if not sprite in uniqueList:
					uniqueList.append(sprite)
					countList.append(1)
				else:
					countList[uniqueList.index(sprite)]+=1


		length = int(round(np.mean(allLengths)))
		thisSeenSprites = []

		#Grab the 'length' most common 
		while len(thisSeenSprites)<length:
			thisCount = max(countList)
			thisIndex = countList.index(thisCount)

			thisSeenSprites.append(uniqueList[thisIndex])
			countList[thisIndex] = -1

		newmu.append(newCenter)

	return newmu


#########################################################
#########################################################
#########################################################

#higher is better
'''
def probabilisticDifference(sNode1, template):
	percentage = 0
	used  = []
	for key1 in sNode1.ConnectionsTo.keys():
		for t in template:
			if key1.name2 == t.name and not t in used:
				used.append(t)
				percentage+=sNode1.ConnectionsTo[key1].probability

	return percentage

def distanceBetweenTemplateAndSNode(sNode1, template):
	numberOfDifferences = 0
	for t in template:
		match = False
		for gNode in sNode1.gNodes:
			for diff in gNode.DNode.connections:
				#name2 because I want the one that this isn't
				if t.name==diff.name2:
					match = True
					break

		if not match:
			numberOfDifferences+=1
	return numberOfDifferences
'''

def PatternOverlap(l1, r1, l2, r2):
	if (l1[0]>r2[0]) or (l2[0]>r1[0]):
		return False
	if (l1[1] <r2[1]) or (l2[1] <r1[1]):
		return False
	return True

def OverlapAmount(l1, r1, l2, r2):
	xAdd = 0
	xMin = min(l1[0], l2[0])
	if xMin<0:
		xAdd = abs(xMin)

	yAdd = 0
	yMin = min(l1[1], l2[1])
	if yMin<0:
		yAdd = abs(yMin)

	l1 = (l1[0]+xAdd,l1[1]+yAdd)
	r1 = (r1[0]+xAdd,r1[1]+yAdd)
	l2 = (l2[0]+xAdd,l2[1]+yAdd)
	r2 = (r2[0]+xAdd,r2[1]+yAdd)


	SI = max(0, max(r1[0], r2[0]) -min(l1[0], l2[0]))*max(0, max(r1[1], r2[1]) -min(l1[1], l2[1]))
	SU = ((r1[0]-l1[0])*(r1[1]-l1[1]))+((r2[0]-l2[0])*(r2[1]-l2[1]))-SI
	if SU==0:
		return 0
	return float(SI)/float(SU)


def DiffConnects(gNode1, gNode2, diff):
	if not gNode1==gNode2 and gNode2.name == gNode1.name:
		minDist = float("inf")
		xDiff = 0
		yDiff = 0
		xDiffToMinPoint1 = 0
		yDiffToMinPoint1 = 0
		xDiffToMinPoint2 = 0
		yDiffToMinPoint2 = 0

		xDiffToMinPoint1 = gNode1.xMin-0
		yDiffToMinPoint1 = gNode1.yMin-0
		xDiffToMinPoint2 = 0-gNode2.xMin
		yDiffToMinPoint2 = 0-gNode2.yMin

		xDiff = gNode2.xMin-gNode1.xMin
		yDiff = gNode2.yMin-gNode1.yMin

		xDiffToMinPoint1 = xDiffToMinPoint1/gNode1.width
		yDiffToMinPoint1 = xDiffToMinPoint1/gNode1.height
		xDiffToMinPoint2 = xDiffToMinPoint2/gNode2.width
		yDiffToMinPoint2 = yDiffToMinPoint2/gNode2.height

		if abs(minDist-diff.distance)<1:
			if abs(xDiffToMinPoint2-diff.xDiffToMinPoint2)<1 and abs(yDiffToMinPoint2-diff.yDiffToMinPoint2)<1:
				return True

	return False

def GetPotentialPosition(gNode, connection, width2, height2):
	xPos = (gNode.xMin+connection.xDiff+connection.xMid2)-(width2/2.0)
	yPos = gNode.yMin

	if connection.above:
		yPos = gNode.yMin-connection.yMaxDiff2 
	if connection.below:
		yPos = connection.yMinDiff2+gNode.yMin#ADDING HEIGHT DID NOT WORK
	return (xPos, yPos)

#Return the connection and its gNode2 that is most likely and matches percentageRequired of the patterns in newTemplate
def CanCoexist(potentialGNode, newTemplate, percentageRequired):
	maxPercent = 0
	matchToUse = None
	gNodeToUse = None
	matchPos = []

	#added
	numMatch = 0
	for newT in newTemplate:
		match, matchDist = newT.CanCoexist2(potentialGNode, percentageRequired)

		if not match==None:
			numMatch +=1
			if matchDist>maxPercent:
				matchPos = GetPotentialPosition(newT, match, potentialGNode.width, potentialGNode.height)
				matchToUse = match
				gNodeToUse = newT
				maxPercent = matchDist

				
	percentageMatch = (float(numMatch)/float(len(newTemplate)))
	if percentageMatch<percentageRequired:
		return None, None, []
	return matchToUse, gNodeToUse, matchPos

#Template is a list of gNodes, protoTemplate is a list of spritenames to numbers that we're aiming for
def generateLevelSectionFromProto(tupleStr, gNode, sNodes, template, protoTemplate, percentageAllowed, percentageRequired, percentageConnectionStep, depth, maxDepth, minX, minY, maxX, maxY):
	global ListOfSections
	print "Depth "+str(depth)
	if depth>=maxDepth:
		print "Depth return"
		return

	#Step -1: Copy all the things
	newTemplate = []
	for c in template:
		newCFromTemplate = c.copy()
		newTemplate.append(newCFromTemplate)

	#Step -0.5: Make the connections
	#if t.name==newConnection.name2 and (((t.xMin-pos[0]+t.width/2.0-newConnection.xMid2)-newConnection.xDiff))<13 and ((newConnection.above and t.yMin<pos[1]) or (newConnection.below and t.yMin>pos[1]) or (not newConnection.above and not newConnection.below)):
	for t in newTemplate:
		minVal = min(t.width, gNode.width)
		#Check all the connections in the new gNode
		for gNodeConnection in gNode.DNode.connections:
			if gNodeConnection.name2 ==t.name and (((t.xMin-gNode.xMin+t.width/2.0-gNodeConnection.xMid2)-gNodeConnection.xDiff))<minVal and ((gNodeConnection.above and t.yMin<gNode.yMin) or (gNodeConnection.below and t.yMin>gNode.yMin) or (not gNodeConnection.above and not gNodeConnection.below)):
				gNodeConnection.used = True
		#Check all the connections for this template against new gNode
		for tConnection in t.DNode.connections:
			if tConnection.name2== gNode.name and not tConnection.used and (((gNode.xMin-t.xMin+gNode.width/2.0-tConnection.xMid2)-tConnection.xDiff))<minVal and ((tConnection.above and gNode.yMin<t.yMin) or (tConnection.below and gNode.yMin>t.yMin) or (not tConnection.above and not tConnection.below)):
				tConnection.used = True

	#Step 0: Add gNode
	newTemplate.append(gNode)

	#Step 1.5: Create a dict of these for easy look up
	templateDict = {}
	for t in newTemplate:
		if t.name in templateDict.keys():
			templateDict[t.name]+=len(t.objects)
		else:
			templateDict[t.name]=len(t.objects)

	#Step 2: Determine if done and if not, what next to add
	nextToAdd1 = ""
	minDiff = float("inf")
	totalPercent = 0

	totalSum = float(sum(protoTemplate.values()))

	for key in protoTemplate.keys():
		diff = 0
		if key in templateDict.keys() and templateDict[key]>0:
			totalPercent +=	float(templateDict[key])/totalSum
			diff = float(templateDict[key])/float(protoTemplate[key])
		else:
			#If is missing this key, then
			diff = 0.0

		if diff<minDiff:
			minDiff = diff 
			nextToAdd1 = key

	nextToAdd = nextToAdd1

	acrossDifferencesProbabilityMin = float("inf")
	nextToDiffAdd = ""

	#Change: Added check to see if its within 0.9% of the ListOfSections (Added back in)
	if totalPercent>0.5:
		printIt = True
		for t in ListOfSections:
			numSame = 0
			totalLength = len(t)
			for tGNode in t:
				for templateNode in template:
					l1 = (tGNode.xMin, tGNode.yMin)
					r1 = (tGNode.xMin+tGNode.width, tGNode.yMin+tGNode.height)

					l2 = (templateNode.xMin, templateNode.yMin)
					r2 = (templateNode.xMin+templateNode.width, templateNode.yMin+templateNode.height)
					if tGNode.name==templateNode.name and OverlapAmount(l1, r1, l2, r2)>0.9:
						numSame+=1
							
			#Must be at least halfway different
			if (float(numSame)/float(totalLength))>0.9:
				printIt=False
				break

		if not printIt:
			print "Template Match Return"
			return

	#Check each GNode to see if it has sufficient stuff in it
	#CHANGE
	'''
	for gNode in newTemplate:
		localProb = 0.0
		localMax = 0
		localNext = ""
		for connection in gNode.DNode.connections:
			if connection.used:
				localProb+= gNode.DNode.probabilityDistribution[connection]
				print "this probability "+str(gNode.DNode.probabilityDistribution[connection])
				print "Upping Local Prob: "+str(localProb)
			else:
				if localMax<gNode.DNode.probabilityDistribution[connection]:
					localMax = gNode.DNode.probabilityDistribution[connection]
					localNext = connection.name2
				print "Not upping local prob"

		if localProb<acrossDifferencesProbabilityMin:
			nextToDiffAdd = localNext
			acrossDifferencesProbabilityMin = localProb
	
	'''
	connectionToUse = None
	gNodeToUse = None

	#Change to just checking if all of the ones above a certain probability are there (Instantiate values above a certain probability as required edges)
	'''
	for gNode in newTemplate:
		localMin = 1
		localNext = ""
		localConnection = None
		localGNode = None

		for connection in gNode.DNode.connections:
			if not connection.used and gNode.DNode.probabilityDistribution[connection]>percentageRequired:
				val = -1*gNode.DNode.probabilityDistribution[connection]
				if val<localMin:
					localMin = val
					localNext = connection.name2
					localConnection = connection
					localGNode = gNode

		if localMin<acrossDifferencesProbabilityMin:
			nextToDiffAdd = localNext
			acrossDifferencesProbabilityMin = localMin
			gNodeToUse = localGNode
			connectionToUse = localConnection
	'''

	#Checks the SNode to determine the probability and force to an edge all those above a given probability
	for gNode in newTemplate:
		localMin = 1
		localNext = ""
		localConnection = None
		localGNode = None

		for connection in gNode.DNode.connections:
			match = gNode.SNode.probabilityDistribution.keys()[gNode.SNode.probabilityDistribution.keys().index(connection)]
			if not connection.used and gNode.SNode.probabilityDistribution[match]>percentageConnectionStep:
				val = -1*gNode.SNode.probabilityDistribution[match]
				if val<localMin:
					localMin = val
					localNext = connection.name2
					localConnection = connection
					localGNode = gNode

		if localMin<acrossDifferencesProbabilityMin:
			nextToDiffAdd = localNext
			acrossDifferencesProbabilityMin = localMin
			gNodeToUse = localGNode
			connectionToUse = localConnection

	#Changed from 1 to percentRequired
	if (minDiff>=percentageRequired and totalPercent>=percentageRequired and acrossDifferencesProbabilityMin<0):
		nextToAdd = nextToDiffAdd

	#We're done!
	if minDiff>=percentageRequired and (totalPercent)>=1.0 and totalPercent<=(percentageAllowed+1.0) and acrossDifferencesProbabilityMin>0:
		'''
		THIS IS THE END
		'''
		#printTemplate(newTemplate, tupleStr+"-"+str(minDiff)+"-"+str(totalPercent)+"-"+str(acrossDifferencesProbabilityMin))
		
	
	#June 9th: Altered to not end early
	if totalPercent>=(1.0+percentageAllowed):
		print "Return"
		return
	else:
		#for nextToAdd in toAdd:
		#print "To add: "+str(nextToAdd)
		for sNode in sNodes:
			#print "SNode name: "+sNode.name
			if sNode.name==nextToAdd: #and probabilisticDifference(sNode, template)>percentageAllowed:
				#print "Found one sNode with name of nextToAdd: "
				for gNode in sNode.gNodes:

					#This adds the same one at lots of different places
					#CHANGE: Ensure that the added thing has a viable connection for all templates
					#for gNode2 in newTemplate:
						#Check if can cooexist on average?

					#Return the connection and its gNode2 that is most likely and matches percentageRequired of the patterns in newTemplate
					connection, gNode2, pos = CanCoexist(gNode, newTemplate, percentageRequired) #TODO; Alter this line
					if not gNode==gNode2 and not connection==None:
						newC = gNode.copy()
						
						'''
						print "Added "+str(newC.name)+" at position "+str(pos) +") of length "+str(len(newC.objects))+" with id "+str(newC.id)
						print "Added via: "+str(gNode2.name)+ " at position "+str(pos) +") of length "+str(len(newC.objects))
						print "with match "+str(connection.xDiff)+", "+str(connection.yDiff)
						for t in newTemplate: 
							print "Already in template "+str(t.name)+" at position ("+str(t.xMin)+", "+str(t.yMin)+") of length "+str(len(t.objects))+" with id "+str(t.id)
						'''
						newMinX = minX
						newMinY = minY
						newMaxX = maxX
						newMaxY = maxY


						newObjects = []
						for p in newC.pattern:
							newObjects.append(FrameObject(p.name, p.x+pos[0], p.y+pos[1], p.w, p.h))
						newC.objects = newObjects
						newC.xMin = pos[0]
						newC.yMin = pos[1]
						newDepth = depth+1

						if newMinX>newC.xMin:
							newMinX = newC.xMin
						elif newMaxX<newC.xMin+newC.width:
							newMaxX = newC.xMin+newC.width
						if newMinY>newC.yMin:
							newMinY = newC.yMin
						elif newMaxY<newC.yMin+newC.height:
							newMaxY = newC.yMin+newC.height

						#print "Calling again with object: "+p.name
						#TODO; switch out for legit version
						if abs(newMaxY-newMinY)>360:#abs(newMaxX-newMinX)>412 or
							print "Exceeded Area"
							return
						generateLevelSectionFromProto(tupleStr, newC, sNodes, newTemplate, protoTemplate, percentageAllowed, percentageRequired, percentageConnectionStep, newDepth, maxDepth, newMinX, newMinY, newMaxX, newMaxY)
	print "Nothing to add return"
	return

##################################################################################################################

##################################################################################################################
# This is the code actually called by terminal calls
##################################################################################################################

##################################################################################################################

def main():
	#STEP ONE: Iterate through each of the gameplay videos to grab frame information
	#This dictionary holds a dictionary of frame number to list of frame objects
	dictOfGameplayToFrameToList ={}
	for gameplayVal in gameplaysToUse:
		#Set up the dict to go from string gameplay value to a second dict for frames
		dictOfGameplayToFrameToList[str(gameplayVal)] = {}
		gameplayPath ="./"+gameplayFolderName+str(gameplayVal)+"/"+frameByFrameDescription

		#Create the reader for each gameplay to read in each frame's values
		source = open(gameplayPath, "rb")
		reader = csv.reader(source)

		#Current frame
		currFrame = str(-1)
		currFrameObjects = []

		readIt = False
		#Read through each row of frames
		for row in reader:
			#Don't read the first frame
			if readIt:
				#grab the frame of this row
				thisFrame = row[0]
				if not thisFrame == currFrame:

					if not currFrame == "-1":
						#write out to dictOfGameplayToFrameToList
						dictOfGameplayToFrameToList[str(gameplayVal)][currFrame] = currFrameObjects
						currFrameObjects = []
					#currFrame set to this new frame
					currFrame = thisFrame
				
				#Append this rows frame object to our list
				currFrameObjects.append(FrameObject(stringTranslation(row[1]), int(row[2]), int(row[3]), int(row[4]), int(row[5])))
			else:
				readIt = True

	#STEP TWO: Grab information from cluster to list of level sections
	clusterToLevelSections = {}
	source = open("./"+clusteringCSV, "rb")
	reader = csv.reader(source)

	readIt = False
	#Read through each cluster
	for row in reader:
		#Don't read the first frame
		if readIt:
			#Determine clusterName
			splits0 = row[0].split(clusterSplit)
			clusterOrig = splits0[0]
			splits1 = row[1].split(clusterSplit)
			recluster = splits1[0]
			clusterName = clusterOrig+"-"+recluster

			#Ensure clusterToLevelSections has a list for clusterName
			if not clusterName in clusterToLevelSections.keys():
				clusterToLevelSections[clusterName] = []

			#Add tuple (gameplay, frame) to list of clusterToLevelSections
			clusterToLevelSections[clusterName].append(tuple((row[2], row[3])))

		else:
			readIt = True

	#STEP THREE: Find the G nodes, D nodes, and N node for each frame in each cluster
	#TODO; make this for every needed cluster instead of just a test case
	clusterName = "19-6"
	print "CLUSTER NAME: "+str(clusterName)

	nNodes = []#Collection of N Nodes from this cluster
	maxGNodes = 0#Max number of G Nodes in this cluster
	gNodesDict = {}#Dict from tupleVal to list of gNodes
	gNodesBySpriteName = {}#Dict from spritename to list of gNodes

	for tupleVal in clusterToLevelSections[clusterName]:

		#STEP 3A: Create N Node
		#Count data of objects by spritename
		objectsByName = {}

		#Grab count data and add it up
		for o in dictOfGameplayToFrameToList[tupleVal[0]][tupleVal[1]]:
			if o.name in objectsByName.keys():
				objectsByName[o.name].append(o)
			else:
				objectsByName.setdefault(o.name, [])
				objectsByName[o.name].append(o)

		#Create the N Node of this section
		pairList = []
		for key in objectsByName.keys():
			pairList.append(tuple(( key, len(objectsByName[key]) )))

		myId = str(tupleVal)
		nNodes.append(NNode(myId, pairList))

		#STEP 3B: Create all gNodes
		
		#Combine the different sections of objects by proximity and by spritename
		gNodes = []
		for key in objectsByName.keys():
			if not key in gNodesBySpriteName.keys():
				gNodesBySpriteName.setdefault(key, [])

			spriteList = list(objectsByName[key])
			spriteList.sort(key=lambda fObj: (fObj.x, fObj.y))
			listOfListsOfPatterns = []

			#Difference of adjacency to use
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
						if newList[index].vectorDifference(sprite, widthToUse, heightToUse):
							newList.append(sprite)
							spriteList.remove(sprite)
					index+=1	
				listOfListsOfPatterns.append(newList)

			#write out pattern to G Node list
			for pattern in listOfListsOfPatterns:
				thisNode = GNode(pattern[0].name, str(tupleVal[0])+"-"+str(tupleVal[1]), pattern)
				thisNode.SetNNode(nNodes[len(nNodes)-1])#SET THE N NODE
				gNodes.append(thisNode)

		#Check if we find the max number of G Nodes
		if len(gNodes)>maxGNodes:
			maxGNodes= len(gNodes)

		
		#STEP 3C D Node Create
		#Now we have all the GNodes of this frame, now make the DNodes of this frame
		for gNode in gNodes:
			for gNode2 in gNodes:
				if not gNode==gNode2:
					gNode.AddDifference(gNode2)


		#Create probability Distribution?
		for gNode in gNodes:
			gNodesBySpriteName[gNode.name].append(gNode)


	#THIS CODE creates the kClusterer for sNodes
	clusterer = KClusterer(distanceBetweenGDNode, True, equivalentGDNode, reevaluateCentersGD, 100, 10)
	'''
	kValuesPerSpritename = {}
	kValuesPerSpritename["mushroomBeam"] = 1
	kValuesPerSpritename["bar"] = 4
	kValuesPerSpritename["bullet"] = 2
	kValuesPerSpritename["goomba"] = 2
	kValuesPerSpritename["treetop"] = 1
	kValuesPerSpritename["koopa"] = 1
	kValuesPerSpritename["bark"] = 5
	kValuesPerSpritename["coin"] = 1
	kValuesPerSpritename["questionBlock"] = 2
	kValuesPerSpritename["cloud"] = 1
	kValuesPerSpritename["questionUnblock"] = 2
	'''
	sNodes = []



	for key in gNodesBySpriteName.keys():
		print "Length of max K: "+str(len(gNodesBySpriteName[key]))+". For spritename "+str(key)
		if (len(gNodesBySpriteName[key]))>1:
			centers = []
			clusters = {}

			maxCPatterns =0
			maxDConnections = 0
			for gNode in gNodesBySpriteName[key]:
				if maxCPatterns<len(gNode.pattern):
					maxCPatterns = len(gNode.pattern)
				if maxDConnections<len(gNode.DNode.connections):
					maxDConnections = len(gNode.DNode.connections)

			
			centers, clusters = clusterer.FindKAndCluster(gNodesBySpriteName[key], int((maxCPatterns+maxDConnections)), len(gNodesBySpriteName[key]), 100, 100 )
			#centers, clusters = clusterer.FindCentersAverage(gNodesBySpriteName[key], kValuesPerSpritename[key], 100)
			for cKey in clusters.keys():
				#Generate Your SNodes
				sNodes.append(SNode(clusters[cKey]))
		else:
			sNodes.append(SNode(gNodesBySpriteName[key]))


	#THIS CODE Cluster These SNodes using the Hellman Metric
	clustererMedoids = KClusterer(distanceBetweenSNode, False, equivalentSNode, reevaluateCentersMedoidsS, 100)

	sNodeDimensionality = 0
	sDicts = {}
	for s in sNodes:
		if len(s.probabilities.keys())>sNodeDimensionality:
			sNodeDimensionality = len(s.probabilities.keys())

		if not s.name in sDicts.keys():
			sDicts[s.name] = [s]
		else:
			sDicts[s.name].append(s)

	#CREATION PHASE
	sNodeCenters, sNodesClusters = clustererMedoids.FindKAndCluster(sNodes, sNodeDimensionality, min(len(nNodes), len(sNodes)), 100, 100)
	#sNodeCenters, sNodesClusters = clustererMedoids.FindCentersAverage(sNodes, 2, 100)

	lNodes = []
	for key in sNodesClusters.keys():
		fullList = list(sNodesClusters[key])
		lNodes.append(LNode(fullList, key))

	#TEST PHASE
	for sNodeName in sDicts.keys():
		for lNode in lNodes:
			#Do we need a cluster of this
			if sNodeName in lNode.probabilityTable.keys() and lNode.probabilityTable[sNodeName]>=0.5 and not sNodeName in lNode.sNames:
				print ""
				print "LNode: "+str(lNode.sNames)
				print "I need "+str(sNodeName)
				newSNode = min(sDicts[sNodeName], key=lambda s:distanceBetweenSNode(s,lNode.medianSNode))
				lNode.AddSNode(newSNode)


	for lNode in lNodes:
		lNode.PrintOut()
		pickle.dump(lNode, open("./LNodes/"+str(clusterName)+"lNode"+str(lNodes.index(lNode))+".p", "wb" ))

if __name__ == '__main__':
    main()  