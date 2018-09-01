using UnityEngine;
using System.Collections;

public class IDEntry : MonoBehaviour {
	public AudioClip hover, dehover;
	private bool mouseOn, mouseOver;

	private string myID;
	private GUIText text;

	void Start(){
		text = gameObject.GetComponent<GUIText> ();
	}


	void OnMouseEnter(){
		if(hover!=null){
			AudioSource.PlayClipAtPoint (hover, Vector3.zero,0.5f);
		}
		//GetComponent<GUIText>().fontSize += 5;
		mouseOver = true;
	}

	void OnMouseExit(){
		if(dehover!=null){
			AudioSource.PlayClipAtPoint (dehover, Vector3.zero,0.5f);
		}
		//GetComponent<GUIText>().fontSize -= 5;
		mouseOver = false;

	}

	void OnMouseDown(){
		mouseOn = true;
		text.text = "";
	}


	void Update(){
		if (mouseOn) {
			text.text = "";
			string textToUse = myID;

			if (Input.GetKeyDown (KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0)) {
				textToUse += "0";
			}
			else if (Input.GetKeyDown (KeyCode.Keypad1)|| Input.GetKeyDown(KeyCode.Alpha1)) {
				textToUse += "1";
			}
			else if (Input.GetKeyDown (KeyCode.Keypad2)|| Input.GetKeyDown(KeyCode.Alpha2)) {
				textToUse += "2";
			}
			else if (Input.GetKeyDown (KeyCode.Keypad3)|| Input.GetKeyDown(KeyCode.Alpha3)) {
				textToUse += "3";
			}
			else if (Input.GetKeyDown (KeyCode.Keypad4)|| Input.GetKeyDown(KeyCode.Alpha4)) {
				textToUse += "4";
			}
			else if (Input.GetKeyDown (KeyCode.Keypad5)|| Input.GetKeyDown(KeyCode.Alpha5)) {
				textToUse += "5";
			}
			else if (Input.GetKeyDown (KeyCode.Keypad6)|| Input.GetKeyDown(KeyCode.Alpha6)) {
				textToUse += "6";
			}
			else if (Input.GetKeyDown (KeyCode.Keypad7)|| Input.GetKeyDown(KeyCode.Alpha7)) {
				textToUse += "7";
			}
			else if (Input.GetKeyDown (KeyCode.Keypad8)|| Input.GetKeyDown(KeyCode.Alpha8)) {
				textToUse += "8";
			}
			else if (Input.GetKeyDown (KeyCode.Keypad9)|| Input.GetKeyDown(KeyCode.Alpha9)) {
				textToUse += "9";
			}
			else if (Input.GetKeyDown (KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete)) {
				if (textToUse.Length > 0) {
					textToUse = textToUse.Substring (0, textToUse.Length - 1);
				}
			}
			myID = textToUse;
			if ((Time.fixedTime ) % 2 == 0) {
				textToUse += "|";
			}
			text.text = textToUse;
		}

		if (Input.GetMouseButtonDown (0) && !mouseOver) {
			mouseOn = false;
		}
	}
}
