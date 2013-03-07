using UnityEngine;
using System.Collections;

public class pauseGame : MonoBehaviour {
	int paused = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
	     
		if (Input.GetKeyDown(KeyCode.P)) 
		{
	        switch (paused) 
			{
	            
				case 1: 
					unPause(); 
					break;
 
				default: 
					pause();
					break;
	        }
	    }
	}
	
	void pause(){
		Debug.Log("Paused");
		Time.timeScale = 0;
		paused = 1;
	
	}
	
	void unPause(){
		Debug.Log("Resumed");
		Time.timeScale = 1;
		paused = 0;
	}
	/*
	Object[] objects = FindObjectsOfType (typeof(GameObject));
	for each (GameObject go in objects) {
		go.SendMessage ("OnPauseGame", SendMessageOptions.DontRequireReceiver);
	}*/
}
