using UnityEngine;
using System.Collections;

public class Interpolate : MonoBehaviour {
	public float distFromStart = 3;
	private Vector3 pos1, pos2;
	private bool towardsOne;
	private float timer;
	private const float timerMax = 2.0f;

	// Use this for initialization
	void Start () {
		pos1 = transform.position + Vector3.up * distFromStart;
		pos2 = transform.position + Vector3.down * distFromStart;
	}
	
	// Update is called once per frame
	void Update () {
		if (timer < timerMax) {
			timer += Time.deltaTime;
		} else {
			timer = 0;
			towardsOne = !towardsOne;
		}

		if (towardsOne) {
			transform.position = Vector3.Lerp (pos2, pos1, timer / timerMax);
		} else {
			transform.position = Vector3.Lerp (pos1, pos2, timer / timerMax);
		}
	}
}
