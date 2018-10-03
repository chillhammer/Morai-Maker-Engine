using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Assets.Scripts.Core;

namespace Assets.PlayabilityCheck.PathFinding
{
	public class PathFinding
	{
		public int characterWidth = 1;
		public int characterHeight = 1;
		public int characterJumpHeight = 5;


		private List<PFNode>[] nodes; //Grid of List of Nodes
		private Stack<int> traversedCoordinates; //(x + y*width)



		public List<Vector2> FindPath(Vector2 start, Vector2 end) {

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
			firstNode.Status = NodeValue.Open;

			//Setting Jump Length
			if (GridManager.Instance.ContainsGridObject(true, (int)start.x, (int)start.y - 1))
				firstNode.JumpLength = 0;
			else
				firstNode.JumpLength = (short)(characterJumpHeight * 2);

			return new List<Vector2>();
		}




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

		public enum NodeValue
		{
			Open
		}

		internal struct PFNode
		{
			public int F; // F = gone + heuristic
			public int G;

			//P = Parent
			public int PX;
			public int PY;
			public int PZ;

			public object Status { get; internal set; }
			public int JumpLength { get; internal set; }
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