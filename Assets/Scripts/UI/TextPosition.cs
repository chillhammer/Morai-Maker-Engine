using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPosition : MonoBehaviour {
	public RectTransform rect;
	// Use this for initialization
	void Start () {
		rect.position = new Vector3 (Screen.width*0.15f, Screen.height*0.95f, 0);
		Destroy (this);
	}

}
