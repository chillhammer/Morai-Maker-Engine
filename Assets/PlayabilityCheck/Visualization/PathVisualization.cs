using Assets.Scripts.Core;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.PlayabilityCheck
{
	public class PathVisualization : MonoBehaviour
	{
		private List<Vector2> path;

		LineRenderer line;
		public float lineZPosition = -1f;

		// Use this for initialization
		private void Start()
		{
			line = gameObject.GetComponent<LineRenderer>();
		}

		//Converts Path to 
		public Vector3[] ConvertPathToLine(List<Vector2> path)
		{
			List<Vector3> vertices = new List<Vector3>();
			Vector2 offset = new Vector2(0.5f, 0.5f);
			foreach (Vector2 node in path)
			{
				Vector3 vertice = node + offset;
				vertice.z = lineZPosition;
				vertices.Add(vertice);
			}
			return vertices.ToArray();
		}

		public void Clear()
		{
			line.positionCount = 0;
		}

		public void SetDrawLine(List<Vector2> path)
		{
			if (path != null)
			{
				line.positionCount = path.Count;
				line.SetPositions(ConvertPathToLine(path));
			}
		}

		public void SetDrawLine(Vector2 source, Vector2 target)
		{
			List<Vector2> linePath = new List<Vector2>();
			linePath.Add(source);
			linePath.Add(target);

			if (linePath != null)
			{
				SetDrawLine(linePath);
			}
		}

	}
}