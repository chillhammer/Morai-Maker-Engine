using UnityEngine;
using System.Collections;

public class CameraFollower : MonoBehaviour {
	public float boundingEdgeRatio = 0.2f;
	public float minX = 0;
	private int maxX = 250;

	void Start(){
		minX = gameObject.transform.position.x;
	}

	public void CheckMoveLeft(Vector3 playerPos, float amnt){
		Vector3 screenPos = Camera.main.WorldToScreenPoint (playerPos);

		if (screenPos.x < (Screen.width * boundingEdgeRatio)) {
			transform.position+=Vector3.left*amnt;	

			if (transform.position.x < minX) {
				transform.position = new Vector3 (minX, transform.position.y, -10);
			}
		}
	}

	public void CheckMoveRight(Vector3 playerPos, float amnt){
		Vector3 screenPos = Camera.main.WorldToScreenPoint (playerPos);

		if (screenPos.x > (Screen.width-(Screen.width * boundingEdgeRatio))) {
			transform.position+=Vector3.right*amnt;	

			if (transform.position.x > maxX) {
				transform.position = new Vector3 (maxX, transform.position.y, -10);
			}
		}
	}


}
