using UnityEngine;
using System.Collections;

public class moveTerror : MonoBehaviour {
		public float turnSpeed = 10.0f;
		public float moveSpeed = 10.0f;
		public float mouseTurnMultiplier = 1.0f;	
		public float sprintMult = 2.0f;
		public int jumpForce = 35000;
		public float jumpThreshold = 1.5f;
 

		private float rotate;
		private float forward;
		private float strafe;
	
	private Collider ai = null;
	private bool TCaught = false;
	private bool close = false;
	private string textString = "Press E to infect";
	
	public NetworkManagerScript netScript;
	
	// Use this for initialization
	void Start () {
		netScript = (NetworkManagerScript) GameObject.Find("NetworkManager").GetComponent("NetworkManagerScript");
	}
	
	void OnNetworkInstantiate(NetworkMessageInfo info) {
        Debug.Log("New object instantiated by " + info.sender);
		string sender = info.sender.ToString();
		
		if (!Network.isServer) {
			Debug.Log("terror instant cams = " + Camera.allCameras.Length);
			for (int i = 0; i < Camera.allCameras.Length; i++) {
				Debug.Log ("Camera[" + i + "] = " + Camera.allCameras[i]);
			}
			if (!sender.Equals ("-1")) {
				//Camera.allCameras[Camera.allCameras.Length - 1].enabled = false;
				Camera.allCameras[1].enabled = false;
				GameObject[] launchers = GameObject.FindGameObjectsWithTag("Launcher");
				Debug.Log ("Number of launchers = " + launchers.Length);
				if(launchers.Length != 1) {
					Debug.Log ("Disabling Launcher");
					launchers[launchers.Length - 1].SetActive(false);
				}
			} else {
				Camera.allCameras[1].enabled = false;
			}
				
		}
		/*if (!Network.isServer) {
			if (sender.Equals ("-1")) {
				Debug.Log ("Camera[0] tag = " + Camera.allCameras[0].tag);
				Debug.Log ("Camera[1] tag = " + Camera.allCameras[1].tag);
				//Debug.Log ("Camera[2] tag = " + Camera.allCameras[2].tag);
				Camera.allCameras[1].enabled = false;
				Camera.allCameras[0].enabled = true;
				//Camera.allCameras[2].enabled = true;
				
			} else {
				Debug.Log ("Disabling foreign camera");
				Camera.allCameras[Camera.allCameras.Length-1].enabled = false;
				launchers[launchers.Length - 1].SetActive(false);
				//Debug.Log("2cams = " + Camera.allCameras.Length);
				//Camera.allCameras[1].enabled = true;
			}
		}*/
    }
	
	void Jump(){
		rigidbody.AddForce(Vector3.up * jumpForce);	
	}
	
	// Update is called once per frame
	void Update () {
		if (networkView.isMine) {
			// x is used for the x axis.  set it to zero so it doesn't automatically rotate
		    float x = 0;
		
		    //normalise speed so strafe and forward are added euclidean
		    float speed = moveSpeed;
		    if((Input.GetAxis("Vertical")!=0)&&(Input.GetAxis("Horizontal")!=0)){
		        speed = Mathf.Sqrt((speed*speed)/2.0f);
		    }
		
		    // check to see if the W or S key is being pressed.  
		    forward = Input.GetAxis("Vertical") * Time.deltaTime * speed;
		    strafe = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
		    
			if (Input.GetKeyDown(KeyCode.E) && close) {
				//ai.transform.parent = transform;
				//BroadcastMessage("infect");
				//ai.transform.parent = null;
				SphereScript aiScript = (SphereScript) ai.GetComponent("SphereScript");
				aiScript.localInfect();
				int aiID = aiScript.aiID;
				netScript.infected(aiID);
			}
			
		    //enable sprint sped mult
		    if(Input.GetKey(KeyCode.LeftShift)){
		    	forward = forward * sprintMult;
		    }
		
			if (Input.GetKey (KeyCode.L)) {
				Debug.Log ("TCaught = " + TCaught);
			}
			
		    // Move the character forwards or backwards
		    transform.Translate(strafe, 0, forward); 
		
		    // jump the player if very close to the ground - prevents double jumps
		    if(Input.GetKeyDown(KeyCode.Space)&& transform.position.y < jumpThreshold){
		        Jump();
		    }     
		
		    // Get the difference in horizontal mouse movement
			if(!Input.GetKey("mouse 1")){	
		    	x = Input.GetAxis("Mouse X") * turnSpeed * mouseTurnMultiplier;    
	
			    // rotate the character based on the x value
			   	transform.Rotate(0, x, 0);
			}
		} else {
			enabled = false;
		}
	}
	
	void Capture(Collider other) {
		//networkView.RPC ("TeCaught",RPCMode.Server,networkView.owner);
		Debug.Log ("isMine? " + networkView.isMine + ", owner - " + networkView.owner);
		//menu.terrorCaught(networkView.owner);
		Debug.Log ("Terrorist is caught");
		//TCaught = true;
	}
	
	/*void SetGUI(Collider other) {
		//Debug.Log("Set Called");
		close = true;
		other.transform.parent = null;
		ai = other;
	}
	
	void UnSetGUI(Collider other) {
		//Debug.Log("Un-Set Called");
		close = false;
		other.transform.parent = null;
		ai = null;
	} */
	
	void OnTriggerEnter(Collider other) {
		//if (other.CompareTag ("Healer")) {
		//	transform.parent = other.transform;
		//	SendMessageUpwards("SetCaptureGUI", collider);
		//}
		if (other.CompareTag("AI")) {
			close = true;
			ai = other;
		}
	}
	
	void OnTriggerExit(Collider other) {
		//if (other.CompareTag ("Healer")) {
		//	transform.parent = other.transform;
		//	SendMessageUpwards("UnSetCaptureGUI", collider);
		//}
		if (other == ai) {
			close = false;
			ai = null;
		}
	}
	
	[RPC]
	void TeCaught() {
		Debug.Log ("Caught sent across network");
		TCaught = true;
	}
	
	void OnGUI () {
		//Debug.Log(Caught);
		if (TCaught) {
			Debug.Log ("Show Terrorist caught");
			GUI.Label (new Rect(10, 10, 100, 200), "You Lose!\n\n You were caught by the Containment Specialists");
			Time.timeScale = 0.01F;
		} else {
			if (close) {
				GUI.Label (new Rect(25, 25, 100, 30), textString);
			}
		}
	}
}
