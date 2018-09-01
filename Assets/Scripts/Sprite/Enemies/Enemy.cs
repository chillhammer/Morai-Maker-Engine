using UnityEngine;
using System.Collections;

public class Enemy : BaseSprite {
	public SpriteAnimator myAnimator;

	//Called when a player hits this enemy from above (returns true if squished)
	public virtual bool Squish(GameObject player){
		return true;
	}

	//Called when hit by a fire
	public virtual void Burn(string playerName){
		if (LogHandler.Instance != null) {
			LogHandler.Instance.WriteLine (playerName + " Firekill: EnemyType = " + spriteName + " time = " + Time.time);
		}
		DestroySprite ();
	}
}
