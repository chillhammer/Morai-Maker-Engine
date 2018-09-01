using UnityEngine;
using System.Collections;

public class PlayerInfo : MonoBehaviour {
	private float health=100;
	private bool isDead = false;

	public void HurtPlayer(float hurtAmount){
		health -= hurtAmount;

		if(health<=0){
			isDead=true;
		}
	}

	void Update(){
		if (isDead) {
			Destroy(gameObject);	
			Application.LoadLevel (Application.loadedLevelName);

		}
	}
}
