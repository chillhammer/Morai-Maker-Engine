using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteAnimator : MonoBehaviour {
	//Storage
	public GameObject AnimationObject;
	private Dictionary<string, SpriteAnimation> animationsDict;

	//For handling the currAnimation
	private SpriteAnimation currAnimation;
	private float CURR_MAX_TIME = 0;
	private float frameTimer = 0;
	private int currIndex = 0;
	private Renderer render;
	private bool facingRight;
	private const float pixelToUnityConversion = 1.0f/16.0f;

	public string CurrAnimationName {get { return currAnimation.AnimationName;}}
	public bool RightFacing {get { return facingRight;}}

	public bool paused = false;

	void Awake(){
		SpriteAnimation[] Animations = AnimationObject.GetComponents<SpriteAnimation> ();
		//Set up dictionary
		animationsDict = new Dictionary<string, SpriteAnimation>();

		//Store each animation by its name
		foreach (SpriteAnimation sa in Animations) {
			animationsDict.Add (sa.AnimationName, sa);
		}

		render = transform.parent.gameObject.GetComponent<Renderer>();
		SetAnimation(Animations[0].AnimationName, transform.parent.localScale.x>0);//Start with the first one
	}

	void Update(){
		if (currAnimation.frames.Length > 1 && !paused) {
			frameTimer += Time.deltaTime;
			if (frameTimer > CURR_MAX_TIME) {
				frameTimer = 0;

				currIndex += 1;
				if (currIndex >= currAnimation.frames.Length) {
					currIndex = 0;
				}

				SetFrame (currAnimation.frames [currIndex]);
			}
		}
	}

	void SetFrame(Texture2D tex){
		render.material.SetTexture ("_MainTex", tex);

		float oldHeight = transform.parent.localScale.y;
		float newWidth = ((float)tex.width) * pixelToUnityConversion;
		float newHeight = ((float)tex.height) * pixelToUnityConversion;
		//Turn it the correct way
		if (!facingRight) {
			newWidth *= -1;
		}
		transform.parent.position -= new Vector3 (0,oldHeight/2.0f-newHeight/2.0f , 0);
		transform.parent.localScale = new Vector3 (newWidth, newHeight, 1.0f);
	}

	public void SetAnimation(string animationName){
		SetAnimation (animationName, facingRight);
	}

	public void SetAnimation(string animationName, bool faceRight, bool overrideCheck = false){
		if (animationsDict.ContainsKey (animationName) && (overrideCheck || currAnimation==null || !currAnimation.AnimationName.Equals(animationName) || faceRight!=facingRight)) {
			currAnimation = animationsDict [animationName];
			CURR_MAX_TIME = 1.0f/currAnimation.framesPerSecond;
			currIndex = 0;
			frameTimer = 0;
			facingRight = faceRight;
			SetFrame (currAnimation.frames [currIndex]);
		}
	}

	public void SetFrameRate(float fps){
		CURR_MAX_TIME = 1.0f / fps;
	}
}
