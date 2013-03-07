
using UnityEngine;
using System.Collections;

public class moveHealer : MonoBehaviour {
		public float turnSpeed = 10.0f;
		public float moveSpeed = 10.0f;
		public float mouseTurnMultiplier = 1.0f;	
		public float sprintMult = 2.0f;
		public int jumpForce = 35000;
		public float jumpThreshold = 1.0f;
 

		private float rotate;
		private float forward;
		private float strafe;
	
	private Collider ai = null;
	
	//private bool Caught = false;
	private bool close = false;
	private bool capture = false;
	private string textString = "Press E to heal";
	
	private NetworkManagerScript netScript;
	
	
	// Use this for initialization
	void Start () {
		netScript = (NetworkManagerScript) GameObject.Find("NetworkManager").GetComponent("NetworkManagerScript");
	}
	
	void OnNetworkInstantiate(NetworkMessageInfo info) {
        Debug.Log("New object instantiated by " + info.sender);
		string sender = info.sender.ToString();
		if (!Network.isServer) {
			if (!sender.Equals ("-1")) {
				Debug.Log("healer instant cams = " + Camera.allCameras.Length);
				for (int i = 0; i < Camera.allCameras.Length; i++) {
					Debug.Log ("Camera[" + i + "] = " + Camera.allCameras[i]);
				}
				//Camera.allCameras[Camera.allCameras.Length - 1].enabled = false;
				Camera.allCameras[1].enabled = false;
				GameObject[] launchers = GameObject.FindGameObjectsWithTag("Launcher");
				Debug.Log ("Number of launchers = " + launchers.Length);
				//launchers[launchers.Length - 1].SetActive(false);
			} else {
				Camera.allCameras[1].enabled = false;
			}
		}
				
		
		
		
		/*Debug.Log("cams = " + Camera.allCameras.Length);
		string sender = info.sender.ToString();
		GameObject[] launchers = GameObject.FindGameObjectsWithTag("Launcher");
		Debug.Log ("Number of launchers = " + launchers.Length);
		if (!Network.isServer) {
			if (sender.Equals ("-1")) {
				Camera.allCameras[1].enabled = false;
				Camera.allCameras[0].enabled = true;
			} else {
				Camera.allCameras[Camera.allCameras.Length-1].enabled = false;
				launchers[launchers.Length - 1].SetActive(false);
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
		    if((Input.GetAxis("Vertical") != 0) && (Input.GetAxis("Horizontal") != 0)){
		        speed = Mathf.Sqrt((speed*speed)/2.0f);
		    }
		
		    // check to see if the W or S key is being pressed.  
		    forward = Input.GetAxis("Vertical") * Time.deltaTime * speed;
		    strafe = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
		    
			
			if (Input.GetKeyDown(KeyCode.E) && close) {
				//ai.transform.parent = transform;
				//BroadcastMessage("heal");
				//ai.transform.parent = null;
				SphereScript aiScript = (SphereScript) ai.GetComponent("SphereScript");
				int aiID = aiScript.aiID;
				netScript.healed(aiID);
			} else {
				if (Input.GetKeyDown(KeyCode.E) && capture) {
					//ai.transform.parent = transform;
					//BroadcastMessage("Capture", collider);
					
					//moveTerror terrorScript = (moveTerror) ai.gameObject.GetComponent("moveTerror");
					//terrorScript.menu.terrorCaught(ai.networkView.owner);
					
					ai.tag = "CaughtTerrorist";
					for (int i = 0; i < ai.transform.childCount; i++) {
						Debug.Log("Child " + i + " - tag = " + ai.transform.GetChild(i));	
						if (ai.transform.GetChild(i).CompareTag("Terrorist")) {
							ai.transform.GetChild(i).tag = "CaughtTerrorist";
						}
					}
					capture = false;
					close = false;
					
					NetworkViewID aiID = ai.networkView.viewID;
					string idString = aiID.ToString();
					Debug.Log ("idString = " + idString);
					//string firstChar = idString.Substring(0,1);
					//string newID = firstChar + "00";
					string newID = "AllocatedID: ";
					//if (idString.Contains("5")) 
					//	netScript.terrorCaught(newID + "500");
					//else
					//	netScript.terrorCaught(newID + "900");
					if (idString.EndsWith("2")) {
						newID = idString.Substring(0,idString.Length - 1);
						newID += "0";
					} else {
						newID = idString;
					}
					netScript.terrorCaught(newID);	
					capture = false;
					close = false;
					ai = null;
					
					//ai.GetInstanceID
					
					Debug.Log ("Healer - caught");
					//Caught = true;
					//ai.transform.parent = null;
				}
			}
			
		    //enable sprint sped mult
		    if(Input.GetKey(KeyCode.LeftShift)){
		    	forward = forward * sprintMult;
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
	
	void infect() {
	}
	
	//void caught() {
		//Caught = true;
	//}
	
	void Capture(Collider other) {
		
	}
	
	/*void SetGUI(Collider other) {
		//Debug.Log("Set Called");
		other.transform.parent = null;
		if (!capture) {
			close = true;
			ai = other;
		}
	}
	
	void UnSetGUI(Collider other) {
		//Debug.Log("Un-Set Called");
		close = false;
		other.transform.parent = null;
		ai = null;
	}
	
	void SetCaptureGUI(Collider other) {
		//Debug.Log("Set Called");
		other.transform.parent = null;
		capture = true;
		close = false;
		ai = other;
	}
	
	void UnSetCaptureGUI(Collider other) {
		//Debug.Log("Un-Set Called");
		//close = false;
		capture = false;
		other.transform.parent = null;
		ai = null;
	} */
	
	void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Infected")) {
			if (!capture) {
				ai = other;
				close = true;
			}
		} else {
			if (other.CompareTag("Terrorist")) {
				capture = true;
				close = false;
				ai = other;
			}
		}
	}
	
	void OnTriggerExit(Collider other) {
		if (other == ai) {
			close = false;
			capture = false;
			ai = null;
		}
	}
	
	void OnGUI () {
		//if (Caught) {
			//GUI.Label (new Rect(10, 10, 100, 200), "You Win!\n\nYou caught the Bio-Terrorist!");
			//Time.timeScale = 0;
		string captureString = "Press E to capture the Bio-Terrorist!";
		if (close && ai != null) {
			GUI.Label (new Rect(25, 25, 100, 30), textString);
		}
		if (capture && ai != null) {
			GUI.Label (new Rect(25, 25, 300, 30), captureString);
		}
	}
}
