using UnityEngine;
using System.Collections;

[System.Serializable]
public class SpriteAnimation : MonoBehaviour {
	public string AnimationName;
	public Texture2D[] frames;
	public float framesPerSecond;
}
