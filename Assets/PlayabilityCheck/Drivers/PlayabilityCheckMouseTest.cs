using Assets.Scripts.Core;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.PlayabilityCheck
{
	public class PlayabilityCheckMouseTest : MonoBehaviour
	{

		public PathFinding.PathFinding pathFinding;
		public List<Vector2> path;

		LineRenderer line;
		public float lineZPosition = -1f;

		[SerializeField] Vector2 pos1;
		[SerializeField] Vector2 pos2;

		// Use this for initialization
		private void Awake()
		{
			line = gameObject.GetComponent<LineRenderer>();
			DontDestroyOnLoad(gameObject);
		}

		private void Start()
		{
			pathFinding = new PathFinding.PathFinding();
		}


		void Update()
		{
			//Reset Pathfinding Class - Shouldn't Need To Do More Than Once This. For Debug Purposes
			if (Input.GetKeyDown(KeyCode.C))
			{
				Debug.Log("Generating PathFinding");
				pathFinding = new PathFinding.PathFinding();
			}

			//Click!
			if (Input.GetMouseButtonDown(1))
			{
				if (!Input.GetKey(KeyCode.LeftShift))
					pos1 = GetMousePosition();
				else
					pos2 = GetMousePosition();
			}

			//Path Find!
			if (Input.GetKeyDown(KeyCode.Space))
			{
				path = pathFinding.FindPath(pos1, pos2);
				//Set Up Line To Be Rendered
				if (path != null)
				{
					line.positionCount = path.Count;
					line.SetPositions(ConvertPathToLine(path));
				}
			}
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

		Vector2 GetMousePosition()
		{
			Vector2 mousePos = Input.mousePosition;
			mousePos = Camera.main.ScreenToWorldPoint(mousePos);
			mousePos.x = (int)mousePos.x;
			mousePos.y = (int)mousePos.y;

			string hasBlock = "";
			if (GridManager.Instance.ContainsGridObject(true, (int)mousePos.x, (int)mousePos.y))
				hasBlock = " - HasBlock!";
			Debug.Log("Mouse Position : x: " + mousePos.x + " - y: " + mousePos.y + hasBlock);
			return mousePos;
		}
	}
}