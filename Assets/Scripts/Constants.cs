using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Constants : MonoBehaviour {

	public static string idOne;
	public static int round;

	public static readonly string SOLID = "Solid";
	public static readonly string BREAKABLE = "Breakable";
	public static readonly string POWERUP = "PowerupBlock";
	public static readonly string GROUND = "Ground";
	public static string directory;

	public static bool IsSolid(string tag){
		List<string> solidSet = new List<string>(new string[]{ SOLID, BREAKABLE, POWERUP, GROUND});

		return solidSet.Contains (tag);
	}

	public static string GetLevelName(){
		return idOne + round;
	}

}
