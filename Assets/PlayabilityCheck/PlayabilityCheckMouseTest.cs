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

		// Use this for initialization
		private void Awake()
		{
			
			DontDestroyOnLoad(gameObject);
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.C))
			{
				Debug.Log("Generating PathFinding");
				pathFinding = new PathFinding.PathFinding();
			}

			//Click!
			if (Input.GetMouseButtonDown(0))
			{
				Vector2 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
				playerPos.x = (int)playerPos.x;
				playerPos.y = (int)playerPos.y;
				Debug.Log("Player Position: x: " + playerPos.x + " - y: " + playerPos.y);

				Vector2 mousePos = Input.mousePosition;
				mousePos = Camera.main.ScreenToWorldPoint(mousePos);
				mousePos.x = (int)mousePos.x;
				mousePos.y = (int)mousePos.y;
				string hasBlock = "";
				if (GridManager.Instance.ContainsGridObject(true, (int)mousePos.x, (int)mousePos.y))
					hasBlock = " - HasBlock!";
				Debug.Log("Mouse Position: x: " + mousePos.x + " - y: " + mousePos.y + hasBlock);

				path = pathFinding.FindPath(playerPos, mousePos);
			}

			for (int i = 0; i < path.Count; ++i)
			{
				if (i == 0) break;
				Debug.DrawLine(path[i - 1], path[i]);
			}
		}
	}
}