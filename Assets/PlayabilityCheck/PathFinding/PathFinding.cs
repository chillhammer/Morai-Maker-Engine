using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Assets.Scripts.Core;
using System;

namespace Assets.PlayabilityCheck.PathFinding
{
	public class PathFinding
	{
		public int characterWidth = 1; //assuming character scales from top-left
		public int characterHeight = 1;
		public int characterJumpHeight = 5;
		//how many blocks character falls before removing horizontal in-air movement
		public int blocksFallenUntilCancelSideways = 4;
		//adds to cost of state by multiplying its JumpLength by this value
		public float jumpDeterrentMultiplier = 0.25f;

		public long iterationSearchLimit = 10000;


		private List<PFNode>[] nodes; //Grid of List of Nodes
		private Stack<int> traversedCoordinates; //(x + y*width)

		private int nodeOpenValue = 1; //Status of Node
		private int nodeCloseValue = 2;

		private Algorithms.PriorityQueueB<Location> openLocations;



		public List<Vector2> FindPath(Vector2 start, Vector2 end) {
			#region Setup
			//Stop if end goal is impossible
			if (GridManager.Instance.ContainsGridObject(true, (int)end.x, (int)end.y))
			{
				Debug.LogWarning("AStar Failed. End Location is blocked");
				return null;
			}

			//Reset Values
			nodeOpenValue += 2; //This allows for nodes to be reset without looping through grid
			nodeCloseValue += 2;


			//Convert From Vector to Location
			Location myLocation = new Location((int)start.y * GridManager.Instance.GridWidth + (int)start.x, 0);
			Location endLocation = new Location((int)end.y * GridManager.Instance.GridHeight + (int)end.x, 0);

			

			//First Node To Branch From
			PFNode firstNode = new PFNode();
			firstNode.G = 0;
			firstNode.F = 1;
			firstNode.PX = (ushort)start.x;
			firstNode.PY = (ushort)start.y;
			firstNode.PZ = 0;
			firstNode.Status = nodeOpenValue;

			//Setting Jump Length
			if (GridManager.Instance.ContainsGridObject(true, (int)start.x, (int)start.y - 1))
				firstNode.JumpLength = 0;
			else
				firstNode.JumpLength = (short)(characterJumpHeight * 2);

			//Adding Start Location to Stack
			traversedCoordinates.Push(myLocation.xy);
			openLocations.Push(myLocation);
			#endregion

			bool found = false;
			long iterationCount = 0;
			sbyte[,] direction = new sbyte[8, 2] { {0,-1}, {1,0 }, {0,1}, {-1,0}, {1,-1}, {1,1}, {-1,1}, {-1,-1} };
			//Loop Through Priority Queue 
			while (openLocations.Count > 0) //Add Other Stop Condition Maybe?
			{
				Location current = openLocations.Pop();

				if (nodes[current.xy][current.z].Status == nodeCloseValue) //Ignore Visited
					continue;

				int currentX = current.xy % GridManager.Instance.GridWidth;
				int currentY = current.xy / GridManager.Instance.GridWidth; //Int division truncates off x portion

				//Found Target Path!
				if (current.xy == endLocation.xy)
				{
					nodes[current.xy][current.z] = nodes[current.xy][current.z].UpdateStatus(nodeCloseValue);
					found = true;
					break;
				}

				//Search Limit
				if (iterationCount > iterationSearchLimit)
				{
					Debug.LogWarning("AStar Pathfinding Failed Due to Search Limit");
					return null;
				}

				//Find Successors
				for (int i = 0; i < direction.Length; ++i)
				{
					int successorX  = (ushort)(currentX + direction[i, 0]);
					int successorY  = (ushort)(currentY + direction[i, 1]);
					int successorXY = successorY * GridManager.Instance.GridWidth + successorX;

					//Ignore non-navigable block
					if (HasBlock(successorX, successorY))
						continue;

					
					bool onGround = HasBlock(successorX, successorY - characterHeight); 
					bool atCeiling = HasBlock(successorX, successorY + 1);

					int jumpLength = nodes[current.xy][current.z].JumpLength; //Grabs Old
					int newJumpLength = -1;


					//JumpLength is how long in the air, at max jump height, JumpLength is maxJumpHeight * 2
					//This gives granularity to the stage of jump length
					//Even JumpLength values means the character could go Up, Down, Right, Left
					//Odd  JumpLength values means the character could go Up, Down

					//If Odd, Ignore Right and Left Successors
					if (jumpLength % 2 != 0 && successorX != currentX)
						continue;

					//If Falling, Make Sure Not Going Up
					if (jumpLength >= characterJumpHeight * 2 && successorY > currentY)
						continue;

					//If Falling fast, Make Sure Not Going Sideways
					if (newJumpLength >= characterJumpHeight * 2 + blocksFallenUntilCancelSideways 
						&& successorX != currentX)
						continue;

					//If revisiting, only continue if it can add something new to the table
					if (nodes[successorXY].Count > 0)
					{
						int lowestJump = short.MaxValue;
						bool visitedCouldMoveSideways = false;
						for (int j = 0; j < nodes[successorXY].Count; ++j)
						{
							if (nodes[successorXY][j].JumpLength < lowestJump)
								lowestJump = nodes[successorXY][j].JumpLength;

							if (nodes[successorXY][j].JumpLength % 2 == 0 
								&& nodes[successorXY][j].JumpLength < characterJumpHeight * 2 + blocksFallenUntilCancelSideways)
								visitedCouldMoveSideways = true;
						}

						//Ignore if visited node has shorter jump length and provides more insight
						if (lowestJump <= newJumpLength 
							&& (newJumpLength % 2 != 0 
							|| newJumpLength >= characterJumpHeight * 2 + blocksFallenUntilCancelSideways 
							|| visitedCouldMoveSideways))
							continue;
					}

					//////////////////////////////////////////
					//--Find New Jump Length for Successor--//
					//////////////////////////////////////////
					//Reset to Zero
					if (onGround) 
						newJumpLength = 0;

					//Ceiling
					else if (atCeiling)
					{
						if (successorX == currentX) //Fall Down
							newJumpLength = (short)Mathf.Max(characterJumpHeight * 2, jumpLength + 2); 
						else //Slide Horizontal
							newJumpLength = (short)Mathf.Max(characterJumpHeight * 2 + 1, jumpLength + 1); 
					}

					//Going Up
					else if (successorY > currentY) 
					{
						if (jumpLength < 2) //Boost! Guarantees next move will go Up
							newJumpLength = 3;
						else
							newJumpLength = NextEvenNumber(jumpLength);
					}

					//Going Down
					else if (successorY < currentY) 
					{
						newJumpLength = (short)Mathf.Max(characterJumpHeight * 2, NextEvenNumber(jumpLength));

					}

					//In-Air Side to Side
					else if (successorX != currentX) 
						newJumpLength = jumpLength + 1;

					////////////////////////////////////
					//--Create and Add Node To Queue--//
					////////////////////////////////////
					//Calculate costs
					int successorCost = nodes[current.xy][current.z].G + (int)(newJumpLength * jumpDeterrentMultiplier);

					int distToGoal = (int)( Math.Sqrt(Math.Pow((successorX - end.x), 2) + Math.Pow((successorY - end.y), 2)));
					
					//Create Node
					PFNode newNode = new PFNode();
					newNode.JumpLength = newJumpLength;
					newNode.PX = currentX;
					newNode.PY = currentY;
					newNode.PZ = current.z;
					newNode.G = successorCost;
					newNode.F = successorCost + distToGoal;
					newNode.Status = nodeOpenValue;

					if (nodes[successorXY].Count == 0)
						traversedCoordinates.Push(successorXY);

					nodes[successorXY].Add(newNode);
					openLocations.Push(new Location(successorXY, nodes[successorXY].Count - 1));
				}

				//After adding all possible successors, mark current node as closed
				nodes[current.xy][current.z] = nodes[current.xy][current.z].UpdateStatus(nodeCloseValue);
				iterationCount++;
			}

			if (found)
			{
				List<Vector2> path = new List<Vector2>();
				int posX = (int)end.x;
				int posY = (int)end.y;

				PFNode fPrevNodeTmp = new PFNode();
				PFNode fNodeTmp = nodes[endLocation.xy][0];

				Vector2 fNode = end;
				Vector2 fPrevNode = end;

				int parentXY = fNodeTmp.PY * GridManager.Instance.GridWidth + fNodeTmp.PX;

				//Recursively Build Path
				while (fNode.x != fNodeTmp.PX || fNode.y != fNodeTmp.PY)
				{
					PFNode fNextNodeTmp = nodes[parentXY][fNodeTmp.PZ];

					Vector2 prevNodePath = path[path.Count - 1];
					//Filters out redundant nodes
					if ((path.Count == 0)
						|| (fNodeTmp.JumpLength == 3)
						|| (fNextNodeTmp.JumpLength != 0 && fNodeTmp.JumpLength == 0)                                                                                                       //mark jumps starts
						|| (fNodeTmp.JumpLength == 0 && fPrevNodeTmp.JumpLength != 0)                                                                                                       //mark landings
						|| (fNode.y > path[path.Count - 1].y && fNode.y > fNodeTmp.PY)
						|| (fNode.y < prevNodePath.y && fNode.y < fNodeTmp.PY)
						|| ((HasBlock(fNode.x - 1, fNode.y) || HasBlock(fNode.x + 1, fNode.y))
							&& fNode.y != prevNodePath.y && fNode.x != prevNodePath.x))
						path.Add(fNode);

					fPrevNode = fNode;
					posX = fNodeTmp.PX;
					posY = fNodeTmp.PY;
					fPrevNodeTmp = fNodeTmp;
					fNodeTmp = fNextNodeTmp;
					parentXY = fNodeTmp.PY * GridManager.Instance.GridWidth + fNodeTmp.PX;
					fNode = new Vector2(posX, posY);
				}

				path.Add(fNode);

				return path;
			}

			Debug.LogWarning("AStar Pathfinding failed. Could not find path to goal. ("+end.x+","+end.y+")");
			return new List<Vector2>();
		}



		#region Helpers
		private bool HasBlock(int x, int y)
		{
			return GridManager.Instance.ContainsGridObject(true, x, y);
		}

		private bool HasBlock(float x, float y)
		{
			return GridManager.Instance.ContainsGridObject(true, (int)x, (int)y);
		}

		private int NextEvenNumber(int num)
		{
			if (num % 2 == 0) //Find next even number
				return num + 2;
			else
				return num + 1;
		}
		#endregion

		#region Internal Struct
		internal struct Location
		{
			public Location(int xy, int z)
			{
				this.xy = xy;
				this.z = z;
			}

			public int xy;
			public int z;
		}

		internal struct PFNode
		{
			public int F; // F = gone + heuristic
			public int G;

			//P = Parent
			public int PX;
			public int PY;
			public int PZ;

			public int Status { get; internal set; }
			public int JumpLength { get; internal set; }

			public PFNode UpdateStatus(int newStatus) //Since List Returns Copy of Node
				//We use this to replace existing node.
			{
				PFNode newNode = this;
				newNode.Status = newStatus;
				return newNode;
			}
		}
		#endregion

		#region Internal Class
		internal class ComparePFNodeMatrix : IComparer<Location>
		{
			List<PFNode>[] mMatrix;

			public ComparePFNodeMatrix(List<PFNode>[] matrix)
			{
				mMatrix = matrix;
			}

			public int Compare(Location a, Location b)
			{
				if (mMatrix[a.xy][a.z].F > mMatrix[b.xy][b.z].F)
					return 1;
				else if (mMatrix[a.xy][a.z].F < mMatrix[b.xy][b.z].F)
					return -1;
				return 0;
			}
		}
		#endregion
	}
}