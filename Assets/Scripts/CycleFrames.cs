using UnityEngine;
using System.Collections;

public class CycleFrames : MonoBehaviour {
	public Texture[] textures;
	private int currIndex = 0;
	private float timer = 0;
	public float MAX_TIME = 0.1f;

	private Renderer myRenderer;

	void Start(){
		myRenderer = gameObject.GetComponent<Renderer> ();
	
	}

	// Update is called once per frame
	void Update () {
		if (timer < MAX_TIME) {
			timer +=Time.deltaTime;		
		}
		else{
			timer = 0;
			currIndex+=1;

			if (currIndex>=textures.Length){
				currIndex = 0;
			}
			myRenderer.material.SetTexture("_MainTex", textures[currIndex]);
		}
	
	}
}
