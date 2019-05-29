using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour {
	//Dictionary of blockMaps
	Dictionary<int,Dictionary<int,GameObject>> blockMap;
	Dictionary<int,Dictionary<int,int>> enemies;

	//Block Constants
	public static readonly int GROUND = 0;
	public static readonly int BLOCK = 1;
	public static readonly int PIPE = 2;
	public static readonly int PIPE_BODY = 3;
	public static readonly int QUESTION = 4;
	public static readonly int STAIR = 5;
	public static readonly int COIN = 6;
	public static readonly int TREETOP = 7;
	public static readonly int BRIDGE = 8;
	public static readonly int CANNON_SMALL = 9;
	public static readonly int CANNON = 10;
	public static readonly int CANNON_BODY = 11;
	public static readonly int BAR_1 = 12;
	public static readonly int BAR_2 = 13;
	public static readonly int BAR_3 = 14;
	public static readonly int SPRING = 15;

	//Enemy Constants
	public static readonly int GOOMBA = 0;
	public static readonly int GREEN_KOOPA = 1;
	public static readonly int RED_KOOPA = 2;
	public static readonly int PLANT = 3;
	public static readonly int BEETLE = 4;
	public static readonly int LAKITU = 5;
	public static readonly int GREEN_KOOPA_WINGED = 6;
	public static readonly int RED_KOOPA_WINGED = 7;
	public static readonly int HAMMER_BRO = 8;

	//Decoration Constants
	public static readonly int BUSH = 0;
	public static readonly int BUSH_2 = 1;
	public static readonly int HILL = 2;
	public static readonly int CLOUD = 3;
	public static readonly int CLOUD_2 = 4;
	public static readonly int TREE = 5;
	public static readonly int SNOW_TREE = 6;
	public static readonly int TREE_TALL = 7;
	public static readonly int SNOW_TREE_TALL = 8;
	public static readonly int FENCE = 9;
	public static readonly int BARK = 10;
	public static readonly int CASTLE = 11;
	public static readonly int WAVES = 12;

	//Block Prefabs
	public GameObject[] blocks;
	public GameObject[] enemyList;
	public GameObject[] decorations;

	//Instance to allow this to act as a singleton
	public static Map Instance;
	public static string level_name;

	// Pathfinding Bot
	[SerializeField] private GameObject pathBot;
	[HideInInspector] public MarioBot pathBotInstance;
	public static List<Vector2> pathForBots;
    public static int botsPathCompleted;

	private Vector2 currCameraPosition, prevCameraPosition;
	private bool initialSpawn = false;

	private int minY;

	public void Init(){
		Instance = this;
		blockMap = new Dictionary<int,Dictionary<int,GameObject>> ();
		enemies = new Dictionary<int,Dictionary<int,int>> ();

		GetCameraPosition ();
		prevCameraPosition = currCameraPosition;

	}

	void GetCameraPosition(){
		int xPos = ((int)Mathf.Floor (Camera.main.transform.position.x))/16;
		currCameraPosition = new Vector2(xPos, Mathf.Floor (Camera.main.transform.position.y));
	}

	void Update(){
		if (!initialSpawn) {
			SpawnEnemies (0, 32);
			initialSpawn = true;

            // A Star Bots //////////
            if (pathForBots != null) {
                botsPathCompleted = 0;
				Vector3 spawnPosition = new Vector3(pathForBots[0].x, pathForBots[0].y, -10);
				
                // Initial Bot Spawn
				Instantiate(pathBot, spawnPosition, Quaternion.identity).GetComponent<MarioBot>();

                int numOfBots = 99;
				for (int i = 0; i < numOfBots; i++)
				{
					pathBotInstance = Instantiate(pathBot, spawnPosition + Random.Range(-0.1f, 0.1f) * Vector3.right, Quaternion.identity).GetComponent<MarioBot>();
					pathBotInstance.xOffsetMagnitude = 0.5f;
				}
                GameObject.Find("BotsCompleted").GetComponent<UnityEngine.UI.Text>().text = "Bots Completed: 0/100";
                
            }
		}
        ////////////////////////////

		GetCameraPosition ();

		if (prevCameraPosition.x < currCameraPosition.x) {
			//Spawn Enemies
			float difference = currCameraPosition.x-prevCameraPosition.x;
			float prevMax = prevCameraPosition.x*16 + 32;
			float newMax = prevMax + difference*16;

			SpawnEnemies ((int)prevMax, (int)newMax);
			prevCameraPosition = currCameraPosition;
		}
	}

	private void SpawnEnemies(int xStart, int xEnd){
		for (int x = xStart; x <= xEnd; x++) {
			if (enemies.ContainsKey (x)) {
				foreach (int y in enemies[x].Keys) {
					int enemyType = enemies [x] [y];
					GameObject cloneBlock = Instantiate<GameObject> (enemyList [enemyType]);
					cloneBlock.transform.position = new Vector3 (x, y+0.4f, 0);
				}
			}
		}
	}

	public void SetBlock(int x, int y, int blockType, int w, int h){
		
		GameObject cloneBlock = Instantiate<GameObject> (blocks [blockType]);
		cloneBlock.transform.position = new Vector3 (x, y, 0);



		//Add it to the dictionary
		for (int xi = x; xi < x + w; xi++) {
			if (!blockMap.ContainsKey (xi)) {
				blockMap.Add (xi, new Dictionary<int,GameObject> ());
			}

			for (int yi = y; yi < y + h; yi++) {
				if (!blockMap [xi].ContainsKey (yi)) {
					blockMap [xi].Add (yi, cloneBlock);
				} else {
					Destroy (blockMap [xi] [yi]);
					blockMap [xi] [yi] = cloneBlock;

				}
			}
		}
	}
	public void SetBlock(int x, int y, int blockType){
		SetBlock (x, y, blockType, 1, 1);
	}

	public void SetDecoration(float x, float y, int blockType){
		GameObject cloneBlock = Instantiate<GameObject> (decorations [blockType]);
		cloneBlock.transform.position = new Vector3 (x, y, 5);
	}

	public void SetSprite(int x, int y, int enemyType){
		if (!enemies.ContainsKey (x)) {
			enemies.Add (x, new Dictionary<int,int> ());
		}

		if (!enemies [x].ContainsKey (y)) {
			enemies [x].Add (y, enemyType);
		} 
		else {
			enemies [x] [y] = enemyType;
		}
	}

	public GameObject GetBlock(int x, int y){
		if(blockMap.ContainsKey(x)){
			if(blockMap[x].ContainsKey(y)){
				if (y<minY){
					minY = y;
				}

				return blockMap[x][y];
			}
		}

		return null;
	}

	public bool HasBlock(int x, int y){
		return blockMap.ContainsKey (x) && (blockMap [x].ContainsKey (y) || y<minY);
	}

	public void DestroySprite(int x, int y, GameObject g){
		if (blockMap.ContainsKey (x) && blockMap[x].ContainsKey(y) && blockMap[x][y]!=null) {
			BaseSprite bs = blockMap [x] [y].GetComponent<BaseSprite> ();
			if (bs != null) {
				bs.DestroySprite ();
			} 
			else {
				Destroy (blockMap [x] [y]);
			}

			blockMap [x] [y] = null;
		} 
		else {
			BaseSprite bs = g.GetComponent<BaseSprite> ();
			if (bs != null) {
				bs.DestroySprite ();
			} 
			else {
				Destroy (g);
			}
		}
	}
}
