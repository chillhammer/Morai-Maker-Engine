using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.PlayabilityCheck.PathFinding
{
	public class PathFinding
	{
		public int characterWidth = 1;
		public int characterHeight = 1;
		public int characterJumpHeight = 5;

		public List<Vector2> FindPath(Vector2 start, Vector2 end)
		{

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

		internal struct PFNode
		{
			public int F; // F = gone + heuristic
			public int G;

			//P = Parent
			public int PX;
			public int PY;
			public int PZ;
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