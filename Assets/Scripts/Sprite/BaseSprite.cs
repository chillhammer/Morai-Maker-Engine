using UnityEngine;
using System.Collections;

public class BaseSprite : MonoBehaviour {
	//Physics Info
	public string spriteName;
	protected Vector2 velocity;
	protected Vector2 position{get { return new Vector2 (transform.position.x, transform.position.y);}}

	protected void SetPosition(Vector2 newPos){
		transform.position = new Vector3 (newPos.x, newPos.y, 0);
	}

	protected Vector3 GetPosition(Vector2 pos){
		return new Vector3 (pos.x, pos.y, 0);
	}

	protected float GetWidth(){
		return Mathf.Abs (transform.localScale.x);
	}

	protected float GetHeight(){
		return Mathf.Abs (transform.localScale.y);
	}

	protected Vector3 GetLeftPosition(){
		return new Vector3 (position.x-GetWidth()/2, position.y, 0);
	}
	protected Vector3 GetRightPosition(){
		return new Vector3 (position.x+GetWidth()/2, position.y, 0);
	}
	protected Vector3 GetUpPosition(){
		return new Vector3 (position.x, position.y+GetHeight()/2, 0);
	}
	protected Vector3 GetDownPosition(){
		return new Vector3 (position.x, position.y-GetHeight()/2, 0);
	}

	public virtual void DestroySprite(){
		Destroy (gameObject);
	}

	protected Vector3 GetPointOnBoundingBox(Vector3 difference){
		RaycastHit hit;
		if (Physics.Raycast (GetPosition (position), difference.normalized, out hit, difference.magnitude * 100)) {
			return hit.point;
		}
		return GetPosition (position);
	}

	protected RaycastHit? HorizontalCollisionCheck(Vector2 currPos, Vector2 futurePos){
		return HorizontalCollisionCheck (currPos, futurePos, 0f, 1f);
	}

	protected RaycastHit? HorizontalCollisionCheck(Vector2 currPos, Vector2 futurePos, float start, float end){
		float distanceToCollider = float.PositiveInfinity;

		Vector2 minPos = currPos + (Vector2.up*(GetHeight () * 0.5f));
		float currentMultiple =start;

		Vector2 direction = (futurePos - currPos).normalized;
		float distanceToCheck = (futurePos - currPos).magnitude;
		RaycastHit hit;
		RaycastHit? closestHit = null;
		while (currentMultiple < end) {
			if (Physics.Raycast (GetPosition(minPos+Vector2.down*GetHeight()*currentMultiple), direction, out hit,distanceToCheck)) {
				if (hit.distance < distanceToCollider) {
					distanceToCollider = hit.distance;
					closestHit = hit;
				}
			}

			currentMultiple += 0.1f;
		}

		return closestHit;
	}

	protected RaycastHit? VerticalCollisionCheck(Vector2 currPos, Vector2 futurePos){
		return VerticalCollisionCheck (currPos, futurePos, 0f, 1f);
	}

	protected RaycastHit? VerticalCollisionCheck(Vector2 currPos, Vector2 futurePos, float start, float end){
		float distanceToCollider = float.PositiveInfinity;

		Vector2 minPos = currPos + (Vector2.left*(GetWidth () * 0.5f));
		float currentMultiple = start;

		Vector2 direction = (futurePos - currPos).normalized;
		float distanceToCheck = (futurePos - currPos).magnitude;
		RaycastHit hit;
		RaycastHit? closestHit = null;
		while (currentMultiple <=end) {
			if (Physics.Raycast (GetPosition(minPos+Vector2.right*GetWidth()*currentMultiple), direction, out hit,distanceToCheck)) {
				if (hit.distance < distanceToCollider) {
					distanceToCollider = hit.distance;
					closestHit = hit;
				}
			}

			currentMultiple += 0.1f;
		}

		return closestHit;
	}
}
