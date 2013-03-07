var Paused = false;


function Awake(){
	Screen.lockCursor = true;
}

function Update(){


	if(Input.GetKeyDown(KeyCode.P)){

		if(Paused == false){
			Screen.lockCursor = false;
			Paused = true;
			Debug.Log("paused");
		}else{
			Screen.lockCursor = true;
			Paused = false;
			Debug.Log("unpaused");
		}
	}
}