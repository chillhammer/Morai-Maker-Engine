using UnityEngine;
using System.Collections;

public class Bowser : Enemy {
	public int hitpoints = 6;
	private bool dying; 

	//Jump stuff
	private bool jumping = false;
	private bool inAir = true;
	private bool facingRight = false;
	private float changeGravitySpeed = 20f;
	private float changeMoveSpeed = 2f;
	private float initialChangeSpeed = 9f;
	private float changeJumpSpeed =36f;
	private float maxJumpSpeed = 3;//20 gets to a max height of ~5
	private const float jumpTimerMax = 3.0f;
	private float jumpTimer = 0.0f;
	private bool rightPressed, leftPressed;

	//Mario Reference
	private GameObject mario;

	//fireball stuff
	public GameObject fireBall;
	private const float fireballTimerMax = 2.5f;
	private float fireballTimer = 0.0f;

	//Movement stuff
	private const float movementTimerMax = 1.0f;
	private float movementTimer = 0.0f;
	private float startX = 0;
	private const float maxX = 4;

	void Start(){
		startX = transform.position.x;
		mario = GameObject.FindGameObjectWithTag ("Player");
	}

	public override void DestroySprite (){
		dying = true;
	}

	//You can't squish the bowser!
	public override bool Squish (GameObject player){
		Mario m = player.GetComponent<Mario> ();

		if (m != null) {
			m.Hurt (this);
		}

		return false;
	}

	public override void Burn (string playerName){
		if (hitpoints <= 0) {
			base.Burn (playerName);
			//DIE
		} else {
			hitpoints -= 1;
		}
	}

	private void SpawnFireball(){
		fireballTimer = 0;
		GameObject cloneFire = Instantiate<GameObject> (fireBall);
		cloneFire.GetComponent<BowserFireball> ().StartFire (gameObject, facingRight);
		myAnimator.SetAnimation ("Fire", !facingRight);
	}

	void Update(){
		if (!dying) {
			BowserMovementLogic ();
		} else {
			velocity.x = 0;
			velocity.y = -10;
			Vector2 newPos = position + velocity * Time.deltaTime;
			SetPosition (newPos);

			if (newPos.y < -10) {
				Destroy (gameObject);
			}
		}
	}
	private void BowserMovementLogic(){
		
		facingRight = mario.transform.position.x > transform.position.x;


		if (fireballTimer < fireballTimerMax) {
			fireballTimer += Time.deltaTime;
		} else {
			SpawnFireball ();  
		}

		if (!inAir && fireballTimer > 0.1) {
			myAnimator.SetAnimation ("Walk", !facingRight);
		}

		if (movementTimer < movementTimerMax) {
			movementTimer += Time.deltaTime;
		} else {
			movementTimer = 0;
			rightPressed = Random.Range(0,2)==0;
			leftPressed = !rightPressed;
		}


		if (transform.position.x - startX > maxX) {
			rightPressed = false;
			leftPressed = true;
		}

		if (startX - transform.position.x > maxX) {
			rightPressed = true;
			leftPressed = false;
		}
		bool rightOrLeftPressed = false;
		//Running (Takes 2 to speed up/slow down)
		if ((rightPressed) && !leftPressed) {
			rightOrLeftPressed = true;
			RaycastHit? rightHit = HorizontalCollisionCheck (GetPosition(position), GetRightPosition());
			if (rightHit == null) {	
				rightPressed = true;

				velocity.x = changeMoveSpeed;
			}
		} 
		else if (leftPressed && !rightPressed){
			rightOrLeftPressed = true;
			RaycastHit? leftHit = HorizontalCollisionCheck (GetPosition(position), GetLeftPosition());
			if (leftHit == null) {
				//Left Start was hit
				velocity.x = -1 * changeMoveSpeed;
			} 
		}

		//Determine if in air
		if (Mathf.Abs(velocity.x)>0 && velocity.y==0) {
			RaycastHit? below = VerticalCollisionCheck (GetPosition (position), GetDownPosition());
			if (below == null) {
				inAir = true;
			} else {
				RaycastHit belowHit = (RaycastHit)below;
				if (!Constants.IsSolid (belowHit.collider.tag) && !belowHit.collider.tag.Equals("Plant")) {
					inAir = true;
				} 
			}
		}

		bool jump = false;

		if (jumpTimer < jumpTimerMax) {
			jumpTimer += Time.deltaTime;
		} else {
			jump = true;
			jumpTimer = 0;
		}


		//Input -Jumping
		if (jump && velocity.y == 0 && !inAir) {
			velocity.y = initialChangeSpeed;
			jumping = true;
			inAir = true;

		} 
		else if (jump && jumping && velocity.y>0) {
			velocity.y += changeJumpSpeed*Time.deltaTime;
			if (velocity.y > maxJumpSpeed) {
				velocity.y = maxJumpSpeed;
				jumping = false;
			}
		}
		else if (velocity.y!=0){
			jumping = false;
		}

		//Gravity
		if (inAir) {
			myAnimator.SetAnimation ("Jump", !facingRight);
			velocity.y -= changeGravitySpeed * Time.deltaTime;
		}

		Vector2 possibleNewPos = position + Time.deltaTime * velocity;
		bool squishedEnemy = false;
		if (velocity.y > 0) {
			RaycastHit? aboveHit = VerticalCollisionCheck (GetUpPosition (), GetUpPosition () + Vector3.up * (possibleNewPos.y - position.y));
			if (aboveHit != null) {
				RaycastHit aboveRaycastHit = (RaycastHit)aboveHit;
				if (Constants.IsSolid (aboveRaycastHit.collider.tag)) {
					possibleNewPos.y = aboveRaycastHit.point.y - GetHeight () / 2.0f;
					velocity.y = 0;

					if (aboveRaycastHit.collider.tag.Equals ("Breakable")) {
						aboveRaycastHit.collider.GetComponent<Block> ().HitBelow (gameObject);
					}
					else if (aboveRaycastHit.collider.tag.Equals ("PowerupBlock")) {
						QuestionBlock b = aboveRaycastHit.collider.GetComponent<QuestionBlock> ();
						if(b!=null){
							b.HitBelow (null);
						}
					}
				}

			}

		} else if (velocity.y < 0) {
			RaycastHit? belowHit = VerticalCollisionCheck (GetPosition(position), GetDownPosition () + Vector3.down * (position.y - possibleNewPos.y), 0.05f, 0.95f); 
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
			}	
		}

		if (velocity.x < 0) {//LEFT CHECK
			RaycastHit? leftHit = HorizontalCollisionCheck (GetPosition (position), GetLeftPosition ()+Vector3.left*(position.x-possibleNewPos.x));
			if (leftHit!=null) {

				RaycastHit leftRaycastHit = (RaycastHit)leftHit;
				if(Constants.IsSolid (leftRaycastHit.collider.tag)){
					possibleNewPos.x = leftRaycastHit.point.x + GetWidth () / 2.0f;
					velocity.x = 0;
				}
				else if(leftRaycastHit.collider.tag.Equals("Player")){
					Mario m = leftRaycastHit.collider.GetComponent<Mario> ();
					m.Hurt (this);
				}
			}
		}
		else if (velocity.x > 0) {//RIGHT CHECK
			RaycastHit? rightHit = HorizontalCollisionCheck (GetPosition (position), GetRightPosition()+Vector3.right*(possibleNewPos.x-position.x));
			if (rightHit!=null) {
				RaycastHit rightRaycastHit = (RaycastHit)rightHit;
				if(Constants.IsSolid (rightRaycastHit.collider.tag)){
					possibleNewPos.x = rightRaycastHit.point.x - GetWidth () / 2.0f;
					velocity.x = 0;
				}
				else if(rightRaycastHit.collider.tag.Equals("Player")){
					Mario m = rightRaycastHit.collider.GetComponent<Mario> ();
					m.Hurt (this);
				}


			}

		}

		SetPosition (possibleNewPos);




	}

}
