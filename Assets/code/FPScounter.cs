using UnityEngine;
using System.Collections;

public class FPScounter : MonoBehaviour {

	public GUIText fpstext;
	
	// Update is called once per frame
	void Update () {
		
		double fps = 1/ Time.deltaTime;
		
		fpstext.text = fps.ToString();
		
	
	}
}
