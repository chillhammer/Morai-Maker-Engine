using UnityEngine;
using System.Collections;

public class Plant : Enemy {
	private float maxHeight = 0;
	private const float MAX_WAIT_TIME = 2.0f;
	private float timer = 0.0f;
	private int currState = 1;

	private const int UP = 0;
	private const int MOVING_DOWN = 1;
	private const int DOWN = 2;
	private const int MOVING_UP = 3;

	private const float maxSpeed = 1f;

	void Start(){
		
		//transform.position -= Vector3.up;
		transform.position+=Vector3.right*0.5f+Vector3.forward;
		transform.position += Vector3.down*0.5f;
		maxHeight = transform.position.y;

	}

	public override bool Squish (GameObject player){
		player.GetComponent<Mario> ().Hurt (this);
		return false;
	}


	
	// Update is called once per frame
	void Update () {
	
		if (timer < MAX_WAIT_TIME) {
			timer += Time.deltaTime;
			switch (currState) {
			case MOVING_UP:
				RaycastHit? above = VerticalCollisionCheck (GetPosition (position), GetUpPosition());
				if (above != null) {
					RaycastHit aboveHit = (RaycastHit)above;
					if (aboveHit.collider.tag.Equals("Player")) {
						aboveHit.collider.GetComponent<Mario> ().Hurt (this);
					} 
				}
				transform.position += Vector3.up * Time.deltaTime * maxSpeed;
				break;
			case MOVING_DOWN:
				transform.position -= Vector3.up * Time.deltaTime * maxSpeed;
				break;
			default:
				break;
			}
		} else {
			if (currState == MOVING_UP) {
				transform.position = new Vector3 (transform.position.x, maxHeight, 1.0f);
			}
			timer = 0;

			currState += 1;
			if (currState > 3) {
				currState = 0;
			}
		}
	}
}
