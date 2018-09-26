using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class LogHandler : MonoBehaviour {
	private static StreamWriter sw;
	public static LogHandler Instance;

	void Awake(){
		if (!File.Exists(Application.dataPath + "/Study_Data/" + Constants.idOne + "/" + Constants.idOne + "-" + Constants.round + ".txt"))
		{
			if (!Directory.Exists(Application.dataPath + "/Study_Data/" + Constants.idOne))
			{
				Directory.CreateDirectory(Application.dataPath + "/Study_Data/" + Constants.idOne);
			}
		}
		sw = new StreamWriter (Application.dataPath+"/Study_Data/" + Constants.idOne + "/" + Constants.idOne + "-" + Constants.round + ".txt");
		Instance = this;
		LogHandler.Instance.WriteLine ("Study Start:  time = " + Time.time);
	}

	public void WriteLine(string line){
		try{
			sw.WriteLine (line);
		}
		catch(ObjectDisposedException e){
			return;
		}
	}

	public void CloseWriter(){
		try{
			sw.Close ();
		}
		catch(ObjectDisposedException e){
			return;
		}
	}

}
