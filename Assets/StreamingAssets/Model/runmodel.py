#!/usr/bin/env python

import socket
import csv, sys, time, os

TCP_IP = '127.0.0.1'
TCP_PORT = 5016
BUFFER_SIZE = 1024

# Remove old additions
if os.path.isfile("./additions.csv"):
    os.remove("./additions.csv")

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect((TCP_IP, TCP_PORT))
s.send(sys.argv[1])
s.close()

time.sleep(2)

while not os.path.isfile("./additions.csv"):
    pass

# Avoids race condition where file exists but is not readable
while True:
    try:
        temp = open("./additions.csv", "r")
        break
    finally:
        temp.close()
