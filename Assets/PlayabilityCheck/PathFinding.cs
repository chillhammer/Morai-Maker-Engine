using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFinding
{


	#region Internal Class
	internal class ComparePFNodeMatrix : IComparer<Location>
	{
		#region Variables Declaration
		List<PathFinderNodeFast>[] mMatrix;
		#endregion

		#region Constructors
		public ComparePFNodeMatrix(List<PathFinderNodeFast>[] matrix)
		{
			mMatrix = matrix;
		}
		#endregion

		#region IComparer Members
		public int Compare(Location a, Location b)
		{
			if (mMatrix[a.xy][a.z].F > mMatrix[b.xy][b.z].F)
				return 1;
			else if (mMatrix[a.xy][a.z].F < mMatrix[b.xy][b.z].F)
				return -1;
			return 0;
		}
		#endregion
	}
	#endregion
}
