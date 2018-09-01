using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IDTextInputCheck : MonoBehaviour {
	public Text t;

	public void Round1Click(){
		if (CheckOkay ()) {
			Constants.idOne = t.text;
			Constants.round = 1;

			if (Application.isEditor){
				Constants.directory=Application.dataPath;
			}
			else{
				if (Application.platform == RuntimePlatform.WindowsPlayer){
					Constants.directory=Application.dataPath;
				}
				else{
					Constants.directory=Application.dataPath + "/Resources/Data";
				}
			}

			SceneManager.LoadScene ("Main");
		}
	}

	public void Round2Click(){
		if (CheckOkay ()) {
			Constants.idOne = t.text;
			Constants.round = 2;

			if (Application.isEditor){
				Constants.directory=Application.dataPath;
			}
			else{
				Constants.directory=Application.dataPath + "/Resources/Data";
			}
			SceneManager.LoadScene ("Main");
		}
	}

	private bool CheckOkay(){
		return true;
	}
}
