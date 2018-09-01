using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuzzyBeetle : Enemy {
	public GameObject shell;
	private bool inAir = true;
	private float changeGravitySpeed = 100f;
	private float changeMoveSpeed = 3f;

	private Vector2 mapPosition, prevMapPosition;

	void Start(){
		velocity = new Vector2 (-1 * changeMoveSpeed, 0);
		myAnimator.SetAnimation ("Walk", false);
		mapPosition = new Vector2 (Mathf.Floor (position.x), Mathf.Floor (position.y));
		prevMapPosition = mapPosition;
	}

	public override bool Squish (GameObject player){
		GameObject cloneShell = Instantiate<GameObject> (shell);
		cloneShell.transform.position = new Vector3 (position.x, position.y, 0);
		LogHandler.Instance.WriteLine(player.name +" BeetleSquish:  time = "+Time.time);
		DestroySprite ();

		return false;
	}

	public override void Burn (string playerName){
		//Nothing happens
	}

	void Update(){
		int startAnimation = -1;
		mapPosition = new Vector2 (Mathf.Floor (position.x), Mathf.Floor (position.y));
		if (mapPosition != prevMapPosition) {
			if (Mathf.Abs (velocity.x) > 0 && velocity.y == 0) {
				RaycastHit? below = VerticalCollisionCheck (GetPosition (position), GetDownPosition ());
				if (below == null) {
					inAir = true;
					Debug.Log ("In air 1");
				} else {
					RaycastHit belowHit = (RaycastHit)below;
					if (!Constants.IsSolid (belowHit.collider.tag) && !belowHit.collider.tag.Equals("Enemy")) {
						inAir = true;
						Debug.Log ("In air 2");
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
				if (Constants.IsSolid (belowRaycastHit.collider.tag) || belowRaycastHit.collider.tag.Equals("Enemy")) {
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
			RaycastHit? leftHit = HorizontalCollisionCheck (GetPosition (position)+Vector3.up*0.2f, GetLeftPosition ()+Vector3.left*(position.x-possibleNewPos.x)+Vector3.up*0.2f, 0.5f, 0.9f);
			if (leftHit != null) {
				RaycastHit leftRaycastHit = (RaycastHit)leftHit;
				if (Constants.IsSolid (leftRaycastHit.collider.tag) || (leftRaycastHit.collider.tag.Equals("Enemy") && leftRaycastHit.collider.GetComponent<Plant>()==null) ) {
					possibleNewPos.x = leftRaycastHit.point.x + GetWidth () / 2.0f;
					velocity.x = changeMoveSpeed;
					startAnimation = 0;
				} else if (leftRaycastHit.collider.tag.Equals ("Player")) {
					Mario m = leftRaycastHit.collider.GetComponent<Mario> ();
					m.Hurt (this);
				}
			} else {
				//Check and see if the position on the left is empty
				Vector2 mapPos = GetMapPosition(possibleNewPos+Vector2.up*0.2f);
				mapPos.y-=1;
				GameObject obj = Map.Instance.GetBlock((int)mapPos.x, (int)mapPos.y);

				if(obj==null || !Constants.IsSolid(obj.tag)){
					possibleNewPos.x = position.x;
					velocity.x = changeMoveSpeed;
					startAnimation = 0;
				}
			}
		}
		else if (velocity.x > 0) {//RIGHT CHECK
			RaycastHit? rightHit = HorizontalCollisionCheck (GetPosition (position)+Vector3.up*0.2f, GetRightPosition()+Vector3.right*(possibleNewPos.x-position.x)+Vector3.up*0.2f, 0.5f, 0.9f);
			if (rightHit != null) {
				RaycastHit rightRaycastHit = (RaycastHit)rightHit;
				if (Constants.IsSolid (rightRaycastHit.collider.tag) || (rightRaycastHit.collider.tag.Equals("Enemy") && rightRaycastHit.collider.GetComponent<Plant>()==null)) {
					Debug.Log (rightRaycastHit.collider.gameObject.name + " Right");
					possibleNewPos.x = rightRaycastHit.point.x - GetWidth () / 2.0f;
					velocity.x = -1 * changeMoveSpeed;
					startAnimation = 1;
				} else if (rightRaycastHit.collider.tag.Equals ("Player")) {
					Mario m = rightRaycastHit.collider.GetComponent<Mario> ();
					m.Hurt (this);
				}
			} else {
				//Check and see if the position on the right is empty
				Vector2 mapPos = GetMapPosition(possibleNewPos+Vector2.up*0.2f);
				mapPos.y-=1;
				mapPos.x += 1;
				GameObject obj = Map.Instance.GetBlock((int)mapPos.x, (int)mapPos.y);

				if(obj==null || !Constants.IsSolid(obj.tag)){
					possibleNewPos.x = position.x;
					velocity.x = -1 * changeMoveSpeed;
					startAnimation = 1;
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

		if (transform.position.y < -2) {
			Destroy (gameObject);
		}

	}

	private Vector2 GetMapPosition(Vector2 pos){
		return new Vector2(Mathf.Floor(pos.x), Mathf.Floor(pos.y));
	}
}