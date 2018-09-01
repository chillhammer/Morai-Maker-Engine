using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBill : Enemy {
	private float speed = 3;
	private float movementTimer = 0;
	private float movementTimerMax = 0.5f;
	private bool dying;
	private float deathTimer = 0.4f;

	public GameObject _shooter;
	private const float MAX_PLAYER_DIST = 36;
	private bool facingRight;

	public override void Burn (string playerName)
	{
	}

	public override bool Squish(GameObject player)
	{
		LogHandler.Instance.WriteLine(player.name +" BulletBill:  time = "+Time.time);
		DestroySprite ();
		return false;
	}

	public void StartFire(GameObject shooter, bool movingRight){
		_shooter = shooter;
		//Set start position

		//Start animation
		myAnimator.SetAnimation("Idle", movingRight);

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
		float currDist = Mathf.Abs(_shooter.transform.position.x-transform.position.x);
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
		else if (velocity.x > 0) {//Right CHECK
			RaycastHit? rightHit = HorizontalCollisionCheck (GetPosition (position), GetLeftPosition ()+Vector3.right*(position.x-possibleNewPos.x));
			if (rightHit!=null) {
				RaycastHit rightRaycastHit = (RaycastHit)rightHit;
				if(rightRaycastHit.collider.tag.Equals("Player")){
					Mario m = rightRaycastHit.collider.GetComponent<Mario> ();
					m.Hurt (this);
					Explode();
				}
			}
		}

		SetPosition (possibleNewPos);
	}

}
