using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Assets.Scripts.UI;

public class TestScene : MonoBehaviour {

	public GameObject player, gate;

	// Use this for initialization
	void Awake () {
		Map map = gameObject.GetComponent<Map> ();
		map.Init ();

		//Blocks
		Dictionary<string,int> dict = new Dictionary<string,int> ();
		dict["Ground"] = Map.GROUND;
		dict["Block"] = Map.BLOCK;
		dict["Stair"] = Map.STAIR;
		dict["Treetop"] = Map.TREETOP;
		dict["Bridge"] = Map.BRIDGE;
		dict["Coin"] = Map.COIN;
		dict["Question"] = Map.QUESTION;
		dict["SmallCannon"] = Map.CANNON_SMALL;
		dict["Cannon"] = Map.CANNON;
		dict["CannonBody"] = Map.CANNON_BODY;
		dict["Bar"] = Map.BAR_1;
		dict["Bar 2"] = Map.BAR_2;
		dict["Bar 3"] = Map.BAR_3;
		dict["Spring"] = Map.SPRING;

		Dictionary<string,int> decorations = new Dictionary<string,int> ();
		decorations["Bush"] = Map.BUSH;
		decorations["Hill"] = Map.HILL;
		decorations["Cloud"] = Map.CLOUD;
		decorations["Tree"] = Map.TREE;
		decorations["Snow Tree"] = Map.SNOW_TREE;
		decorations["Tree 2"] = Map.TREE_TALL;
		decorations["Snow Tree 2"] = Map.SNOW_TREE_TALL;
		decorations["Fence"] = Map.FENCE;
		decorations["Bark"] = Map.BARK;
		decorations["Castle"] = Map.CASTLE;
		decorations["Waves"] = Map.WAVES;

		//, "Goomba", "Koopa", "Hard Shell", "Hammer Bro", "Lakitu"]
		Dictionary<string,int> enemies = new Dictionary<string,int> ();
		enemies["Goomba"] = Map.GOOMBA;
		enemies["Koopa"] = Map.GREEN_KOOPA;
		enemies["Koopa 2"] = Map.RED_KOOPA;
		enemies["Hard Shell"] = Map.BEETLE;
		enemies["Hammer Bro"] = Map.HAMMER_BRO;
		enemies["Lakitu"] = Map.LAKITU;
		enemies["Plant"] = Map.PLANT;
		enemies["Winged Koopa"] = Map.GREEN_KOOPA_WINGED;
		enemies["Winged Koopa 2"] = Map.RED_KOOPA_WINGED;

		// Check level exists
		//test

		string filePath = Constants.directory + "/StreamingAssets/Levels/" + Map.level_name + ".csv";

		if(File.Exists(filePath))
		{
			// - Parse file
			string[] lines = File.ReadAllLines(filePath);
			for (int i = 1; i < lines.Length; i++) {
				string[] line = lines[i].Split(',');
				if (dict.ContainsKey (line [0])) {
					if (line [0] == "Bridge") {
						map.SetBlock (int.Parse (line [1]), int.Parse (line [2])+1, dict [line [0]]);
					} else {
						map.SetBlock (int.Parse (line [1]), int.Parse (line [2]), dict [line [0]]);
					}
				} else if (enemies.ContainsKey (line [0])) {
					map.SetSprite (int.Parse(line [1]), int.Parse(line [2]), enemies [line [0]]);
				} else if (decorations.ContainsKey (line [0])) {
					if (line [0] == "Hill") {
						map.SetDecoration (int.Parse (line [1]), int.Parse (line [2]) + 1, decorations [line [0]]);
					} else if (line [0] == "Cloud") {
						map.SetDecoration (int.Parse (line [1]), int.Parse (line [2]) + 1, decorations [line [0]]);
					} else if (line [0] == "Castle") {
						map.SetDecoration (int.Parse (line [1]) + 2, int.Parse (line [2]) + 2, decorations [line [0]]);
					} else if (line [0] == "Tree") {
						map.SetDecoration (int.Parse (line [1]), int.Parse (line [2]) + 0.5f, decorations [line [0]]);
					} else if (line [0] == "Tree 2") {
						map.SetDecoration (int.Parse (line [1]), int.Parse (line [2]) + 1, decorations [line [0]]);
					} else if (line [0] == "Snow Tree") {
						map.SetDecoration (int.Parse (line [1]), int.Parse (line [2]) + 0.5f, decorations [line [0]]);
					} else if (line [0] == "Snow Tree 2") {
						map.SetDecoration (int.Parse (line [1]), int.Parse (line [2]) + 1, decorations [line [0]]);
					} else if (line [0] == "Waves") {
						map.SetDecoration (int.Parse (line [1]) + 0.5f, int.Parse (line [2]), decorations [line [0]]);
					}else {
						map.SetDecoration(int.Parse(line [1]), int.Parse(line [2]), decorations [line [0]]);
					}
				} else if (line [0] == "Pipe") {
					map.SetBlock(int.Parse(line [1])+1, int.Parse(line [2])+1, Map.PIPE, 2, 2);
				}else if (line [0] == "PipeBody") {
					map.SetBlock(int.Parse(line [1])+1, int.Parse(line [2]), Map.PIPE_BODY, 2, 1);
				}else if (line [0] == "Mario") {
					player.transform.position = new Vector3 (float.Parse (line [1]), float.Parse (line [2]), 0);
				}else if (line [0] == "Flag") {
					gate.transform.position = new Vector3 (float.Parse (line [1]), float.Parse (line [2])-0.5f, 5);
				}
			}
		}
	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.Escape)) {
			//Level name
			LogHandler.Instance.WriteLine ("End Run:  time = "+Time.time);
			FileMenu.prevLoaded = true;
			FileMenu.levelName=Map.level_name;
			SceneManager.LoadScene ("Main");
		}
	}
}
