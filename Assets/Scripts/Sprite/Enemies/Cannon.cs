using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : BaseSprite {
	public GameObject bulletBill;
	public Vector2 relativeBarrelPosition;
	private const float MAX_WAIT_TIME = 3.0f;
	private float timer = 0.0f;
	private float maxRange = 10;

	private void Fire(bool right){
		timer = 0;
		GameObject bulletBillClone = Instantiate<GameObject> (bulletBill);
		bulletBillClone.transform.position = right ? GetRightPosition ()+Vector3.up*relativeBarrelPosition.y : GetLeftPosition ()+Vector3.up*relativeBarrelPosition.y;
		bulletBillClone.GetComponent<BulletBill> ().StartFire (gameObject, right);
	}
	
	// Update is called once per frame
	void Update () {
		if (timer < MAX_WAIT_TIME) {
			timer += Time.deltaTime;
		} else {
			RaycastHit? leftHit = HorizontalCollisionCheck (GetPosition (position)+Vector3.up*relativeBarrelPosition.y*2, GetLeftPosition ()+Vector3.left*(maxRange)+Vector3.up*relativeBarrelPosition.y*2);
			if (leftHit != null) {
				RaycastHit leftRaycastHit = (RaycastHit)leftHit;
				if (leftRaycastHit.collider.tag.Equals ("Player")) {
					Fire (false);
				}
			} else {
				RaycastHit? rightHit = HorizontalCollisionCheck (GetPosition (position)+Vector3.up*relativeBarrelPosition.y*2, GetRightPosition ()+Vector3.right*(maxRange)+Vector3.up*relativeBarrelPosition.y*2);
				if (rightHit!=null) {
					RaycastHit rightRaycastHit = (RaycastHit)rightHit;
					if(rightRaycastHit.collider.tag.Equals("Player")){
						Fire (true);
					}
				}
			}
		}
	}
}
