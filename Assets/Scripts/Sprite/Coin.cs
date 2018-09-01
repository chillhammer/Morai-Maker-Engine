using UnityEngine;
using System.Collections;

public class Coin : BaseSprite {

	public void CollectCoin(Mario m){
		LogHandler.Instance.WriteLine(m.name+" got "+spriteName);
		//Score update
		LevelHandler.Instance.ScoreUpdate(m.name);
		DestroySprite();
	}
}
