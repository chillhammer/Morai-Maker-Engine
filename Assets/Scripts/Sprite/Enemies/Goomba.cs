using UnityEngine;
using System.Collections;

public class Goomba : Enemy {
	private bool inAir = true;
	private float changeGravitySpeed = 100f;
	private float changeMoveSpeed = 3f;

	private Vector2 mapPosition, prevMapPosition;

	private bool fallDeath, squishDeath;
	private float deathTimer = 0;
	private float deathTimerMax = 1;

	void Start(){
		velocity = new Vector2 (-1 * changeMoveSpeed, 0);
		myAnimator.SetAnimation ("Walk", false);
		mapPosition = new Vector2 (Mathf.Floor (position.x), Mathf.Floor (position.y));
		prevMapPosition = mapPosition;
	}

	public override bool Squish (GameObject player){
		if (!squishDeath) {
			deathTimer = deathTimerMax;
			squishDeath = true;
			GetComponent<BoxCollider> ().enabled = false;
			myAnimator.SetAnimation ("Dead", false);
			myAnimator.paused = true;
			LogHandler.Instance.WriteLine(player.name +" GoombaSquish:  time = "+Time.time);
			return false;
		} else {
			return true;
		}
	}

	private void GoombaMoveLogic(){
		int startAnimation = -1;
		mapPosition = new Vector2 (Mathf.Floor (position.x), Mathf.Floor (position.y));
		if (mapPosition != prevMapPosition) {
			if (Mathf.Abs (velocity.x) > 0 && velocity.y == 0) {
				RaycastHit? below = VerticalCollisionCheck (GetPosition (position), GetDownPosition ());
				if (below == null) {
					inAir = true;
				} else {
					RaycastHit belowHit = (RaycastHit)below;
					if (!Constants.IsSolid (belowHit.collider.tag)&& !belowHit.collider.tag.Equals("Enemy") ) {
						inAir = true;
					} 
				}
			}
		}

		if (inAir) {
			velocity.y -= changeGravitySpeed * Time.deltaTime;

		}
		Vector2 possibleNewPos = position + Time.deltaTime * velocity;

		if (velocity.y < 0) {
			RaycastHit? belowHit = VerticalCollisionCheck (GetDownPosition (), GetDownPosition () + Vector3.down * (position.y - possibleNewPos.y)); 
			if (belowHit != null) {
				RaycastHit belowRaycastHit = (RaycastHit)belowHit;
				if (Constants.IsSolid (belowRaycastHit.collider.tag)|| belowRaycastHit.collider.tag.Equals("Enemy") ) {
					possibleNewPos.y = belowRaycastHit.point.y + GetHeight () / 2.0f;
					inAir = false;
					velocity.y = 0;
				}
				else if(belowRaycastHit.collider.tag.Equals("Player")){
					Mario m = belowRaycastHit.collider.GetComponent<Mario> ();
					m.Hurt (this);
				}
			}	
		}

		if (velocity.x < 0) {//LEFT CHECK
			RaycastHit? leftHit = HorizontalCollisionCheck (GetPosition (position), GetLeftPosition ()+Vector3.left*(position.x-possibleNewPos.x), 0f, 0.9f);
			if (leftHit!=null) {
				RaycastHit leftRaycastHit = (RaycastHit)leftHit;
				if (Constants.IsSolid (leftRaycastHit.collider.tag) || (leftRaycastHit.collider.tag.Equals("Enemy") && leftRaycastHit.collider.GetComponent<Plant>()==null) ) {
					possibleNewPos.x = leftRaycastHit.point.x + GetWidth () / 2.0f;
					velocity.x = changeMoveSpeed;
					//myAnimator.SetAnimation ("Walk", true);
					startAnimation = 0;
				} else if(leftRaycastHit.collider.tag.Equals("Player")){
					Mario m = leftRaycastHit.collider.GetComponent<Mario> ();
					m.Hurt (this);
				}
			}
		}
		else if (velocity.x > 0) {//RIGHT CHECK
			RaycastHit? rightHit = HorizontalCollisionCheck (GetPosition (position), GetRightPosition()+Vector3.right*(possibleNewPos.x-position.x), 0f, 0.9f);
			if (rightHit!=null) {

				RaycastHit rightRaycastHit = (RaycastHit)rightHit;
				if (Constants.IsSolid (rightRaycastHit.collider.tag) || (rightRaycastHit.collider.tag.Equals("Enemy") && rightRaycastHit.collider.GetComponent<Plant>()==null) ) {
					possibleNewPos.x = rightRaycastHit.point.x - GetWidth () / 2.0f;
					velocity.x = -1 * changeMoveSpeed;
					//myAnimator.SetAnimation ("Walk", false);
					startAnimation = 1;
				}
				else if(rightRaycastHit.collider.tag.Equals("Player")){
					Mario m = rightRaycastHit.collider.GetComponent<Mario> ();
					m.Hurt (this);
				}
			}
		}

		SetPosition (possibleNewPos);
		if (startAnimation == 0) {
			myAnimator.SetAnimation ("Walk", true);
		} else if (startAnimation == 1) {
			myAnimator.SetAnimation ("Walk", false);
		}

		prevMapPosition = mapPosition;
	}

	void Update(){
		if (!fallDeath && !squishDeath) {
			GoombaMoveLogic ();
		} else if (fallDeath) {
			transform.position += Vector3.down * Time.deltaTime*changeMoveSpeed;
			if (transform.position.y < -2) {
				DestroySprite ();
			}
		} else if (squishDeath) {
			deathTimer -= Time.deltaTime;
			if (deathTimer <= 0) {
				DestroySprite ();
			}
		}

		if (transform.position.y < -2) {
			Destroy (gameObject);
		}
	}

}
