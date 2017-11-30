import tflearn, csv, random, glob, os
import tensorflow as tf
from tflearn import batch_normalization, fully_connected, regression, input_data, dropout, custom_layer, flatten, reshape, embedding
from tflearn.layers.recurrent import bidirectional_rnn, BasicLSTMCell
import numpy as np
from agent import *

class LSTMAgent(Agent):

	def __init__(self):
		object.__init__('LSTMAgent')


	def LoadModel(self):
		self.window_height = 16
		self.window_width = 4
		self.threshold = 0.03
		self.ground_height = 2
		symbol_count = 13

		network = input_data(shape = [None, self.window_height * self.window_width, symbol_count])
		network = bidirectional_rnn(network, BasicLSTMCell(2), BasicLSTMCell(2))
		network = dropout(network, 0.8)
		network = fully_connected(network, self.window_height * symbol_count, activation='prelu')
		network = tf.reshape(network, [-1, self.window_height, symbol_count])
		network = regression(network, optimizer='adagrad', learning_rate=0.005, loss='mean_square', name='target', batch_size=64)

		self.model = tflearn.DNN(network)
		self.model.load('./LSTMmodel/model.tfl')


	# Convert sprite list to grid representation
	def ConvertToAgentRepresentation(self, objectsList, levelWidth, level_height):
		level = [['-' for _ in range(levelWidth)] for _ in range(level_height)]
		symbol_map = {'Ground':'X', 'Block':'S', 'Stair':'X', 'Pipe':'<', 'PipeBody':'[', 'Treetop':'X', 'Bridge':'X',
		              'Coin':'o', 'Question':'?', 'Cannon 1':'B', 'Cannon 2':'B', 'CannonBody':'b', 'Bar':'X', 'Bar 2':'X', 'Bar 3':'X',
					  'Goomba':'E', 'Koopa':'E', 'Koopa 2':'E', 'Hard Shell':'E', 'Hammer Bro':'E', 'Plant':'E', 'Winged Koopa':'E', 'Winged Koopa 2':'E'}

		# Add sprites to level representation
		for sprite in objectsList:
			if sprite.name in symbol_map:
				symbol = symbol_map[sprite.name]

				if symbol == '<':
					# - Construct entire pipe top
					level[sprite.y + 0][sprite.x + 0] = '<'
					level[sprite.y + 0][sprite.x + 1] = '>'
					level[sprite.y - 1][sprite.x + 0] = '['
					level[sprite.y - 1][sprite.x + 1] = ']'
				elif symbol == '[':
					# - Construct both pieces of pipe body
					level[sprite.y][sprite.x + 0] = '['
					level[sprite.y][sprite.x + 1] = ']'
				elif symbol == 'E':
					# - Limit enemies to one tile
					level[sprite.y][sprite.x] = 'E'
				else:
					# - Normal case
					for row in range(sprite.y, sprite.y + sprite.h):
						for column in range(sprite.x, sprite.x + sprite.w):
							level[row][column] = symbol

		# Flip level representation vertically
		return [level[level_height - row - 1] for row in range(level_height)]


	# Clean up grid representation and convert to sprite list
	def ConvertToSpriteRepresentation(self, level):
		level_height = len(level)
		level_width = len(level[0])
		symbol_map = {'X':'Ground', 'Q':'Question', 'S':'Block', 'E':'Goomba', '?':'Question', '<':'Pipe', '[':'PipeBody', 'o':'Coin', 'B':'Cannon 1', 'b':'CannonBody'}
		size_map = {'Default':(1, 1), 'Pipe':(2, 2), 'PipeBody':(2, 1)}

		# Flip level representation vertically
		level = [level[level_height - row - 1] for row in range(level_height)]

		# Fill out pipes
		for level_y in range(level_height):
			for level_x in range(level_width):
				symbol = level[level_y][level_x]
				if symbol == '[':
					if level_x < level_width - 1:
						level[level_y][level_x + 1] = ']'
					else:
						level[level_y][level_x] = '-'
				elif symbol == ']':
					if level_x > 0:
						level[level_y][level_x - 1] = '['
					else:
						level[level_y][level_x] = '-'
				elif symbol == '<':
					if level_y > 0 and level_x < level_width - 1:
						level[level_y + 0][level_x + 1] = '>'
						level[level_y - 1][level_x + 0] = '['
						level[level_y - 1][level_x + 1] = ']'
					else:
						level[level_y][level_x] = '-'
				elif symbol == '>':
					if level_y > 0 and level_x > 0:
						level[level_y + 0][level_x - 1] = '<'
						level[level_y - 1][level_x - 1] = '['
						level[level_y - 1][level_x + 0] = ']'
					else:
						level[level_y][level_x] = '-'

		# Convert map to sprite list
		additions = []
		for level_y in range(level_height - 1, -1, -1):
			for level_x in range(level_width):
				symbol = level[level_y][level_x]
				if symbol in symbol_map:
					name = symbol_map[symbol]

					# - Replace ground with stairs above altitude 2
					name = 'Stair' if name == 'Ground' and level_y >= self.ground_height else name

					size_x, size_y = size_map[name] if name in size_map else size_map['Default']
					additions.append(Sprite(name, level_x, level_y, size_x, size_y))

		return additions


	# Run the model to generate new suggestions on the grid representation
	def RunModel(self, level):
		level_height = len(level)
		level_width = len(level[0])
		window_height = self.window_height
		window_width = self.window_width
		symbols = ['-', 'X', 'Q', 'S', 'E', '?', '<', '[', ']', '>', 'o', 'B', 'b']

		# Pad ground by one block
		level.pop(0)
		level.append(['X' if level[-1][i] == 'X' else '-' for i in range(level_width)])

		for level_x in range(level_width):

			# Extract obfuscated window from level
			window = np.zeros([window_height, window_width, len(symbols)])
			for window_y in range(window_height):
				for window_x in range(window_width):
					y = level_height - window_height + window_y
					x = level_x - window_width + window_x

					# - Handle bounding issues
					symbol = None
					if y < 0 or x < 0:
						symbol = 'X' if level_height - y <= self.ground_height else '-'
					else:
						symbol = level[y][x]

					window[window_y][window_x][symbols.index(symbol)] = 1

			# Run the model
			window = np.reshape(window, [window_height * window_width, len(symbols)])
			result = self.model.predict([window])[0]

			# Update level representation with model results
			for window_y in range(window_height):
				y = level_height - window_height + window_y
				x = level_x

				# - Make sure prediction is in bounds
				if y < 0:
					continue

				# - Normalize the result distribution
				one_hot = result[window_y]
				if sum(one_hot) == 0:
					continue
				one_hot = np.divide(one_hot, sum(one_hot))

				# - Thresholding based off result distribution
				one_hot = [i if i >= self.threshold else 0 for i in one_hot]
				if sum(one_hot) == 0:
					continue
				one_hot = np.divide(one_hot, sum(one_hot))

				# - Probabilitic sampling from model results
				level[y][x] = np.random.choice(symbols, p=one_hot)

		# Remove ground padding
		level.pop()
		level.insert(0, ['-' for i in range(level_width)])

		return level