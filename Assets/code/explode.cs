using UnityEngine;
using System.Collections;

public class explode : MonoBehaviour {
	
	bool exploding = false;
	public NetworkManagerScript netScript;
	
	private SphereCollider sCol;
	
	
	
	// Use this for initialization
	void Start () {
		if (networkView.isMine) {
			netScript = (NetworkManagerScript) GameObject.Find("NetworkManager").GetComponent("NetworkManagerScript");
			sCol = (SphereCollider) gameObject.GetComponent("SphereCollider");
				//GetComponent<SphereCollider>();
			//sCol.enabled = false;
		} else {
			enabled = false;
		}
	}
	
	/*void OnCollisionEnter(Collision collision){
		//if(collision.gameObject.tag == "Ground"){
		if(collision.gameObject.CompareTag("Ground")) {
			Debug.Log("ground collide");
			exploding = true;
			sCol.enabled = true;
			
		}
	} */
	
	void OnTriggerEnter(Collider victim){
		//if(victim.gameObject.tag == "AI"){
		if(networkView.isMine) {
			if(exploding && victim.CompareTag("AI")) {
			//	Debug.Log ("GRENADE INFECTING ON THIS COMPUTER");
				SphereScript aiScript = (SphereScript) victim.GetComponent("SphereScript");
				int aiID = aiScript.aiID;
				aiScript.localInfect();
				netScript.infected(aiID);
			} else {
				if (victim.CompareTag("Ground") || victim.CompareTag("Building")) {
					exploding = true;
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(networkView.isMine) {
			if(exploding){
				sCol.radius = sCol.radius + 0.1f;
				if(sCol.radius > 10.0f){
					Network.Destroy(this.gameObject);
				}
			}
		}
	}
}
