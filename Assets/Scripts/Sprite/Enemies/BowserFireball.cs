using UnityEngine;
using System.Collections;

public class BowserFireball : BaseSprite {
	public SpriteAnimator myAnimator;
	private float speed = 3;
	private float movementTimer = 0;
	private float movementTimerMax = 0.5f;
	private bool dying;
	private float deathTimer = 0.4f;

	public GameObject _bowser;
	private const float MAX_PLAYER_DIST = 12;
	private bool facingRight;

	public void StartFire(GameObject bowser, bool movingRight){
		_bowser = bowser;
		//Set start position
		Vector3 startPos = bowser.transform.position;
		startPos.x += movingRight ? 0.5f : -0.5f;
		startPos.y += 0.1f;

		transform.position = startPos;

		//Start animation
		myAnimator.SetAnimation("Spin", !movingRight);

		//Velocity
		velocity = movingRight? new Vector2(speed*2,0): new Vector2(-speed*2,0);

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
		}
	}

	private void AliveLogic(){
		//Distance
		float currDist = Mathf.Abs(_bowser.transform.position.x-transform.position.x);
		if (currDist > MAX_PLAYER_DIST) {
			DestroySprite ();
		}


		Vector2 possibleNewPos = position + velocity * Time.deltaTime;

		if (velocity.x < 0) {//LEFT CHECK
			RaycastHit? leftHit = HorizontalCollisionCheck (GetPosition (position), GetLeftPosition ()+Vector3.left*(position.x-possibleNewPos.x));
			if (leftHit!=null) {
				RaycastHit leftRaycastHit = (RaycastHit)leftHit;
				if(leftRaycastHit.collider.tag.Equals("Player")){
					Mario m = leftRaycastHit.collider.GetComponent<Mario> ();
					m.Hurt (this);
					Explode();
				}
			}
		}

		SetPosition (possibleNewPos);
	}

}
