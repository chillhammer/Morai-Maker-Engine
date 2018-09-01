#!/usr/bin/env python
#scp ./testserver.py eilab@128.61.69.16:./Desktop/UserStudyResults/testserver.py
import socket, csv, time, struct
import numpy as np
import tflearn
import tensorflow as tf
from tflearn import conv_2d, max_pool_2d, local_response_normalization, batch_normalization, fully_connected, regression, input_data, dropout, custom_layer, flatten, reshape, embedding,conv_2d_transpose

def GetEmptyMap(allNames):
    emptyMap = []
    for x in range(0, 40):
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

def StringToList(strVal):
    listVal = []
    for s in strVal:
        listVal.append(int(s))
    return listVal

def StringToMap(strVal, allNames):
    emptyMap = GetEmptyMap(allNames)
    splits1 = strVal.split("-")
    for s1 in splits1:
        if len(s1)>0:
            splits2 = s1.split("*")
            if len(splits2)>=3:
                try:
                    emptyMap[int(splits2[0])][int(splits2[1])][int(splits2[2])] = 1
                except ValueError:
                    print("VALUE ERRORED")
    return emptyMap

#In this 8 by 8 localized on the added point find what prompted it
# shape=(4, 4, 34, 8)
def GetExplanation(weights, mapVal, x, y, allNames):
    maxPatch = [0,0]
    maxVal = -5
    for xi in range(-4+x, x):
        for yi in range(-4+y, y):
            thisPatchMax = -5
            bestfilter = -1
            for j in range(0, 8):
                thisScore = 0
                for xj in range(xi, xi+4):
                    for yj in range(yi, yi+4):
                        if xj>=0 and xj<40 and yj>=0 and yj<15:
                            for z in range(0, 34):
                                try: 
                                    thisScore+=float((weights[xj-xi][yj-yi][z][j]*mapVal[xj][yj][z]))
                                except IndexError:
                                    thisScore+=0
                if thisScore>thisPatchMax:
                    thisPatchMax = thisScore
                    bestfilter = j
            if thisPatchMax>maxVal:
                maxPatch = [xi,yi, bestfilter]
                maxVal = thisPatchMax
    buildString = ""+str([maxPatch[0]-x, maxPatch[1]-y, maxVal, "filter: "+str(maxPatch[2])])+"\n"
    for y in range(maxPatch[1]+3, maxPatch[1]-1, -1):
        line = ""
        for x in range(maxPatch[0], maxPatch[0]+4):
            if x>=0 and x<40 and y>=0 and y<15:
                if 1 in mapVal[x][y]:
                    line+=(allNames[mapVal[x][y].index(1)])+"-"
                else:
                    line+=" -"
            else:
                line+="0-"
        buildString+=line+"\n"
    return buildString

allNames = ['Ground', 'Stair', 'Treetop', 'Block', 'Bar', 'Koopa', 'Koopa 2', 'PipeBody', 'Pipe', 'Question', 'Coin', 'Goomba', 'CannonBody', 'Cannon', 'Lakitu', 'Bridge', 'Hard Shell', 'SmallCannon', 'Plant', 'Waves', 'Hill', 'Castle', 'Snow Tree 2', 'Cloud 2', 'Cloud', 'Bush', 'Tree 2', 'Bush 2', 'Tree', 'Snow Tree', 'Fence', 'Bark', 'Flag', 'Mario']
actions = ['Ground', 'Stair', 'Treetop', 'Block', 'Bar', 'Koopa', 'Koopa 2', 'PipeBody', 'Pipe', 'Question', 'Coin', 'Goomba', 'CannonBody', 'Cannon', 'Lakitu', 'Bridge', 'Hard Shell', 'SmallCannon', 'Plant', 'Waves', 'Hill', 'Castle', 'Snow Tree 2', 'Cloud 2', 'Cloud', 'Bush', 'Tree 2', 'Bush 2', 'Tree', 'Snow Tree', 'Fence', 'Bark', 'Nothing']


networkInput = tflearn.input_data(shape=[None, 40, 15, len(allNames)])
conv = conv_2d(networkInput, 8,4, activation='leaky_relu')
conv2 = conv_2d(conv, 16,3, activation='leaky_relu')
conv3 = conv_2d(conv2, 32,3, activation='leaky_relu')
fc = tflearn.fully_connected(conv3, 40*15*len(actions), activation='leaky_relu')
mapShape = tf.reshape(fc, [-1,40,15,len(actions)])
network = tflearn.regression(mapShape, optimizer='adam', metric='accuracy', loss='mean_square', learning_rate=0.004)
model = tflearn.DNN(network)
#model.load("testFull.tflearn")#smbANDtestFull
print ("MODEL LOADED")
#print ("CONV 1: "+str(conv.W))

TCP_IP = '127.0.0.1'
TCP_PORT = 5015
BUFFER_SIZE = 20400

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.bind((TCP_IP, TCP_PORT))
s.listen(1)

conn, addr = s.accept()
prevMap = None
prevAction = None
prevStartX = -1
rejections = []#HOLD ON TO ALL REJECTIONS. NEVER ADD AGANE
#WAIT FOR PROMPT
minimumInteractionConfidence = 0.01
confidenceDelta = 0.001

while 1:
    try:
        data = conn.recv(BUFFER_SIZE)

        if len(data)>0:
            #print("Data: "+str(data)+", "+str(len(data)))
            dataSplits = data.split("|")


            currMap = StringToMap(dataSplits[0], allNames)

            thisStartX = int(dataSplits[1])
            #Retrain
            if not prevMap==None and abs(thisStartX-prevStartX)<30:
                anyChange = False
                differenceX = thisStartX-prevStartX
                for x in range(0, 40):
                    if x+differenceX<40 and x+differenceX>=0:
                        for y in range(0, 15):
                            for z in range(0, len(actions)):
                                if prevAction[x+differenceX][y][z]>0:
                                    if currMap[x][y][z]>0:
                                        anyChange = True
                                        prevAction[x+differenceX][y][z]+=0.1
                                    else:
                                        anyChange = True
                                        prevAction[x+differenceX][y][z]=-0.1
                                        
                                        print ("REMOVED: "+str([actions[z], x+thisStartX,y]))
                                elif currMap[x][y][z]>0:
                                    anyChange = True
                                    prevAction[x+differenceX][y][z]+=0.1
                #LEARN
                if anyChange:
                    minimumInteractionConfidence+=confidenceDelta
                    print ("RETRAIN")
                    model.fit([prevMap], 
                    Y_targets=[prevAction], 
                    n_epoch=1, 
                    shuffle=False, 
                    show_metric=False, 
                    snapshot_epoch=False,
                    batch_size=1,
                    run_id='activeLearning')


            predY = model.predict([currMap])
            thisAction = GetEmptyMap(actions)

            #REPLY
            toString = ""
            nothingSent = True
            maxToSend = []
            maxVal = -1

            sents = []
            for x in range(0, 40):
                for y in range(0, 15):
                    for z in range(0, len(actions)):
                        if predY[0][x][y][z]>minimumInteractionConfidence and currMap[x][y][z]==0 and len(sents)<30:
                            if not [actions[z], x+thisStartX, y] in rejections:
                                conn.send(str(x)+","+str(y)+","+str(z)+","+str( predY[0][x][y][z])+"*")
                                sents.append([x,y,z])
                                print ("SENDING: "+str([actions[z], x+thisStartX,y, predY[0][x][y][z]]))
                                rejections.append([actions[z], x+thisStartX,y])
                                #print ("     EXPLANATION: "+GetExplanation(convW, currMap, x, y, allNames))
                                thisAction[x][y][z] = predY[0][x][y][z]
                                nothingSent = False
                        elif nothingSent:
                            if predY[0][x][y][z]>maxVal and currMap[x][y][z]==0 and not z==len(actions)-1:
                                if not [actions[z], x+thisStartX, y] in rejections:
                                    maxToSend = [x,y,z,predY[0][x][y][z]]
                                    maxVal = predY[0][x][y][z]
            #MUST DO SOMETHING
            if nothingSent:
                thisAction[x][y][z] = minimumInteractionConfidence+confidenceDelta
                sents.append([x,y,z])
                print ("FAILURE CASE: "+str([actions[maxToSend[2]], maxToSend[0], maxToSend[1], maxToSend[3]]))
                conn.send(str(maxToSend[0])+","+str(maxToSend[1])+","+str(maxToSend[2])+","+str( maxToSend[3])+"*")
            conn.close()#Close
            
            #PRINT MAP
            '''
            currMapStr = ""
            for y in range(14, -1, -1):
                line = ""
                for x in range(0, 40):
                    if 1 in currMap[x][y]:
                        line+= allNames[currMap[x][y].index(1)]+"-"
                    else:
                        line+="      -"
                currMapStr+=line+"\n"
            print (currMapStr)
            '''
            #EXPLANATIONS
            convW = np.array(model.get_weights(conv.W))
            for sent in sents:
                print ("---")
                print ("FOR SENT: "+str([actions[sent[2]], sent[0], sent[1], predY[0][sent[0]][sent[1]][sent[2]]]))
                if 1 in currMap[sent[0]][sent[1]]:
                    #WHY IS THIS NOT HIT????
                    print ("IN CURR MAP: "+str(allNames[currMap[sent[0]][sent[1]].index(1)]))
                print ("EXPLAIN:\n"+GetExplanation(convW, currMap, sent[0], sent[1], allNames))
            
            prevMap = list(currMap)
            prevAction = list(thisAction)
            prevStartX = thisStartX
            conn, addr = s.accept()#Reset
    except KeyboardInterrupt:
        conn.close()
        del conn
        break
