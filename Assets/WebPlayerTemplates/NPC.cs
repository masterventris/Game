using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour {
	
	CivilianAIEntity aiControl;
	
	Vector currentWayPoint;
	
	

	// Use this for initialization
	void Start () {
		aiControl = new CivilianAIEntity();
		Debug.Log ("lol");
		Debug.Log("npc class " + EntityManager.globalInstance().numberOfEntities());
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(0.1f,0,0); //This moves it 0.1x and 0y
	}
}
