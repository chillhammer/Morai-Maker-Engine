using UnityEngine;
using System.Collections;

public class Block : BaseSprite {

	public void HitBelow(GameObject hitBy){

		Mario m = hitBy.GetComponent<Mario> ();
		RaycastHit? above = VerticalCollisionCheck (GetPosition(position), GetUpPosition()+Vector3.up*1.5f);
		if (above != null) {
			RaycastHit aboveHit = (RaycastHit)above;
			if (m!=null && aboveHit.collider.tag.Equals ("Coin")) {
				aboveHit.collider.GetComponent<Coin> ().CollectCoin(m);
			} else if (aboveHit.collider.tag.Equals ("Enemy")) {
				aboveHit.collider.GetComponent<Enemy> ().DestroySprite ();
			}
		}

		if (m!=null && m.IsLarge ()) {
			LogHandler.Instance.WriteLine(m.name +" BlockEmptyDestroy:  time = "+Time.time);
			DestroySprite ();//overwrite given time
		}
	}


}
