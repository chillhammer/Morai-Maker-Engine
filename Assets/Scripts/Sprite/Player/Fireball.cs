using UnityEngine;
using System.Collections;

public class Fireball : BaseSprite {
	public SpriteAnimator myAnimator;
	private float speed = 3;
	private float movementTimer = 0;
	private float movementTimerMax = 0.5f;
	private bool dying;
	private float deathTimer = 0.4f;

	public GameObject _player;
	private const float MAX_PLAYER_DIST = 12;

	public void StartFire(GameObject player, bool movingRight){
		_player = player;
		//Set start position
		Vector3 startPos = player.transform.position;
		startPos.x += movingRight ? 0.5f : -0.5f;
		startPos.y += 0.1f;

		transform.position = startPos;

		//Start animation
		myAnimator.SetAnimation("Spin", movingRight);

		//Velocity
		velocity = movingRight? new Vector2(speed*2.5f,-speed): new Vector2(-speed*2.5f,-speed);
	}

	void Explode(){
		velocity = Vector2.zero;
		myAnimator.SetAnimation("Explosion");
		dying = true;
	}

	void Update(){
		if (!dying) {
			AliveLogic ();
		} else {
			if (deathTimer > 0) {
				deathTimer -= Time.deltaTime;
			} else {
				DestroySprite ();
			}

			if (transform.position.y < -5) {
				DestroySprite ();
			}
		}
	}

	private void AliveLogic(){
		//Distance
		float currDist = Mathf.Abs(_player.transform.position.x-transform.position.x);
		if (currDist > MAX_PLAYER_DIST) {
			DestroySprite ();
		}

		//Movement flip
		if (velocity.y>0){
			if (movementTimer < movementTimerMax) {
				movementTimer += Time.deltaTime;
			} else {
				velocity.y *= -1f;
				movementTimer = 0;
			}
		}

		Vector2 possibleNewPos = position + velocity * Time.deltaTime;

		if (velocity.x < 0) {//LEFT CHECK
			RaycastHit? leftHit = HorizontalCollisionCheck (GetPosition (position), GetLeftPosition ()+Vector3.left*(position.x-possibleNewPos.x));
			if (leftHit!=null) {
				RaycastHit leftRaycastHit = (RaycastHit)leftHit;
				if (Constants.IsSolid (leftRaycastHit.collider.tag)) {
					//KILL
					Explode();
				} else if(leftRaycastHit.collider.tag.Equals("Enemy")){
					Enemy e = leftRaycastHit.collider.GetComponent<Enemy> ();
					e.Burn (_player.name);
					Explode();
				}
			}
		}
		else if (velocity.x > 0) {//RIGHT CHECK
			RaycastHit? rightHit = HorizontalCollisionCheck (GetPosition (position), GetRightPosition()+Vector3.right*(possibleNewPos.x-position.x));
			if (rightHit!=null) {

				RaycastHit rightRaycastHit = (RaycastHit)rightHit;
				if (Constants.IsSolid (rightRaycastHit.collider.tag)) {
					//KILL
					Explode();
				}
				else if(rightRaycastHit.collider.tag.Equals("Enemy")){
					Enemy e = rightRaycastHit.collider.GetComponent<Enemy> ();
					e.Burn (_player.name);
					Explode();
				}

			}
		}

		if (velocity.y > 0) {
			RaycastHit? aboveHit = VerticalCollisionCheck (GetUpPosition (), GetUpPosition () + Vector3.up * (possibleNewPos.y - position.y));
			if (aboveHit != null) {
				RaycastHit aboveRaycastHit = (RaycastHit)aboveHit;
				if (Constants.IsSolid (aboveRaycastHit.collider.tag)) {
					possibleNewPos.y = aboveRaycastHit.point.y - GetHeight () / 2.0f;
					velocity.y *= -1;
					movementTimer = 0;
				} else if (aboveRaycastHit.collider.tag.Equals ("Enemy")) {
					Enemy e = aboveRaycastHit.collider.gameObject.GetComponent<Enemy> ();
					if (e != null) {
						e.Burn (_player.name);
						Explode();
					}
				}
			}

		} else if (velocity.y < 0) {
			RaycastHit? belowHit = VerticalCollisionCheck (GetPosition(position), GetDownPosition () + Vector3.down * (position.y - possibleNewPos.y)); 
			if (belowHit != null) {
				RaycastHit belowRaycastHit = (RaycastHit)belowHit;
				if (Constants.IsSolid (belowRaycastHit.collider.tag)) {
					possibleNewPos.y = belowRaycastHit.point.y + GetHeight () / 2.0f;
					velocity.y *=-1;
					movementTimer = 0;
				}
				else if(belowRaycastHit.collider.tag.Equals("Enemy")){
					Enemy e = belowRaycastHit.collider.gameObject.GetComponent<Enemy> ();
					if (e != null) {
						e.Burn (_player.name);
						Explode();
					}
				}
			}	
		}

		SetPosition (possibleNewPos);
	}

}
