using UnityEngine;
using System.Collections;

public class PlayerCubeLocSet : MonoBehaviour {
	
	public Transform player;
	float x;

	// Use this for initialization
	void Start () {
		
		float x = player.transform.position.x;
		float z = player.transform.position.z;
		
		z += 3;
		
		//transform.Translate(x,1,z);
		//transform.position.z = z;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
