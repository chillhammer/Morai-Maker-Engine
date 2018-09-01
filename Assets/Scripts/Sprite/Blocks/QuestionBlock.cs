using UnityEngine;
using System.Collections;

public class QuestionBlock : BaseSprite {
	public SpriteAnimator myAnimator;

	private bool raisingPowerup;

	public GameObject mushroom, flower;
	private GameObject myPowerup;
	private float raiseSpeed = 3;

	public AudioSource mySource;
	public AudioClip myClip;

	public void HitBelow(Mario m){
		if(!raisingPowerup){
			GameObject toSpawn = mushroom;
			if (m != null) {
				toSpawn = m.IsLarge () ? flower : mushroom;
			} 
			mySource.PlayOneShot (myClip);
			myPowerup = Instantiate<GameObject> (toSpawn);
			myPowerup.transform.position = transform.position + Vector3.forward * 0.25f;
			raisingPowerup = true;
			myAnimator.SetAnimation ("Dead");
			if (m != null) {
				if (LogHandler.Instance != null) {
					LogHandler.Instance.WriteLine (m.name + " BlockPowerDestroy:  time = " + Time.time);
				}
			}
			//Play sound
		}
	}

	void Update(){
		if (raisingPowerup) {
			if (myPowerup!=null && myPowerup.transform.position.y < transform.position.y+1) {

				myPowerup.transform.position += Vector3.up * Time.deltaTime * raiseSpeed;

				if (myPowerup.transform.position.y > transform.position.y +1) {
					myPowerup.transform.position = transform.position + Vector3.up*1.0f;

					Mushroom m = myPowerup.GetComponent<Mushroom> ();
					if(m!=null){
						m.Activate();
					}

					//Destroy (this);
				}
			} else {
				Destroy (this);
			}
		}
	}
}
