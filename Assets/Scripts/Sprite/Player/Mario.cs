using UnityEngine;
using System.Collections;

public class Mario : BaseSprite {
	//Max run speed
	private float maxWalkSpeed = 5f;
	private float changeWalkSpeed = 30f;

	private float maxRunSpeed = 8f;
	private float changeRunSpeed = 33f;
	private float changeSpeed = 4f;

	private float changeJumpSpeed =72f;
	private float changeGravitySpeed = 100f;//100f
	private float initialChangeSpeed = 18f;
	private float maxJumpSpeed = 20;//20 gets to a max height of ~5
	private bool jumping = false;
	private bool inAir = true;
	private bool running = false;
	public bool playerTwo = false;

	//Powerups
	private bool large, fire, dying, invulnerable;
	private float invulnerabilityTimer = 0.0f, maxInvulnerableTime = 5, fireTimer = 0, maxFireTime = 0.1f;

	public CameraFollower follower;

	public SpriteAnimator smallAnimator, largeAnimator, fireAnimator;
	private SpriteAnimator currAnimator;

	public GameObject fireBall;

	private bool held = false;//HACKY
	public KeyCode left, right, jump, down, a;
	public string horizontalStringName = "Horizontal1";
	public string verticalStringName = "Vertical1";
	public string jumpStringName = "Jump1";
	public string speedStringName = "Speed1";

	//Logging info
	private float startPlay;
	private float leftStart;//Last time left was pressed
	private float rightStart;//Last time right was pressed
	private float duckStart;//Last time duck was pressed
	private float startLittleRecord = -17;//Last time little 
	private float startLargeRecord; //Last time large began
	private float startFireRecord;//Last time fire began
	private float startRunningRecord;//Last time run began
	private float jumpStart;//When jump began

	public AudioClip jumpSound, bump, breakBlock, powerUp, powerDown, death, coin, squish, shoot;
	public AudioSource myAudio;
	void Start(){
		SetAnimator (smallAnimator);
		currAnimator.SetAnimation ("Idle", true);
		follower = Camera.main.GetComponent<CameraFollower> ();
		startPlay = Time.time;

	}

	public void SetHeld(){
		if (!jumping){
			held = true;
			velocity.y = 0;
			inAir = false;
		}

	}

	public void SetNotHeld(){
		held = false;

	}

	// Update is called once per frame
	void Update () {
		//joystick 1 button 1 is A (jump)
		//joystick 1 button 2 is B (speed)
		//Debug.Log("Jump: "+Input.GetKey("joystick 2 button 1")+" Speed: "+Input.GetKey("joystick 2 button 2")+" Horizontal: "+Input.GetAxis("Horizontal2")+" Vertical: "+Input.GetAxis("Vertical2"));
 		if (!dying) {
			MarioMovementLogic ();
		} else {
			velocity.x = 0;
			velocity.y = -10;
			Vector2 newPos = position + velocity * Time.deltaTime;
			SetPosition (newPos);
		}
	}

	public bool IsLarge(){
		return large || fire;
	}

	public bool IsDead(){
		bool isDead = transform.position.y < -2;

		if (isDead && !dying) {
			PrintDeath (null);//Killed by gap
		}

		return isDead;
	}

	private void SpawnFireball(){
		GameObject[] fireballs = GameObject.FindGameObjectsWithTag ("Fireball");
		int myFireballCount = 0;
		for (int i = 0; i < fireballs.Length; i++) {
			if (fireballs [i].GetComponent<Fireball> ()._player == gameObject) {
				myFireballCount += 1;
			}
		}

		if (myFireballCount< 2) {
			myAudio.PlayOneShot (shoot);
			fireTimer = maxFireTime;
			GameObject cloneFire = Instantiate<GameObject> (fireBall);
			cloneFire.GetComponent<Fireball> ().StartFire (gameObject, transform.localScale.x > 0);
		}
	}


	private void MarioMovementLogic(){
		bool rightOrLeftPressed = false;
		bool rightPressed = false;
		bool leftPressed = false;
		if (Input.GetKeyDown (a)){ //|| Input.GetButton(speedStringName)) {
			running = true;

			if (fire) {
				SpawnFireball ();
			}

			if(startRunningRecord==0){
				startRunningRecord = Time.time;
			}

		} else if (running && !Input.GetKey (a)) {
			running = false;

			if (startRunningRecord != 0) {
				if (LogHandler.Instance != null) {
					LogHandler.Instance.WriteLine (gameObject.name + " RunState: StTime = " + startRunningRecord + " EdTime = " + Time.time);
				}
				startRunningRecord = 0;
			}
		}

		if (fireTimer > 0) {
			fireTimer -= Time.deltaTime;
		}

		//Running (Takes 2 to speed up/slow down)
		if ((Input.GetKey (right) || Input.GetKeyDown(right)) ){//||Input.GetAxis(horizontalStringName)>0.5f) {
			rightOrLeftPressed = true;
			RaycastHit? rightHit = HorizontalCollisionCheck (GetPosition(position), GetRightPosition());
			if (rightHit == null) {	
				rightPressed = true;
				if (rightStart == 0) {
					rightStart = Time.time;
				}
				if (running) {
					velocity.x += changeRunSpeed*Time.deltaTime;
					if (Mathf.Abs (velocity.x) > maxRunSpeed) {
						velocity.x = maxRunSpeed;
					}
				} 
				else {
					velocity.x += changeWalkSpeed*Time.deltaTime;
					if (Mathf.Abs (velocity.x) > maxWalkSpeed) {
						velocity.x = maxWalkSpeed;
					}
				}
			}
		} 
		else if ((Input.GetKey (left) || Input.GetKeyDown(left) )){// ||Input.GetAxis(horizontalStringName)<-0.5f){
			rightOrLeftPressed = true;
			RaycastHit? leftHit = HorizontalCollisionCheck (GetPosition(position), GetLeftPosition());
			if (leftHit == null) {
				//Left Start was hit
				if (leftStart == 0) {
					leftStart = Time.time;
				}
				leftPressed = true;
				if (running) {
					velocity.x -= changeRunSpeed*Time.deltaTime;
					if (Mathf.Abs (velocity.x) > maxRunSpeed) {
						velocity.x = -1*maxRunSpeed;
					}
				} 
				else {
					velocity.x -= changeWalkSpeed*Time.deltaTime;
					if (velocity.x < -1 * maxWalkSpeed) {
						velocity.x = -1 * maxWalkSpeed;
					}
				}
			} 
		} 
		else {
			if (Mathf.Abs(velocity.x) > changeWalkSpeed*Time.deltaTime ) {
				if (velocity.x > 0) {
					velocity.x -= changeWalkSpeed*Time.deltaTime;
				} else {
					velocity.x += changeWalkSpeed*Time.deltaTime;
				}
				if (Mathf.Abs (velocity.x) < changeWalkSpeed*Time.deltaTime) {
					velocity.x = 0;
				}
			} 
			else {
				velocity.x = 0;
			}
		}

		//Logging
		if (!leftPressed && leftStart != 0) {
			if (LogHandler.Instance != null) {
				LogHandler.Instance.WriteLine (gameObject.name + " LeftMove: StTime = " + leftStart + " EdTime = " + Time.time);
			}
			leftStart = 0;
		}
		if (!rightPressed && rightStart != 0) {
			if (LogHandler.Instance != null) {
				LogHandler.Instance.WriteLine (gameObject.name + " RightMove: StTime = " + rightStart + " EdTime = " + Time.time);
			}
			rightStart = 0;
		}

		//Determine if in air
		if (velocity.y==0 && !held) {
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

		//Input -Jumping
		if ( (Input.GetKeyDown (jump) ) && velocity.y == 0 && !inAir) {//|| Input.GetButtonDown(jumpStringName)
			velocity.y = initialChangeSpeed;
			jumping = true;
			inAir = true;
			held = false;
			myAudio.PlayOneShot (jumpSound);
			if (jumpStart == 0) {
				jumpStart = Time.time;
			}

		} 
		else if ( (Input.GetKey (jump)) && jumping && velocity.y>0) {//||Input.GetButton(jumpStringName)
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
						if (large) {
							myAudio.PlayOneShot (breakBlock);
						} else {
							myAudio.PlayOneShot (bump);
						}
							aboveRaycastHit.collider.GetComponent<Block> ().HitBelow (gameObject);
						
					} else if (aboveRaycastHit.collider.tag.Equals ("PowerupBlock")) {
						myAudio.PlayOneShot (bump);
						QuestionBlock b = aboveRaycastHit.collider.GetComponent<QuestionBlock> ();
						if (b != null) {
							b.HitBelow (this);
						}
					} else {
						myAudio.PlayOneShot (bump);
					}
				}
				else if (aboveRaycastHit.collider.tag.Equals ("Powerup")) {
					HandlePowerup (aboveRaycastHit.collider.GetComponent<BaseSprite> ());
				}
				else if (aboveRaycastHit.collider.tag.Equals ("Enemy")) {
					Hurt (aboveRaycastHit.collider.GetComponent<BaseSprite>());
				}
				else if (aboveRaycastHit.collider.tag.Equals ("Coin")) {
					myAudio.PlayOneShot (coin);
					aboveRaycastHit.collider.GetComponent<Coin> ().CollectCoin (this);
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


					if (jumpStart != 0) {
						if (LogHandler.Instance != null) {
							LogHandler.Instance.WriteLine (gameObject.name + " Jump: StTime = " + jumpStart + " EdTime = " + Time.time);
						}
						jumpStart = 0;
					}
				}
				else if (belowRaycastHit.collider.tag.Equals ("Powerup")) {
					HandlePowerup (belowRaycastHit.collider.GetComponent<BaseSprite> ());
				}
				else if(belowRaycastHit.collider.tag.Equals("Enemy")){
					Enemy e = belowRaycastHit.collider.gameObject.GetComponent<Enemy> ();
					if (e != null) {
						bool squished = e.Squish (gameObject);

						if (squished) {
							myAudio.PlayOneShot (squish);
							//if squished just continue falling as normal
							squishedEnemy = true;
						} else if(!IsDead()){
							myAudio.PlayOneShot (squish);
							//Wasn't squished so bounce off to get out of the way
							velocity.y = initialChangeSpeed;
							jumping = true;
							inAir = true;
						}

					}
				}
				else if (belowRaycastHit.collider.tag.Equals ("Coin")) {
					myAudio.PlayOneShot (coin);
					belowRaycastHit.collider.GetComponent<Coin> ().CollectCoin (this);
				}
			}	
		}

		if (velocity.x < 0) {//LEFT CHECK
			RaycastHit? leftHit = HorizontalCollisionCheck (GetPosition (position), GetLeftPosition ()+Vector3.left*(position.x-possibleNewPos.x));
			if (leftHit!=null) {

				RaycastHit leftRaycastHit = (RaycastHit)leftHit;
				if (leftRaycastHit.collider.tag.Equals ("Enemy") && !squishedEnemy) {
					Shell s= leftRaycastHit.collider.GetComponent<Shell>();
					if (s == null) {
						Hurt (leftRaycastHit.collider.GetComponent<BaseSprite>());
					} else if (s.Moving ()) {
						Hurt (leftRaycastHit.collider.GetComponent<BaseSprite>());
					}
				} else if (leftRaycastHit.collider.tag.Equals ("Powerup")) {
					HandlePowerup (leftRaycastHit.collider.GetComponent<BaseSprite> ());
				} 
				else if (leftRaycastHit.collider.tag.Equals ("Coin")) {
					myAudio.PlayOneShot (coin);
					leftRaycastHit.collider.GetComponent<Coin> ().CollectCoin (this);
				}else if(Constants.IsSolid (leftRaycastHit.collider.tag)){
					possibleNewPos.x = leftRaycastHit.point.x + GetWidth () / 2.0f;
					velocity.x = 0;
				}
			}
			if (!playerTwo) {
				follower.CheckMoveLeft (GetPosition (possibleNewPos), Mathf.Abs (possibleNewPos.x - position.x));
			}
		}
		else if (velocity.x > 0) {//RIGHT CHECK
			RaycastHit? rightHit = HorizontalCollisionCheck (GetPosition (position), GetRightPosition()+Vector3.right*(possibleNewPos.x-position.x));
			if (rightHit!=null) {
				RaycastHit rightRaycastHit = (RaycastHit)rightHit;
				if (rightRaycastHit.collider.tag.Equals ("Enemy") && !squishedEnemy) {
					Shell s= rightRaycastHit.collider.GetComponent<Shell>();
					if (s == null) {
						Hurt (rightRaycastHit.collider.GetComponent<BaseSprite>());
					} else if (s.Moving ()) {
						Hurt (rightRaycastHit.collider.GetComponent<BaseSprite>());
					}
				} else if (rightRaycastHit.collider.tag.Equals ("Powerup")) {
					HandlePowerup (rightRaycastHit.collider.GetComponent<BaseSprite> ());
				} 
				else if (rightRaycastHit.collider.tag.Equals ("Coin")) {
					myAudio.PlayOneShot (coin);
					rightRaycastHit.collider.GetComponent<Coin> ().CollectCoin (this);
				}
				else if(Constants.IsSolid (rightRaycastHit.collider.tag)){
					possibleNewPos.x = rightRaycastHit.point.x - GetWidth () / 2.0f;
					velocity.x = 0;
				}


			}

			if (!playerTwo) {
				follower.CheckMoveRight (GetPosition (possibleNewPos), Mathf.Abs (possibleNewPos.x - position.x));
			}
		}

		//Set new position
		if (!dying) {
			SetPosition (possibleNewPos);
			
			//Determine animation

			if (velocity.y == 0) {
				if ( (Input.GetKey (down)) && !rightOrLeftPressed) {//||Input.GetAxis(verticalStringName)>0.5f
					currAnimator.SetAnimation ("Duck");
					//Logging info
					if (duckStart == 0) {
						duckStart = Time.time;
					}
				} else {
					//Print out dodge log
					if (duckStart > 0) {
						if (LogHandler.Instance != null) {
							LogHandler.Instance.WriteLine (gameObject.name + " Duck: StTime = " + duckStart + " EdTime = " + Time.time);
						}
						duckStart = 0;
					}

					if (velocity.x > changeSpeed) {
						currAnimator.SetAnimation ("Run", true);
					} else if (velocity.x < -1 * changeSpeed) {
						currAnimator.SetAnimation ("Run", false);
					} else if (velocity.x > 0 && leftPressed) {
						currAnimator.SetAnimation ("Turn", false);
					} else if (velocity.x < 0 && rightPressed) {
						currAnimator.SetAnimation ("Turn", true);
					} else if (velocity.x == 0) {
						if (fireTimer <= 0) {
							currAnimator.SetAnimation ("Idle");
						} else {
							//currAnimator.SetAnimation ("Fire");
						}
					} else if (velocity.x > 0) {
						currAnimator.SetAnimation ("Run", true);
					} else if (velocity.x < -1 * 0) {
						currAnimator.SetAnimation ("Run", false);
					}
				}
			} else if (velocity.y > 0) {
				currAnimator.SetAnimation ("Jump");
			} else if (velocity.y < 0) {
				currAnimator.SetAnimation ("Fall");
			}
		

			//Invulnerability flash
			if (invulnerable) {
				invulnerabilityTimer -= Time.deltaTime;


				float flashTime = invulnerabilityTimer * 10f;
				int flashTimeInt = (int)Mathf.Round (flashTime);

				GetComponent<MeshRenderer> ().material.color = new Color32 (255, 255, 255, 255);
				if (flashTimeInt % 2 != 0 && invulnerabilityTimer > 0) {
					GetComponent<MeshRenderer> ().material.color = new Color32 (255, 255, 255, 0);
				}

				if (invulnerabilityTimer < 0) {
					invulnerable = false;
				}
			}
		}
	}

	private void SetAnimator(SpriteAnimator toSet){
		smallAnimator.paused = true;
		largeAnimator.paused = true;
		fireAnimator.paused = true;
		SpriteAnimator oldAnimator = currAnimator;
		currAnimator = toSet;
		currAnimator.paused = false;
		if (oldAnimator != null) {
			currAnimator.SetAnimation (oldAnimator.CurrAnimationName);
		}
	}


	public void SetLarge(){
		
		if (!fire && !large) {
			myAudio.PlayOneShot (powerUp);
			//Start little record
			if (startLittleRecord != 0) {
				if (LogHandler.Instance != null) {
					LogHandler.Instance.WriteLine (gameObject.name + " LittleState: StTime = " + startLittleRecord + " EdTime = " + Time.time);
				}
				startLittleRecord = 0;
			}
			if (startLargeRecord == 0) {
				startLargeRecord = 0;
			}

			SetAnimator (largeAnimator);
			large = true;
		}
	}

	public void SetFire(){
		if (!fire) {
			myAudio.PlayOneShot (powerUp);
			//Check for start little record
			if (!large) {
				if (startLittleRecord != 0) {
					if (LogHandler.Instance != null) {
						LogHandler.Instance.WriteLine (gameObject.name + " LittleState: StTime = " + startLittleRecord + " EdTime = " + Time.time);
					}
					startLittleRecord = 0;
				}
			}
			if (startLargeRecord != 0) {
				if (LogHandler.Instance != null) {
					LogHandler.Instance.WriteLine (gameObject.name + " LargeState: StTime = " + startLargeRecord + " EdTime = " + Time.time);
				}
				startLargeRecord = 0;
			}
			if (startFireRecord == 0) {
				startFireRecord = Time.time;
			}

			SetAnimator (fireAnimator);
			fire = true;
			large = false;
		}
	}

	private void SetInvulnerable(){
		invulnerable = true;
		invulnerabilityTimer = maxInvulnerableTime;
	}

	public void Hurt(BaseSprite b){
		if (!invulnerable) {
			if (large) {
				myAudio.PlayOneShot (powerDown);
				SetAnimator (smallAnimator);
				SetInvulnerable ();
				//Start little recording
				if (startLittleRecord == 0) {
					startLittleRecord = Time.time;
				}
				if (startLargeRecord != 0) {
					if (LogHandler.Instance != null) {
						LogHandler.Instance.WriteLine (gameObject.name + " LargeState: StTime = " + startLargeRecord + " EdTime = " + Time.time);
					}
					startLargeRecord = 0;
				}
				large = false;
			} else if (fire) {
				myAudio.PlayOneShot (powerDown);
				SetAnimator (largeAnimator);
				SetInvulnerable ();
				fire = false;
				large = true;

				if (startLargeRecord == 0) {
					startLargeRecord = 0;
				}

				if (startFireRecord != 0) {
					if (LogHandler.Instance != null) {
						LogHandler.Instance.WriteLine (gameObject.name + " FireState: StTime = " + startFireRecord + " EdTime = " + Time.time);
					}
					startLargeRecord = 0;
				}

			} else {
				
				Kill (b);
			}
		}
	}

	public void Kill(BaseSprite b){
		myAudio.PlayOneShot (death);
		PrintDeath (b);
		dying = true;
		currAnimator.SetAnimation("Death");
		velocity.x = 0;
		velocity.y = -10;	
	}

	public void PrintDeath(BaseSprite b){
		if (b != null) {
			if (LogHandler.Instance != null) {
				LogHandler.Instance.WriteLine (gameObject.name + " Die = " + Time.time + " by " + b.spriteName);
			}
		} else {
			if (LogHandler.Instance != null) {
				LogHandler.Instance.WriteLine (gameObject.name + " Die = " + Time.time + " by gap");
			}
		}

		WriteTimeToPlay ();
	}

	public void WriteTimeToPlay(){
		if (LogHandler.Instance != null) {
			LogHandler.Instance.WriteLine (gameObject.name + " Totaltime: StTime = " + startPlay + " EdTime = " + Time.time);
		}
	}

	private void HandlePowerup(BaseSprite baseSprite){
		if (baseSprite.spriteName.Equals ("Fireflower")) {
			SetFire ();
		} else if (baseSprite.spriteName.Equals ("Mushroom")) {
			SetLarge ();
		}
		baseSprite.DestroySprite ();
	}




}
