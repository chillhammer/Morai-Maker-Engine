using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lakitu : Enemy {
	public GameObject spikeyEgg;
	private const float MAX_WAIT_TIME = 2.0f;
	private float timer = 0.0f;
	private float chanceToThrow = 0.2f;
	private GameObject player;
	private float maxPlayerDist = 0f;
	private float speed = 1f;
	private float yOrig;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player");
		yOrig = transform.position.y;
	}

	public override bool Squish (GameObject player)
	{
		LogHandler.Instance.WriteLine(player.name +" LakituSquished:  time = "+Time.time);
		DestroySprite ();
		return false;
	}
	
	// Update is called once per frame
	void Update () {
		float distToPlayer = player.transform.position.x - transform.position.x;
		if (Mathf.Abs(distToPlayer) > maxPlayerDist && timer<MAX_WAIT_TIME) {
			
			Vector3 newPos = transform.position+Vector3.right * (distToPlayer+Random.Range(-1f,1f)) * Time.deltaTime*speed;
			if (timer > 0.1f) {
				if ((newPos [0] - transform.position.x) > 0) {
					myAnimator.SetAnimation ("Idle", false);
				} else {
					myAnimator.SetAnimation ("Idle", true);
				}
			}
			newPos.y = yOrig;
			transform.position = newPos;
		}


		if (timer < MAX_WAIT_TIME) {
			timer += Time.deltaTime;
		} else {
			if (Random.Range (0, 1.0f) <= chanceToThrow) {
				timer = 0;
				GameObject spikeyClone = Instantiate<GameObject> (spikeyEgg);
				spikeyClone.transform.position = transform.position;
				myAnimator.SetAnimation ("Throw");

				//Destroy (gameObject);
			}
		}
	}
}
