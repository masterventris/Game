using UnityEngine;
using System.Collections;

public class Policeman : MonoBehaviour {
	PolicemanAIEntity aiControl;
	
	

	// Use this for initialization
	void Start () {
		aiControl = new PolicemanAIEntity();
		Debug.Log ("lol");
		Debug.Log("npc class " + EntityManager.globalInstance().numberOfEntities());
	}
	
	// Update is called once per frame
	void Update () {
		double dt = Time.deltaTime;
		aiControl.update(dt);
		transform.Translate(0.1f,0,0); //This moves it 0.1x and 0y
	}
}