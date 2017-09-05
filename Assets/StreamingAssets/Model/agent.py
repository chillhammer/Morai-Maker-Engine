#Sprite Class
class Sprite:
	def __init__(self, name, x, y, w, h):
		self.name = name
		self.x = x
		self.y = y
		self.w = w
		self.h = h


#Base Agent Class
class Agent:
	#Set Agent Name
	def __init__(self, name):
		self.name = name

	#Called when agent is loaded by server, grab any files/weights needed to run agent
	def LoadModel(self):
		return None

	#Given list of Sprites return agent's internal level representation
	def ConvertToAgentRepresentation(self, objectsList, levelWidth, levelHeight):
		return objectsList

	#Convert from internal level representation/additions to lists of Sprites (opposite of above)
	def ConvertToSpriteRepresentation(self, level):
		return []

	#Run the model given internal agent level representation, return the updated full level in agent representation or just additions
	def RunModel(self, level):
		return level



