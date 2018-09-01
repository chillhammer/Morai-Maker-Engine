using UnityEngine;
using System.Collections;

public class SinglePlayerIDPlay : TextButton {
	public GUIText id;

	public override void ButtonPress (){
		if (id.text.Length == 4) {
			Constants.idOne = id.text;
			base.ButtonPress ();
		}
	}
}
