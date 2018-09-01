using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar : MonoBehaviour {
	public float distFromStart = 3;
	private Vector3 pos1, pos2;
	private bool towardsOne;
	private float timer;
	private const float timerMax = 2.0f;
	public GameObject player;
	Vector3 prevPos;

	// Use this for initialization
	void Start () {
		pos1 = transform.position + Vector3.up * distFromStart;
		pos2 = transform.position + Vector3.down * distFromStart;
		player = GameObject.Find ("Player");
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

			if (player.transform.position.x+player.transform.localScale.x/2.0f > transform.position.x - 1.5f && player.transform.position.x-player.transform.localScale.x/2.0f<transform.position.x+1.5f) {
				if (transform.position.y+transform.localScale.y/2.0f + player.transform.localScale.x / 2.0f >= player.transform.position.y &&
					transform.position.y+transform.localScale.y/2.0f + player.transform.localScale.x / 2.0f < player.transform.position.y+0.5f) {
					player.GetComponent<Mario> ().SetHeld();
					Vector3 playerPos = player.transform.position;
					playerPos.y = transform.position.y + transform.localScale.y / 2.0f + player.transform.localScale.x / 2.0f;
					player.transform.position = playerPos;
				} else {
					player.GetComponent<Mario> ().SetNotHeld();
				}
			} else {
				player.GetComponent<Mario> ().SetNotHeld();
			}

		} else {
			transform.position = Vector3.Lerp (pos1, pos2, timer / timerMax);

			if (player.transform.position.x + player.transform.localScale.x / 2.0f > transform.position.x - 1.5f && player.transform.position.x - player.transform.localScale.x / 2.0f < transform.position.x + 1.5f) {
				if (transform.position.y + transform.localScale.y / 2.0f + player.transform.localScale.x / 2.0f+0.25f >= player.transform.position.y&&
					transform.position.y+transform.localScale.y/2.0f + player.transform.localScale.x / 2.0f < player.transform.position.y+0.5f) {
					player.GetComponent<Mario> ().SetHeld();
					Vector3 playerPos = player.transform.position;
					playerPos.y = transform.position.y + transform.localScale.y / 2.0f + player.transform.localScale.x / 2.0f;
					player.transform.position = playerPos;
				} else {
					player.GetComponent<Mario> ().SetNotHeld();
				}
			} else {
				player.GetComponent<Mario> ().SetNotHeld();
			}
		}

		prevPos = transform.position;
	}
}
