using UnityEngine;
using System.Collections;

public class NetworkManagerScript : MonoBehaviour {
	private float btnX;
	private float btnY;
	private float btnW;
	private float btnH;	
	
	public string gameName = "FullBeards_Epidemic";
	public GameObject aiPrefab;
	public GameObject healPrefab;
	public GameObject terrorPrefab;
	public GameObject mapPrefab;
	public GameObject lightPrefab;
	
	public GameObject cameraPrefab;
	public GameObject cameraControllerPrefab;
	
	public Transform terrorSpawn;
	public Transform healSpawn;
	public Transform aiSpawn;
	private GameObject[] cameraSpawns;
	
	private GameObject myCharacter;
	
	private bool gameStarted = false;
	private float startTime;
	private bool refreshing = false;
	private string playerName = "Enter your name";
	private string serverIP = "Enter Server IP Address";
	private bool playerType = false; //false for terrorist, true for containment
	private bool serverStarted = false;
	public int playerNum;
	private bool serverFailed = false;
	private bool isClient = false;
	//private bool gameType = null;
	private HostData[] hostData;
	private NetworkPlayer netPlayer;
	private bool TCaught = false;
	
	private NetworkPlayer[] netPlayers = new NetworkPlayer[8];
	private string[] nameList = new string[9];
	private NetworkViewID[] netIDs = new NetworkViewID[9];
	
	private int playerCount = -1;
	private int readyCount = 1;
	//private short terrorCount = 0;
	
	private int healScore = 0;
	private int terrorScore = 0;
	private int[] scores = new int[8];
	
	private GameObject[] aiObjects;
	private int aiCount = 0;
	private int infectCount = 0;
	private int healedCount = 0;
	private int deadCount = 0;
	
	private float infectionPercent = 0;
	private bool terrorWin = false;
	public bool gameOver = false;
	
	private GUIText centreText;
	
	void Awake() {
		//DontDestroyOnLoad(this.gameObject);
	}
	
	// Use this for initialization
	void Start () {
		btnX = Screen.width;
		btnY = Screen.width;
		btnW = 75;
		btnH = 50;
		
		centreText = GameObject.FindGameObjectWithTag("CentreText").guiText;
		
		for (int i = 0; i < 8; i++) scores[i] = 0;
		
		Time.timeScale = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if (!Network.isClient && !Network.isServer) {
			if ((Time.time > startTime + 2) && refreshing) {
				refreshing = false;
				serverFailed = true;
				Debug.Log ("Server not found, starting new server");
				//startServer ();
			} else {
				if(refreshing) {
					if(MasterServer.PollHostList ().Length > 0) {
						refreshing = false;
						Debug.Log (MasterServer.PollHostList ().Length);
						hostData = MasterServer.PollHostList ();
						Debug.Log ("Server found, joining as client");
						connectClient();
					}
					
				}
			}
		} else {
			if (Network.isServer && readyCount == playerCount) {
				giveTypes();
			}
			if (Network.isServer) {
				int totalInfected = infectCount + deadCount;
				infectionPercent = (float) totalInfected / (float) (aiCount - healedCount);
				
				if (infectionPercent > 0.85) {
					//The terrorists have won the game
					networkView.RPC ("netTerrorWin",RPCMode.AllBuffered);
				}
			}
		}
	}

	//Tell server a terrorist is caught
	public void terrorCaught(string ID) {
		networkView.RPC("TerrorCaught", RPCMode.Server, ID);	
	}
	
	//Tell server someone has been healed
	//public void healed(NetworkViewID ID, int aiID) {
	public void healed(int aiID) {
		networkView.RPC ("aiHeal", RPCMode.Server, playerNum /*ID*/, aiID);
	}
	
	//Tell server someone has been infected
	public void infected(int aiID) {
		networkView.RPC ("aiInfect", RPCMode.Server, playerNum/*ID*/, aiID);
	}
	
	public void selfInfect() {
		infectCount++;
	}
	
	public void aiDead() {
		deadCount++;
		infectCount--;
	}
	
	void startServer() {
		if (!serverStarted) {
			Network.incomingPassword = "Panda";
			Network.InitializeServer(9, 25000, false);
			//serverStarted = true;
			//MasterServer.RegisterHost (gameName, "Epidemic", "Full Beards Game Epidemic");
			spawnAIPlayers();
		}
	}
	
	void connectClient() {
		for (int i = 0; i < hostData.Length; i++) {
			if (hostData[i].gameName != null)
				Network.Connect (hostData[i],"Panda");
		}
	}
	
	void directConnect() {
		Network.Connect(serverIP, 25000, "Panda");
	}
	
	void checkServer() {
		MasterServer.RequestHostList (gameName);
		refreshing = true;
		startTime = Time.time;
		Debug.Log ("Start Time = " + startTime);
	}
	
	void giveTypes() {
		for (int i = 1; i < playerCount; i++) {
			if (i == 1 || i == 5) networkView.RPC ("receiveType", netPlayers[i - 1], false);
			else networkView.RPC ("receiveType", netPlayers[i - 1], true);
		}
	}
	
	void spawnAIPlayers() {
		Vector3 tf = aiSpawn.position;
		for(int i = 0; i < 10; i++) {
			tf.x = aiSpawn.position.x;
			tf.z -= 2f;
			for (int j = 0; j < 10; j++) {
				tf.x -= 1.5f;
				//tf.z -= 1.5f;
				Rigidbody newAI = ((GameObject) Network.Instantiate(aiPrefab, tf, aiSpawn.rotation, 0)).rigidbody;
			}
			//yield return 1;
		}
	}
	
	void spawnPlayer() {
		Debug.Log ("SpawnPlayer Called");
		//Vector3 spawnPos = new Vector3(0f,10f,-2.5f + (5*playerNum));
		GameObject prefabType;
		Vector3 spawnPoint;
		if (playerType) {
			prefabType = healPrefab;
			spawnPoint = healSpawn.position;
			/*GameObject clone = (GameObject) Network.Instantiate (healPrefab,healSpawn.position,Quaternion.identity,0);
			NetworkView nView = clone.GetComponent<NetworkView>();
			NetworkViewID viewid = nView.viewID;
			networkView.RPC ("giveViewID", RPCMode.Server, playerNum, viewid);
			Debug.Log ("Healer Instantiated");*/
		} else {
			prefabType = terrorPrefab;
			spawnPoint = terrorSpawn.position;
			/*GameObject clone = (GameObject) Network.Instantiate (terrorPrefab,terrorSpawn.position,Quaternion.identity,0);
			NetworkView nView = clone.GetComponent<NetworkView>();
			NetworkViewID viewid = nView.viewID;
			networkView.RPC ("giveViewID", RPCMode.Server, playerNum, viewid);
			Debug.Log ("Terrorist Instantiated");*/
		}
		
		GameObject clone = (GameObject) Network.Instantiate (prefabType,spawnPoint,Quaternion.identity,0);
		NetworkView nView = clone.GetComponent<NetworkView>();
		NetworkViewID viewid = nView.viewID;
		networkView.RPC ("giveViewID", RPCMode.Server, playerNum, viewid);
		Debug.Log ("Player Instantiated");
		
		myCharacter = clone;
		
		Vector3 mapPosition = new Vector3(clone.transform.position.x, 200, clone.transform.position.z);
		GameObject miniMap = (GameObject) Instantiate (mapPrefab, mapPosition, clone.transform.rotation);
		miniMap.transform.parent = clone.transform;
		//disableCameras();
		//instantiated = true;
	}
	
	void OnConnectedToServer() {
		Debug.Log ("Successfully connected to server!");
		//Send playerName to server
		//networkView.RPC ("setPlayerInformation", RPCMode.Server, netPlayer, playerName);
	}
	
    void OnFailedToConnect(NetworkConnectionError error) {
        Debug.Log("Could not connect to server: " + error);
		serverFailed = true;
    }
	
	//check for successful registry
	void OnMasterServerEvent(MasterServerEvent mse) {
		if (mse == MasterServerEvent.RegistrationSucceeded) {
			if (!serverStarted) {
				serverStarted = true;
				Debug.Log ("Registered Server!");
				nameList[0] = playerName;
				netIDs[0] = networkView.viewID;
				playerType = false;
				playerCount = 1;
			}
		}
	}
	
	void OnServerInitialized() {
		Debug.Log ("Registered Server!");
		nameList[0] = playerName;
		netIDs[0] = networkView.viewID;
		playerType = false;
		playerCount = 1;
	}
	
	void OnPlayerConnected(NetworkPlayer player) {
		if (gameStarted) {
			Network.CloseConnection(player, true);
		} else {
			//networkView.RPC ("givePlayerList", RPCMode.Others, nameList, playerCount);
			networkView.RPC ("giveNetworkPlayer", player, player, playerCount);
		}
	}
	
	void OnDisconnectedFromServer(NetworkDisconnection info) {
		if (Network.isServer) {
			Debug.Log ("Lost connection to local server");
		} else {
			if (info == NetworkDisconnection.LostConnection) {
				Debug.Log ("Lost Connection to the server");
			} else {
				Debug.Log ("Successfully disconnected");
			}
		}
	}
	
	void launchGame() {
		//giveTypes ();
		if (Network.isServer) {
			GameObject[] ai = GameObject.FindGameObjectsWithTag("AI");
			aiCount = ai.Length;
			aiObjects = new GameObject[aiCount];
			for (int i = 0; i < aiCount; i++) {
				SphereScript script = (SphereScript) ai[i].GetComponent("SphereScript");
				script.giveID(i);
				aiObjects[i] = ai[i];
			}
			
			GameObject[] cameras = new GameObject[4];
			Instantiate(lightPrefab, new Vector3(0,250,0),Quaternion.identity);
			
			cameraSpawns = GameObject.FindGameObjectsWithTag("CameraSpawn");
			for (int i = 0; i < cameraSpawns.Length; i++) {
				cameras[i] = (GameObject) Instantiate(cameraPrefab, cameraSpawns[i].transform.position, cameraSpawns[i].transform.rotation);
			}
			
			Instantiate(cameraControllerPrefab, new Vector3(0,0,0), Quaternion.identity);
			for (int i = 0; i < 4; i++) {
				cameraTurn camScript = (cameraTurn) cameras[i].GetComponent("cameraTurn");
				camScript.setThreshold();
			}
		}
		networkView.RPC ("loadLevel", RPCMode.Others);
		
	}
	
	//[RPC]
	//void tellReady() {
	//	readyCount++;
	//}
	
	[RPC]
	void giveNetworkPlayer(NetworkPlayer player, int count) {
		netPlayer = player;
		if (count == 1 || count == 5) playerType = false;
		else playerType = true;
		playerNum = count - 1;
		//NetworkViewID id = networkView.viewID;
		
		//Send playerName to server
		networkView.RPC ("setPlayerInformation", RPCMode.Server, netPlayer, playerName);
	}
	
	[RPC]
	void giveViewID(int number, NetworkViewID id) {
		netIDs[number] = id;
	}
	
	[RPC]
	void setPlayerInformation(NetworkPlayer player, string name) {
		nameList[playerCount] = name;
		netPlayers[playerCount - 1] = player;
		playerCount++;
		for (int i = 0; i < playerCount; i++) {
			Debug.Log ("Player[" + i + "] = " + nameList[i]);
		}
		//Send new player other names
		for (int i = 0; i < playerCount - 1; i++)
			networkView.RPC ("setNames", player, nameList[i], i);
		
		//Send everyone new player's name
		networkView.RPC ("setNames", RPCMode.All, name, playerCount - 1);
	}
	
	[RPC]
	void setNames(string name, int count) {
		Debug.Log ("Count = " + count);
		nameList[count] = name;
		//nameList = names;
		playerCount = count + 1;
	}
	
	[RPC]
	void loadLevel() {
		//Application.LoadLevel("interface");
		gameStarted = true;
		Screen.showCursor = false;
		Screen.lockCursor = true;
		spawnPlayer();
	}
	
	[RPC]
	void aiHeal(int pNum, int aiID) {
		healScore += 10;
		healedCount++;
		infectCount--;
		
		SphereScript aiScript = (SphereScript) aiObjects[aiID].GetComponent("SphereScript");
		aiScript.heal ();
		
		scores[pNum] += 10;
	}
	
	/*[RPC]
	void aiHeal(NetworkViewID ID, int aiID) {
		healScore += 10;
		healedCount++;
		infectCount--;
		
		SphereScript aiScript = (SphereScript) aiObjects[aiID].GetComponent("SphereScript");
		aiScript.heal();
		
		Debug.Log ("Looking for id - " + ID);
		for (int i = 0; i < netIDs.Length; i++) {
			Debug.Log (i + ", ID = " + netIDs[i]);
			if (netIDs[i] == ID) {
				scores[i] += 10;
				i = 9;
			}
		}
	}*/
	
	[RPC]
	void aiInfect(int pNum, int aiID) { 
		terrorScore += 10;
		infectCount++;
		
		SphereScript aiScript = (SphereScript) aiObjects[aiID].GetComponent("SphereScript");
		aiScript.infect();
		
		scores[playerNum] += 10;
	}
	
	/*[RPC]
	void aiInfect(NetworkViewID ID, int aiID) {
		terrorScore += 10;
		infectCount++;
		
		Debug.Log ("aiID = " + aiID);
		SphereScript aiScript = (SphereScript) aiObjects[aiID].GetComponent("SphereScript");
		aiScript.infect();
		
		Debug.Log ("Looking for id - " + ID);
		for (int i = 0; i < netIDs.Length; i++) {
			Debug.Log (i + ", ID = " + netIDs[i]);
			if (netIDs[i] == ID) {
				scores[i] += 10;
				i = 9;
			}
		}
	}*/
		
	[RPC]
	void TerrorCaught(string terrorID) {
		//GameObject[] terrors = GameObject.FindGameObjectsWithTag("Terrorist");
		//for (int i = 0; i < terrors.Length; i++) {
		//}
		
		Debug.Log ("Looking for terrorist id - " + terrorID);
		for (int i = 0; i < netIDs.Length; i++) {
			Debug.Log (i + ", ID = " + netIDs[i]);
			string currentID = netIDs[i].ToString();
			if (currentID == terrorID) {
				Debug.Log ("TerrorCaught - playerCount = " + playerCount);
				//There is only one terrorist
				if (playerCount < 6) {
					TCaught = true;
					gameOver = true;
				} else {
					//There are 2 terrorists
					if (TCaught) {
						gameOver = true;
					} else {
						TCaught = true;
					}
				}
				
				NetworkPlayer player = netPlayers[i];
				Debug.Log ("Server telling terrorist caught");
				//TCaught = true;
				networkView.RPC ("Caught",player);
				i = 9;
			}
		}
		
		if (gameOver) {
			networkView.RPC ("netTerrorLose",RPCMode.Others);
		}
			
	}
	
	[RPC]
	void Caught() {
		Debug.Log ("Terrorist received caught notification");
		myCharacter.tag = "CaughtTerrorist";
		Debug.Log ("Child tag = " + myCharacter.transform.GetChild(0).tag);
		TCaught = true;
	}
	
	[RPC]
	void netTerrorWin() {
		terrorWin = true;
		gameOver = true;
	}
	
	[RPC]
	void netTerrorLose() {
		gameOver = true;
	}
	
	[RPC]
	void restartGame() {
		Time.timeScale = 1f;
		Network.Disconnect ();
		Application.LoadLevel(0);
	}
	
	void OnGUI() {
		if (!Network.isClient && !Network.isServer) {
			if (!isClient) {
				if (GUI.Button (new Rect(btnX*0.2f, btnY*0.1f, btnW, btnH), "Server")) {
						Debug.Log ("I am Server.");
						//checkServer();
						startServer();
				}
				if (GUI.Button (new Rect(btnX*0.4f, btnY*0.1f, btnW, btnH), "Client")) {
						Debug.Log ("I am Client.");
						isClient = true;
						//checkServer();
						//startServer();
				}
			} else {
				playerName = GUI.TextArea (new Rect (btnX*0.1f, btnY*0.1f, btnW+100, btnH-25), playerName);
				//if (GUI.Button (new Rect(btnX*0.1f, btnY*0.15f, btnW+100, btnH), "Search for Server") && playerName != "Enter your name") {
				//	Debug.Log ("Looking for Server...");
					//checkServer();
				//}
				serverIP = GUI.TextField (new Rect (btnX*0.1f, btnY*0.3f,btnW+100, btnH-25), serverIP);
				if (GUI.Button (new Rect(btnX*0.1f, btnY*0.35f, btnW+100, btnH), "Connect") && serverIP != "Enter Server IP Address" && playerName != "Enter your name") {
					Debug.Log ("Attempting direct connect to " + serverIP);
					directConnect();
				}
			}
		} else {
			if (!gameStarted) {
				//Display lobby of players
				for (int i = 1; i < playerCount; i++) {
					float lblW = Screen.width * (float) 0.1;
					float lblH = Screen.height * (float) 0.07;
					float lblX = (Screen.width / 2) - lblW - 30;
					float lblY = Screen.height - ((lblH + 20) * (playerCount - i));
					
					GUI.Label (new Rect(lblX, lblY, lblW, lblH), nameList[i]);
					if (i == 1 || i == 5) GUI.Label (new Rect(lblX + lblW, lblY, lblW, lblH), "Terrorist");
					else GUI.Label (new Rect(lblX + lblW, lblY, lblW, lblH), "Healer");
				}
				
				if (Network.isServer) {
					if (GUI.Button (new Rect(btnX*0.1f, btnY*0.1f, btnW, btnH), "Launch Game")) {
						launchGame();
						gameStarted = true;
					}
				}
			} else {
				if (gameOver) {
					Screen.lockCursor = false;
					Screen.showCursor = true;
					if (terrorWin) {
						//Display a large message in the middle of the screen
						centreText.text = "Terrorists Win!";
						Time.timeScale = 0.0001f;
					} else {
						centreText.text = "Anti-Terrorists Win!";
						Time.timeScale = 0.0001f;
					}
					if (GUI.Button (new Rect(btnX*0.2f, btnY*0.1f, btnW, btnH), "Restart")) {
						if(Network.isServer) {
							networkView.RPC ("restartGame",RPCMode.Others);	
						}
						Time.timeScale = 1f;
						Network.Disconnect();
						Application.LoadLevel(0);
					}
				}
				
				if (Network.isServer) {
					//Display Scoreboard
					for (int i = 1; i < playerCount; i++) {
						float lblW = Screen.width * 0.1f;
						float lblH = Screen.height * 0.07f;
						float lblX = (Screen.width / 2) - lblW - 30;
						float lblY = Screen.height - ((lblH + 20) * (playerCount - i));
						
						GUI.Label (new Rect(lblX, lblY, lblW, lblH), nameList[i]);
						GUI.Label (new Rect(lblX + lblW, lblY, lblW, lblH), "" + scores[i - 1]);
					}
					
					GUI.Label (new Rect(Screen.width * 0.8f, Screen.height * 0.1f, 200f, 30f), "Current Infection");
					GUI.Label (new Rect(Screen.width * 0.8f, Screen.height * 0.15f, 200f, 30f), "" + (infectionPercent * 100));
				} else {
					if (TCaught) {
						Debug.Log ("Show Terrorist caught");
						//GUI.Label (new Rect(10, 10, 100, 200), "You Lose!\n\n You were caught by the Containment Specialists");
						centreText.text = "You have been caught by the Anti-Terrorists!";
						Time.timeScale = 0.0001F;
					}
				}
			}
		}
	}
}
