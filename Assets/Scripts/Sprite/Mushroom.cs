using UnityEngine;
using System.Collections;

public class Mushroom : BaseSprite {
	private bool activate = false;
	private bool inAir = false;
	private float changeGravitySpeed = 20f;
	private float changeMoveSpeed = 4f;

	void Update(){
		//activate means that the mushroom is actually moving around on it's own
		if (activate) {

			if (Mathf.Abs(velocity.x)>0 && velocity.y==0) {
				RaycastHit? below = VerticalCollisionCheck (GetPosition (position), GetDownPosition());
				if (below == null) {
					inAir = true;
				} else {
					RaycastHit belowHit = (RaycastHit)below;
					if (!Constants.IsSolid (belowHit.collider.tag)) {
						inAir = true;
					}
				}
			}

			if (inAir) {
				velocity.y -= changeGravitySpeed * Time.deltaTime;
			}

			Vector2 possibleNewPos = position + Time.deltaTime * velocity;

			if (velocity.x < 0) {//LEFT CHECK
				RaycastHit? leftHit = HorizontalCollisionCheck (GetPosition (position), GetLeftPosition ()+Vector3.left*(position.x-possibleNewPos.x), 0f, 0.9f);
				if (leftHit!=null) {
					RaycastHit leftRaycastHit = (RaycastHit)leftHit;
					if (Constants.IsSolid (leftRaycastHit.collider.tag)) {
						possibleNewPos.x = leftRaycastHit.point.x + GetWidth () / 2.0f;
						velocity.x = changeMoveSpeed;
					} else if(leftRaycastHit.collider.tag.Equals("Player")){
						Mario m = leftRaycastHit.collider.GetComponent<Mario> ();
						m.SetLarge ();
						DestroySprite ();
					}
				}
			}
			else if (velocity.x > 0) {//RIGHT CHECK
				RaycastHit? rightHit = HorizontalCollisionCheck (GetPosition (position), GetRightPosition()+Vector3.right*(possibleNewPos.x-position.x), 0f, 0.9f);
				if (rightHit!=null) {
					
					RaycastHit rightRaycastHit = (RaycastHit)rightHit;
					if (Constants.IsSolid (rightRaycastHit.collider.tag)) {
						possibleNewPos.x = rightRaycastHit.point.x - GetWidth () / 2.0f;
						velocity.x = -1 * changeMoveSpeed;
					}
					else if(rightRaycastHit.collider.tag.Equals("Player")){
						Mario m = rightRaycastHit.collider.GetComponent<Mario> ();
						m.SetLarge ();
						DestroySprite ();
					}
				}
			}

			if (velocity.y < 0) {
				RaycastHit? belowHit = VerticalCollisionCheck (GetDownPosition (), GetDownPosition () + Vector3.down * (position.y - possibleNewPos.y)); 
				if (belowHit != null) {
					RaycastHit belowRaycastHit = (RaycastHit)belowHit;
					if (Constants.IsSolid (belowRaycastHit.collider.tag)) {
						possibleNewPos.y = belowRaycastHit.point.y + GetHeight () / 2.0f;
						inAir = false;
						velocity.y = 0;
					}
					else if(belowRaycastHit.collider.tag.Equals("Player")){
						Mario m = belowRaycastHit.collider.GetComponent<Mario> ();
						m.SetLarge ();
						DestroySprite ();
					}

				}	
			}

			SetPosition (possibleNewPos);
		}
	}

	public void Activate(){
		activate = true;
		velocity = new Vector2 (changeMoveSpeed, 0);
	}
}
