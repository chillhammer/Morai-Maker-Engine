using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeyEgg : Enemy {
	private bool inAir = true;
	private float changeGravitySpeed = 100f;
	public GameObject spikey;
	public override bool Squish (GameObject player){
		return false;
	}





	// Use this for initialization
	void Start () {
		if (Random.Range (0, 2) == 0) {
			velocity = new Vector2 ((-2f), 20);	
		} else {
			velocity = new Vector2 ((2f), 20);	
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (inAir) {
			velocity.y -= changeGravitySpeed * Time.deltaTime;

		}
		Vector2 possibleNewPos = position + Time.deltaTime * velocity;

		if (velocity.y < 0) {
			RaycastHit? belowHit = VerticalCollisionCheck (GetDownPosition (), GetDownPosition () + Vector3.down * (position.y - possibleNewPos.y)); 
			if (belowHit != null) {
				RaycastHit belowRaycastHit = (RaycastHit)belowHit;
				if (Constants.IsSolid (belowRaycastHit.collider.tag)|| belowRaycastHit.collider.tag.Equals("Enemy") ) {
					GameObject spikeyClone = Instantiate<GameObject> (spikey);
					spikeyClone.transform.position = transform.position;
					Destroy (gameObject);
				}
			}	
		}

		SetPosition (possibleNewPos);
	}
}
