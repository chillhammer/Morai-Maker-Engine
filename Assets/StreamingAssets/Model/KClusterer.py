import random
import numpy as np


'''
@Matthew Guzdial

K Means++: http://theory.stanford.edu/~sergei/papers/kMeansPP-soda.pdf
Distortion Ratio: http://www.ee.columbia.edu/~dpwe/papers/PhamDN05-kmeans.pdf
'''

class KClusterer():
	def __init__(self, distanceMethod, kMeans, equivalenceMethod, reevalueCenters=None, maxAttemptsToConverge = 1000, localMax = 10):
		#Used to determine distance between objects you're clustering
		self.distanceMethod = distanceMethod
		#Boolean for determining if this is kMeans or kMedoids
		self.kMeans = kMeans
		#Method to determine if two of the objects are equal
		self.equivalenceMethod = equivalenceMethod
		#Used to generate mean centers given a cluster (only for kMeans)
		self.reevalueCenters = reevalueCenters
		#Used to change the max number of allowable steps before convergence
		self.maxAttempts = maxAttemptsToConverge
		#Number of steps past finding local max
		self.localMax = localMax


	#Pick Initial Centers from dataset X and known number of centers K
	def InitializeClusters(self, X, K):
		#Gives initial clusters of length 1 chosen randomly from the list
		initialCenters =random.sample(X, 1)
		while len(initialCenters)< K:
			#Step 1. Compute current min distance vector from centers to all points
			minDistanceVector = []
			minSum = float("inf")
			for center in initialCenters:
				myDistanceVector = []
				for x in X:
					myDistanceVector.append(self.distanceMethod(x, center))
				mySum = sum(myDistanceVector)
				if mySum<minSum or minSum==float("inf"):
					minSum = mySum
					minDistanceVector = myDistanceVector

			#Step 2. Grab a new point based on the probabilities of the distance over the sum of all distances
			probabilities = []
			probabilities = minDistanceVector/np.sum(minDistanceVector)
			cummulativeProbabilities = np.cumsum(probabilities)
			r = random.random()
			try:
				index = np.where(cummulativeProbabilities>=r)[0][0]
				initialCenters.append(X[index])
			except:
				print "Your distance method is returning an integer, please use a float"
				xToUse = random.choice(X)
				while xToUse in initialCenters:
					xToUse = random.choice(X)
				initialCenters.append(xToUse)
		return initialCenters

	#Determines if convergence has occured by checking if there is a match for each one of the old and new centers
	def HasConverged(self, centers, oldCenters):
		numSimilar = 0
		
		for node1 in oldCenters:
			for node2 in centers:
				#Check if equivalent
				if self.equivalenceMethod(node1, node2):
					numSimilar+=1
		#If they each have one they match, we've converged
		return numSimilar==(len(oldCenters))


	#Find the most center of each cluster as the new center of each cluster for k medoids
	def ReevaluateCentersMediods(self, clusters):
		newCenters = []
		keys = sorted(clusters.keys())
		for k in keys:
			newCenter = None
			minDist = float("inf")
			for node in clusters[k]:
				dist = 0
				for node2 in clusters[k]:
					dist = dist+ self.distanceMethod(node, node2)
				if dist<minDist:
					minDist = dist
					newCenter = node
			newCenters.append(newCenter)
		return newCenters

	#Cluster all the points to closest center
	def ClusterPoints(self, X, centers):
		clusters  = {}
		for x in X:
			#Just find the closest center for each point and assign it to that cluster as a dictionary
			bestCenterKey = None
			minDist = float("inf")

			for center in centers:
				dist = self.distanceMethod(center, x)
				if dist<minDist:
					minDist = dist
					bestCenterKey = center

			if not bestCenterKey in clusters.keys():
				clusters[bestCenterKey] = []

			clusters[bestCenterKey].append(x)
		return clusters

	#Find clusters and centers given X (dataset) and K (number of clusters) [Call This One With Known K]
	def FindCenters(self, X, K):
		# Initialize to K random centers for prev
		oldCenters = random.sample(X, K)
		# Make use of k Means++ seeding technique to find initial centers
		centers = None
		if self.kMeans: 
			centers = self.InitializeClusters(X, K)
		else:
			centers = random.sample(X, K)
		attempts = 0
		clusters = {}
		while not self.HasConverged(centers, oldCenters) and attempts<self.maxAttempts:
			oldCenters = centers
			# Assign all points in X to clusters
			clusters = self.ClusterPoints(X, centers)
			# Reevaluate centers with means or medoids
			if self.kMeans:
				centers = self.reevalueCenters(clusters)	
			else:
				centers = self.ReevaluateCentersMediods(clusters)

			#Check to ensure that the distance metric (if it's faulty), dropped the length below the requirement
			if not len(centers) ==K:
				if self.kMeans: 
					centers = self.InitializeClusters(X, K)
				else:
					centers = random.sample(X, K)
			attempts = attempts+1
		return(centers, clusters)

	#Find K Centers + Clusters from X by averaging over iterations
	def FindCentersAverage(self, X, K, iterations):
		#Generate iterations clusterings
		centers = []
		clusters = []
		dists = []
		for i in range(iterations):
			print "Find Centers Average "+str(i)
			iCenters, iClusters = self.FindCenters(X, K)
			centers.append(iCenters)
			clusters.append(iClusters)
			thisSK = 0
			for ci in range(0, len(iClusters.keys())):
				for c in iClusters[iClusters.keys()[ci]]:
					thisSK+= self.distanceMethod(iCenters[ci], c)
			dists.append(thisSK)


		#Find the median clustering
		median = np.median(np.array(dists))
		iToUse = np.nanargmin(np.abs(np.array(dists)-median))

		return centers[iToUse], clusters[iToUse]


	#Returns a value of the ratio of the real distortion within clusters to the estimated distortion for a K. 
	#Small distortion ratio values indicate there's a high density of clustering
	def FindDistortionRatio(self, thisk, X, dimensionality,  Skm1=0, timesToRunPerK = 1):
		a = lambda k, dimensionality: 1 - 3/(4*dimensionality) if k == 2 else a(k-1, dimensionality) + (1-a(k-1, dimensionality))/6.0
		
		centersAll = []
		clustersAll = []

		SKList = []
		for times in range(timesToRunPerK):
			print "Attempt: "+str(times)
			centers, clusters = self.FindCenters(X, thisk)

			thisSK=0

			for i in range(0, len(clusters.keys())):
				for c in clusters[clusters.keys()[i]]:
					thisSK+= self.distanceMethod(centers[i], c)
			SKList.append(thisSK)

		Sk = np.median(np.array(SKList))

		if thisk == 1:
			fs = 1
		elif Skm1 == 0:
			fs = 1
		else:
			fs = Sk/(a(thisk,dimensionality)*Skm1)
		return fs, Sk

	#Pass in a data set, the max dimensionality of the data set, the maxK, and the number of times to generate clusters and average
	def FindKAndCluster(self, X, dimensionality, maxK, numberToRun = 100, timesToRunPerDistortionCalculation=100, startK = 1):
		kValuesToUse = []

		ks = range(startK, maxK)
		distortionRatios = [0]*(len(ks))

		#Special Case K=1
		distortionRatios[0], Sk = self.FindDistortionRatio(startK, X, dimensionality, 0, timesToRunPerDistortionCalculation)

		#Initialize variables with which to find K to use
		kToUse = startK
		minDistortion = distortionRatios[0]
		counter = 0

		#Calculate distortion ratio for all other values of K
		index = 1
		for k in ks[1:]:
			print "K: "+str(k)
			#Can have an index out of range error here if we've begun to have too many keys
			distortionRatios[index], Sk = self.FindDistortionRatio(k, X, dimensionality, Sk, timesToRunPerDistortionCalculation)
			
			print "Distortion Ratio: "+str(distortionRatios[index])
			if minDistortion>distortionRatios[index] and distortionRatios[index]<=0.85:
				print "New minimum!"
				minDistortion = distortionRatios[index]
				kToUse = k
				counter=0
			#If we haven't found a lower distorition value within 10 K's, use the current one
			counter+=1
			if counter>=self.localMax:
				break
			index+=1
		
		return self.FindCentersAverage(X, kToUse, numberToRun)







	




	