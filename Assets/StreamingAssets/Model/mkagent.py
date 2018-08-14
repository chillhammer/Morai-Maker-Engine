import json
import random
from numpy.random import choice
from agent import *

class MKAgent():

    # Set Agent Name
    def __init__(self):
        object.__init__("MKAgent")
        self.distributionMatrix = {}
        self.lastXPos = 0

    # Called when agent is loaded by server, grab any files/weights needed to run agent
    def LoadModel(self):

        # Load the trained, Probability Distribution Matrix from an external file
        self.distributionMatrix = json.load(open("./MKmodel/MKmodelmatrix.txt"))
        return None


    # Given list of Sprites return agent's internal level representation
    def ConvertToAgentRepresentation(self, objectsList, levelWidth, levelHeight):
        spriteToChar = {"Ground": 'X', "Block": 'S', "Stair": 'X', "Pipe": '<', "PipeBody": '[',
                        "Treetop": '-', "Bridge": 'X', "Coin": 'o', "Question": '?',
                        "Cannon 1": '-', "Bar": 'S', "Spring": '-', "Bush": '-', "Hill": '-',
                        "Cloud": '-', "Tree": '-', "Snow Tree": '-', "Fence": '-', "Bark": '-',
                        "Castle": '-', "Waves": '-', "Goomba": 'E', "Koopa": 'E', "Hard Shell": 'E',
                        "Hammer Bro": 'E', "Lakitu": 'E'}

        level = []
        rows = levelHeight
        columns = levelWidth

        self.lastXPos = objectsList[-1].x

        # Convert objectsList into 2-D (list of lists) char matrix
        for i in range(rows, -1, -1):
            objects = []
            for j in range(0, columns):
                spriteToAdd = None
                for o in objectsList:
                    # Ensure that sprites in the map match the position of sprites in our matrix
                    if o.y == i and o.x == j and o.name in spriteToChar:
                            spriteToAdd = o
                            break
                if not spriteToAdd == None:
                    print "spriteToAdd: " + str(spriteToAdd.name)
                    objectsList.remove(spriteToAdd)

                    # Convert to a sprite representation our agent can understand
                    convertedSprite = Sprite(spriteToChar[spriteToAdd.name],
                                             spriteToAdd.y, spriteToAdd.x, spriteToAdd.w, spriteToAdd.h)
                    objects.append(convertedSprite)
                else:
                    # An empty sprite represents empty (sky) space in the map
                    convertedSprite = Sprite("-", i, j, 1, 1)
                    objects.append(convertedSprite)
            level.append(objects)

        # Add an underground 'dummy' layer to the matrix
        # This allows the agent to predict ground tiles using the the top-down-downright dependency
        undergroundSprite = Sprite("U", None, None, 1, 1)
        extensionRow = []
        for i in range(0, len(level[0])):
            extensionRow.append(undergroundSprite);
        level.append(extensionRow);

        # Koopas, pipes, and pipebodies are special cases that should be handled

        return level

    # Convert from internal level representation/additions to lists of Sprites (opposite of above)
    def ConvertToSpriteRepresentation(self, level):

        charToSprite = {'X': ["Ground", "Stair"], 'S': ["Block"], '<': ["Pipe"], '>': ["Pipe"],
                        '[': ["PipeBody"], ']': ["PipeBody"], 'o': ["Coin"], '?': ["Question"],
                        'Q': ["Question"], '-': [""],
                        'E': ["Goomba", "Koopa", "Hard Shell", "Hammer Bro"]}
        objectsList = []

        # Convert the final result matrix (with the agent's additions) back to a list of sprites
        for tileRow in range(0, len(level) - 1):
            for tile in range(0, len(level[tileRow])):
                currentTile = level[tileRow][tile]

                # Distinguishing between ground and stair sprite
                if currentTile.name == 'X' and currentTile.x < 2:
                    spriteName = "Ground"
                elif currentTile.name == 'X':
                    spriteName = "Stair"
                elif currentTile.name=="E":
                    if currentTile.y>8 and level[tileRow-1][tile]=="":
                        spriteName =  "Lakitu"
                    else:
                        spriteName = random.choice(charToSprite[currentTile.name])
                elif currentTile.name!="U":
                    spriteName = random.choice(charToSprite[currentTile.name])

                if len(spriteName) > 0:
                    sprite = Sprite(spriteName, currentTile.y, currentTile.x, currentTile.w, currentTile.h)
                    objectsList.append(sprite)

        return objectsList

    # Run the model given internal agent level representation, return the updated full level in agent representation or just additions
    def RunModel(self, level):
        print "RUN MODEL"

        tiles = ['X', 'S', '-', '?', 'Q', 'E', 'o']
        maxAdditions = 30
        numAdditions = 0

        # Use the agent's markov model to make additions using a bottom of order of generation
        width = len(level[0]) -1
        minX = max(0,self.lastXPos-15)
        maxX = min(self.lastXPos+15,width)
        print([minX,maxX])
        for tile in range(minX, maxX):
            for tileRow in range(len(level) - 2, 0, -1):
            

                # A tile depends on the tile before it, the tile below it, and the tile below it and to the left
                prevTiles = level[tileRow][tile].name + level[tileRow + 1][tile].name + level[tileRow + 1][tile + 1].name

                # Agent encounters unseen state
                if prevTiles not in self.distributionMatrix:
                    currentTile = level[tileRow][tile + 1]
                    #print "Unseen State - Choosing random"
                    if random.random()>0.9 and tileRow<14:
                        newTile = Sprite(random.choice(tiles), currentTile.x, currentTile.y, currentTile.w, currentTile.h)
                        level[tileRow][tile + 1] = newTile
                else:
                    currentTile = level[tileRow][tile + 1]

                    # Handling special case with pipes and pipebodies
                    if level[tileRow][tile].name == '<':
                        chosenTile = '>'
                    elif level[tileRow][tile].name == '[':
                        chosenTile = ']'
                    else:

                        probabilities = self.distributionMatrix[prevTiles]

                        # NOTE: You can add code here to limit the amount of agent additions
                        # based on its maximum confidence with respect to a threshold probability

                        # Random sampling based on probability weights
                        
                        tiles = []
                        weights = []
                        for t in probabilities.keys():
                            tiles.append(t)
                            weights.append(probabilities[t])
                        chosenTile = choice(tiles, p=weights)
                        if tileRow==14:
                            chosenTile = level[tileRow+1][tile].name

                        '''
                        
                        # OR -----------------------------

                        # choose tile with max probability
                        maxProb = 0.0
                        chosenTile = ''
                        for t in probabilities.keys():
                            if probabilities[t] > maxProb:
                                chosenTile = t
                                maxProb = probabilities[t]
                        '''
                        
                    # Overrides tile at the specific location with agent's recommendation (keep dimensions & pos the same)
                    if numAdditions<maxAdditions:

                        newTile = Sprite(chosenTile, currentTile.x, currentTile.y, currentTile.w, currentTile.h)
                        if newTile.name!=level[tileRow][tile + 1].name:
                            level[tileRow][tile + 1] = newTile
                            numAdditions+=1
                            

        return level
