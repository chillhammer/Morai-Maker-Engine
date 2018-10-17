using Assets.Scripts.Core;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.PlayabilityCheck
{
	public class PlayabilityCheckLoadLevel : MonoBehaviour
	{

		public string LevelName = "andrew1";

		// Use this for initialization
		private void Awake()
		{
			Constants.idOne = "andrew1";
			Constants.round = 1;
			Constants.directory = Application.dataPath;
			gameObject.AddComponent<LogHandler>();
		}
		void Start()
		{
			string filePath = Constants.directory + "/StreamingAssets/Levels/" + LevelName + ".csv";
			if (File.Exists(filePath))
			{
				// - Parse file
				string[] lines = File.ReadAllLines(filePath);
				string[] gridSize = lines[0].Split(',');
				GridManager.Instance.SetGridSize(int.Parse(gridSize[0]), int.Parse(gridSize[1]), false);
				for (int i = 1; i < lines.Length; i++)
				{
					string[] line = lines[i].Split(',');
					GridManager.Instance.AddGridObject(SpriteManager.Instance.GetSprite(line[0]), int.Parse(line[1]), int.Parse(line[2]), false);
				}

				//Run Game
				Map.level_name = LevelName;
				SceneManager.LoadScene("LevelTest");
			}
			else
			{
				Debug.LogWarning("Error! Cannot find level at " + filePath);
			}

		}
	}
}