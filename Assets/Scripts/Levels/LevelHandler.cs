using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class LevelHandler : MonoBehaviour {
	public float endOfLevel;
	public static LevelHandler Instance;
	public string levelName;

	private GameObject[] players;
	Dictionary<string,int> playerToScore;
	public static int lives;

	// Use this for initialization
	void Awake () {
		Instance = this;
	}

	void Start(){
		endOfLevel = GameObject.Find ("Gate").transform.position.x;
		playerToScore = new Dictionary<string, int> ();

		players = new GameObject[]{ GameObject.Find ("Player") };
		for (int x = 0; x < players.Length; x++) {
			if (players [x] != null) {
				playerToScore.Add (players [x].name, 0);
			}
		}

	}

	public void ScoreUpdate(string playerName){
		playerToScore [playerName] = playerToScore [playerName] + 1000;
	}
	
	// Update is called once per frame
	void Update () {
		bool dead = true;
		for (int i = 0; i < players.Length; i++) {
			if (players [i] != null) {
				if (players [i].transform.position.x > endOfLevel) {
					//Set Scores
					foreach (KeyValuePair<string,int> kvp in playerToScore) {
						LogHandler.Instance.WriteLine (kvp.Key + " had score " + kvp.Value);
					}
					players [i].GetComponent<Mario> ().WriteTimeToPlay ();

					LogHandler.Instance.WriteLine ("Win!");


					SceneManager.LoadScene ("LevelTest");
				}
				if (!players [i].GetComponent<Mario> ().IsDead ()) {
					dead = false;
				}
			}

		}

		if (dead) {
			//Set Scores
			foreach (KeyValuePair<string,int> kvp in playerToScore) {
				LogHandler.Instance.WriteLine (kvp.Key + " had score " + kvp.Value);
			}

			if (lives > 0) {
				//Tell writer to close 
				lives -= 1;
				SceneManager.LoadScene (levelName);
			} else {
				LogHandler.Instance.WriteLine ("Lost!");

				SceneManager.LoadScene ("LevelTest");
			}
		}
	}
}
