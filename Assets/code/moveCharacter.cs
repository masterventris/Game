using UnityEngine;
using System.Collections;

public class moveCharacter : MonoBehaviour {
		public float turnSpeed = 10.0f;
		public float moveSpeed = 10.0f;
		public float mouseTurnMultiplier = 1.0f;	
		public float sprintMult = 2.0f;
		public int jumpForce = 35000;
		public float jumpThreshold = 1.5f;
 

		private float rotate;
		private float forward;
		private float strafe;
	
	
	// Use this for initialization
	void Start () {
		
	}
	
	void Jump(){
		rigidbody.AddForce(Vector3.up * jumpForce);	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
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
	}
}
