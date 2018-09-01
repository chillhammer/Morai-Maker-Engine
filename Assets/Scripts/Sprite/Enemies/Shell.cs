using UnityEngine;
using System.Collections;

public class Shell : Enemy {
	private bool inAir = false;
	private float changeGravitySpeed = 100f;
	private float changeMoveSpeed = 9f;

	private Vector2 mapPosition, prevMapPosition;
	private bool moving, dying;

	void Start(){
		mapPosition = new Vector2 (Mathf.Floor (position.x), Mathf.Floor (position.y));
		prevMapPosition = mapPosition;
	}

	private void StartMovement(bool moveRight){
		int multiple = moveRight ? 1 : -1;
		velocity = new Vector2 (multiple * changeMoveSpeed, 0);
		myAnimator.SetAnimation ("Walk", moveRight);
		moving = true;
	}

	public bool Moving(){
		return Mathf.Abs (velocity.x) > 0;
	}

	public void StartMovement(Vector3 playerPosition){
		StartMovement (transform.position.x > playerPosition.x);
	}

	public override bool Squish (GameObject player){
		if (LogHandler.Instance != null) {
			if (moving) {
				LogHandler.Instance.WriteLine (player.name + " StopShell:  time = " + Time.time);
			} else {
				LogHandler.Instance.WriteLine (player.name + " UnleashShell:  time = " + Time.time);
			}
		}

		PlayerCollisionAbove (player.transform.position);
		return false;
	}

	public void PlayerCollisionAbove(Vector3 playerPosition){
		if (moving) {
			moving = false;
			velocity = new Vector2 (0, 0);
			myAnimator.SetAnimation ("Idle");
		} else {
			StartMovement (playerPosition);
		}
	}

	void ShellMovement(){
		int startAnimation = -1;
		mapPosition = new Vector2 (Mathf.Floor (position.x), Mathf.Floor (position.y));
		if (mapPosition != prevMapPosition) {
			if (Mathf.Abs (velocity.x) > 0 && velocity.y == 0) {
				RaycastHit? below = VerticalCollisionCheck (GetPosition (position), GetDownPosition ());
				if (below == null) {
					inAir = true;
				} else {
					RaycastHit belowHit = (RaycastHit)below;
					if (!Constants.IsSolid (belowHit.collider.tag)) {
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
				if (Constants.IsSolid (belowRaycastHit.collider.tag)) {
					possibleNewPos.y = belowRaycastHit.point.y + GetHeight () / 2.0f;
					inAir = false;
					velocity.y = 0;
				}
				else if(belowRaycastHit.collider.tag.Equals("Player")){
					Mario m = belowRaycastHit.collider.GetComponent<Mario> ();
					m.Hurt (this);
				}
				else if(belowRaycastHit.collider.tag.Equals("Enemy")){
					Enemy m = belowRaycastHit.collider.GetComponent<Enemy> ();
					if (LogHandler.Instance != null) {
						LogHandler.Instance.WriteLine ("ShellKill: " + m.spriteName + " time = " + Time.time);
					}
					m.DestroySprite ();
				}

			}	
		}

		if (velocity.x < 0) {//LEFT CHECK
			RaycastHit? leftHit = HorizontalCollisionCheck (GetPosition (position), GetLeftPosition ()+Vector3.left*((position.x-possibleNewPos.x)), 0f, 0.9f);
			if (leftHit!=null) {
				RaycastHit leftRaycastHit = (RaycastHit)leftHit;
				if (Constants.IsSolid (leftRaycastHit.collider.tag)) {
					possibleNewPos.x = leftRaycastHit.point.x + GetWidth () / 2.0f;
					velocity.x = changeMoveSpeed;
					startAnimation = 0;
				} else if(leftRaycastHit.collider.tag.Equals("Player")){
					Mario m = leftRaycastHit.collider.GetComponent<Mario> ();
					m.Hurt (this);
				}
				else if(leftRaycastHit.collider.tag.Equals("Enemy")){
					Enemy m = leftRaycastHit.collider.GetComponent<Enemy> ();
					m.DestroySprite ();
				}
			}
		}
		else if (velocity.x > 0) {//RIGHT CHECK
			RaycastHit? rightHit = HorizontalCollisionCheck (GetPosition (position), GetRightPosition()+Vector3.right*((possibleNewPos.x-position.x)), 0f, 0.9f);
			if (rightHit!=null) {

				RaycastHit rightRaycastHit = (RaycastHit)rightHit;
				if (Constants.IsSolid (rightRaycastHit.collider.tag)) {
					possibleNewPos.x = rightRaycastHit.point.x - GetWidth () / 2.0f;
					velocity.x = -1 * changeMoveSpeed;
					startAnimation = 1;
				}
				else if(rightRaycastHit.collider.tag.Equals("Player")){
					Mario m = rightRaycastHit.collider.GetComponent<Mario> ();
					m.Hurt (this);
				}
				else if(rightRaycastHit.collider.tag.Equals("Enemy")){
					Enemy m = rightRaycastHit.collider.GetComponent<Enemy> ();
					m.DestroySprite ();
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
		if (moving && !dying) {
			ShellMovement ();
		} 
	}

}
