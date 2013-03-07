var turnSpeed = 10.0;

var moveSpeed = 10.0;

var mouseTurnMultiplier = 1.0;

var sprintMult = 2.0;

var jumpSpeed = 35000.0;

var jumpThreshold = 1.5;

 

private var rotate : float;

private var forward : float;

private var strafe : float;

function OnCollisionEnter (collision : Collision) {   //stop movement on collision      
    if(Input.GetAxis("Vertical")){
    	forward = 0;
    }    
    
}

function Jump(){
    // jump function - allows for calling jump animation etc easily
    rigidbody.AddForce(Vector3.up * jumpSpeed);
} 

function Update () 

{

    // x is used for the x axis.  set it to zero so it doesn't automatically rotate

    x = 0;

    //normalise speed so strafe and forward are added euclidean

    var speed = moveSpeed;
    if(Input.GetAxis("Vertical")&&Input.GetAxis("Horizontal")){
        speed = Mathf.Sqrt((speed*speed)/2.0);
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

    x = Input.GetAxis("Mouse X") * turnSpeed * mouseTurnMultiplier;    

 

    // rotate the character based on the x value

    transform.Rotate(0, x, 0);

}