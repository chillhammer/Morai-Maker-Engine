using UnityEngine;
using System.Collections;

public class Lava : Enemy {


	public override void Burn (string playerName){
	}

	public override bool Squish (GameObject player){
		Mario m = player.GetComponent<Mario> ();
		if (m != null) {
			m.Kill (this);
		}
		return false;
	}
}
