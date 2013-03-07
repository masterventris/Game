using UnityEngine;
using System.Collections;

public class AIspawn : MonoBehaviour {
	public Rigidbody AI;
	int i;
	// Use this for initialization
	void Start () {
		/*if (Network.isServer) {
			Vector3 tf = transform.position;
			for(int i=0;i<200;i++){
				tf.x -= 1.5f;
				tf.z -= 1.5f;
				Rigidbody newAI = (Rigidbody) Instantiate(AI, tf, transform.rotation);
				//yield return 1;
			}
		}*/
	}
	
	
}
