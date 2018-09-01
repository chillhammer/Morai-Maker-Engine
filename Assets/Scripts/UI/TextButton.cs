using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TextButton : MonoBehaviour {
	public string levelName = "Level1";
	public AudioClip hover, dehover;
	private bool mouseOver = false;

	void OnMouseEnter(){
		if(hover!=null){
			AudioSource.PlayClipAtPoint (hover, Vector3.zero,0.5f);
		}
		GetComponent<GUIText>().fontSize += 5;
		mouseOver = true;
	}

	void OnMouseExit(){
		if(dehover!=null){
			AudioSource.PlayClipAtPoint (dehover, Vector3.zero,0.5f);
		}
		GetComponent<GUIText>().fontSize -= 5;
		mouseOver = false;
	}

	void OnMouseDown(){
		if (mouseOver) {
			ButtonPress ();
		}
	}

	public virtual void ButtonPress(){

		SceneManager.LoadScene (levelName);
	}
}
