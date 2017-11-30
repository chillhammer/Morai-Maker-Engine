#!/usr/bin/env python

import socket, csv, time
from agent import *
from LSTM import *

TCP_IP = '127.0.0.1'
TCP_PORT = 5016
BUFFER_SIZE = 1024

print("\nAgent loading")
#TODO; pick between options
currAgent = LSTMAgent()
currAgent.LoadModel()
print("Agent loaded\n")

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.bind((TCP_IP, TCP_PORT))
s.listen(1)

conn, addr = s.accept()
#print 'Connection address:', addr
while 1:
    data = conn.recv(BUFFER_SIZE)
    if data:
        #Grab from data file
        source = open(data, "r")
        reader = csv.reader(source)
        readRow = False
        levelSize = []
        levelSprites = []
        for row in reader:
            if readRow:
                levelSprites.append(Sprite(row[0], int(row[1]), int(row[2]), int(row[3]), int(row[4])))
            else:
                levelSize = [int(row[0]), int(row[1])]
                readRow = True

        agentLevel = currAgent.ConvertToAgentRepresentation(levelSprites, levelSize[0], levelSize[1])
        updatedLevel = currAgent.RunModel(agentLevel)
        spriteList = currAgent.ConvertToSpriteRepresentation(updatedLevel)

        finalSpriteList = []
        for s in spriteList:
            addIt = True
            for l in levelSprites:
                if l.x==s.x and s.y==l.y:
                    addIt = False
                    break
            if addIt:
                finalSpriteList.append(s)
        finalSpriteList.sort(key=lambda s: s.x * 16 - s.y)

        # Write additions to file
        printedList = []
        print('Request received')
        with open('./additions.csv', 'w', newline='') as additions_file:
            writer = csv.writer(additions_file)
            for f in finalSpriteList:
                row = [f.name, str(f.x), str(f.y)]
                #print('Row: ' + ','.join(row))
                if not row in printedList:
                    writer.writerow(row)
                    printedList.append(row)
        print('Response written\n')
        conn.close()
        time.sleep(1)
        s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        s.bind((TCP_IP, TCP_PORT))
        s.listen(1)
        conn, addr = s.accept()
