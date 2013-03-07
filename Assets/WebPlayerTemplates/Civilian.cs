using UnityEngine;
using System.Collections;

public class Civilian : MonoBehaviour {
	CivilianAIEntity aiControl;
	
	

	// Use this for initialization
	void Start () {
		//changed= 0;
		aiControl = new CivilianAIEntity(new Vector(100,100)); //This position won't really work, and it will need to convert easily to / from Vector3
		aiControl.setDestinationWaypoint(new Vector(0,0));
		Debug.Log ("lol");
		Debug.Log("npc class " + EntityManager.globalInstance().numberOfEntities());
	}
	
	// Update is called once per frame
	void Update () {
		double dt = Time.deltaTime;
		aiControl.update(dt);
		//transform.Translate(0.1f,0,0); //This moves it 0.1x and 0y
		Vector3 pos;
		pos.x = (float)aiControl.getDestinationWaypoint().x();
		pos.z = (float)aiControl.getDestinationWaypoint().y();
		pos.y = 1f;
		transform.position = pos; //This is how I set the entities position, so I use steering behaviours to calculate the position, set it here.
		aiControl.setPosition(new Vector(pos.x,pos.z));
	}
}