using UnityEngine;
using System.Collections;

public class SphereScript : MonoBehaviour {
	private bool infected = false;
	private float infectTime = 0f;
	private float infectionRate = 0.79f;
	
	public int aiID = 0;
	public bool dead = false;
	//private bool deadPosition = false;
	private bool starOff = false;
	private bool[] colourPhase = {false, false, false};
	
	private NetworkManagerScript netScript;
	
	void Start() {
		netScript = (NetworkManagerScript) GameObject.Find("NetworkManager").GetComponent("NetworkManagerScript");
	}
	
	void OnTriggerEnter(Collider other) {
		if (Network.isServer) {
			if (infected) {
				if (other.CompareTag("AI")) {
					Debug.Log ("Attempting infect");
					//Generate a random number to try and infect other ai
					float rnd = Random.Range (0f, 1f);
					Debug.Log ("rnd = " + rnd + ", infectionRate = " + infectionRate);
					if (rnd > infectionRate) {
						Debug.Log ("Infecting other ai");
						//Infect other ai
						netScript.selfInfect();
						SphereScript aiScript = (SphereScript) other.gameObject.GetComponent("SphereScript");
						aiScript.infect();
					}
				}
			}
		}
	}
	
	void OnNetworkInstantiate(NetworkMessageInfo info) {
		float rnd = Random.Range (150f, 210f);
		((AstarAI) gameObject.GetComponent("AstarAI")).speed = rnd;	
	}
	
	void OnTriggerExit(Collider other) {
	}
	
	public void giveID(int number) {
		networkView.RPC ("netGiveID",RPCMode.OthersBuffered,number);
	}
	
	public void localInfect() {
		renderer.material.color = new Color(0,1,0);
	}
	
	public void infect() {
		infectTime = Time.time;
		//renderer.material.color = new Color(0,1,0);
		networkView.RPC ("netInfect",RPCMode.AllBuffered);		
	}
	
	public void heal() {
		networkView.RPC ("netHeal", RPCMode.AllBuffered);
	}
	
	private float tChange = 0; // force new direction in the first Update
	private float randomX = 0;
	private float randomY = 0;
	
	void Update () {	
		if (Network.isServer) {
			if (!dead && !netScript.gameOver) {
				if (infected) {
					float time = Time.time;
					bool change = false;
					Vector3 colour = new Vector3(0, 0, 0);
					if (infectTime + 80 < time) {
						//Kill
						netScript.aiDead();
						networkView.RPC("netKill", RPCMode.AllBuffered);
						
					} else {
						if (infectTime + 60 < time && !colourPhase[2]) {
							//Set to brightest colour
							colour = new Vector3(1, 0, 0);
							colourPhase[2] = true;
							change = true;
						} else {
							if (infectTime + 40 < time && !colourPhase[1]) {
								//Set to medium colour
								colour = new Vector3(1, 98f/255f, 98f/255f);
								colourPhase[1] = true;
								change = true;
							} else {
								if (infectTime + 20 < time && !colourPhase[0]) {
									//Set to light colour
									colour = new Vector3(1, 187f/255f, 187f/255f);
									colourPhase[0] = true;
									change = true;
								}
							}
						}
					}
					if (change) {
						networkView.RPC ("changeColour", RPCMode.AllBuffered, colour);
					}
				}
				
				//Move the ai
			    /*if (Time.time >= tChange){
			        randomX = Random.Range(-2.0F,2.0F); // with float parameters, a random float
			        randomY = Random.Range(-2.0F,2.0F); //  between -2.0 and 2.0 is returned
			        // set a random interval between 0.5 and 1.5
			        tChange = Time.time + Random.Range(0.5F,1.5F);
			    }
			    transform.Translate(new Vector3(randomX,0,randomY) * 1.0F * Time.deltaTime);*/
				
			} else {
				//Move to dead position
				if (dead && !starOff) {
					AstarAI aiScript = (AstarAI) gameObject.GetComponent("AstarAI");
					aiScript.enabled = false;
					Destroy (rigidbody);
					transform.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
					starOff = true;
				}
			}
		}
	}
	
	[RPC]
	void netGiveID(int number) {
		aiID = number;
	}
	
	[RPC]
	void netHeal() {
		Debug.Log ("Sphere healed");
		infected = false;
		renderer.material.color = new Color(1, 1, 1);
	}
	
	[RPC]
	void netInfect() {
		Debug.Log ("Sphere infected");
		infected = true;
		tag = "Infected";
	}
	
	[RPC]
	void netKill() {
		dead = true;
		tag = "DeadAI";
		infectionRate = 0.94f;
		renderer.material.color = new Color(0, 0, 0);
	}
	
	[RPC]
	void changeColour(Vector3 colour) {
		renderer.material.color = new Color(colour.x, colour.y, colour.z);
	}
}
