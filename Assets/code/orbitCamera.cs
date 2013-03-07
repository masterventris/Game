using UnityEngine;
using System.Collections;

public class orbitCamera : MonoBehaviour {
	public Transform target;

	float distance = 10.0f; 

	//float xSpeed = 10.0f;

	public float ySpeed = 300.0f; 

	float yMinLimit = -10f;

	float yMaxLimit = 80f; 

	float zoomRate = 50f;
	
	float previousZoom = 0.0f;

	private float x = 0.0f;

	private float y = 0.0f;
	
	
	// Use this for initialization
	void Start () {
		
    	x = target.localEulerAngles.y;
    	y = target.localEulerAngles.x;
     
    	if (rigidbody)
        	rigidbody.freezeRotation = true;
		
	}
	// Update is called once per frame
	void LateUpdate () {
	 	

	    if (target) {
			
			if(Input.GetKey("mouse 1")){
				x += Input.GetAxis("Mouse X") * ySpeed * 0.02f;
			
			}else{
				
	        	x = target.localEulerAngles.y;
				
			}
			
	        y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
	
	        	
	        
	
	        distance += -(Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime) * zoomRate * Mathf.Abs(distance);
	
	        if (distance < 2.5){	
	            distance = 2.5f;	
	        }
	
	        if (distance > 18){	
	            distance = 18f;	
	        }
	
	        
	
	        
	
	        y = ClampAngle(y, yMinLimit, yMaxLimit);
	
	               
	
	        //Debug.Log("y: "+y+" test: "+test);
	
	        
	
	        if( y == yMinLimit || previousZoom > 0)
	
	        {
	
	            // This is to allow the camera to slide across the bottom if the player is too low in the y 
	
	            distance += -(Input.GetAxis("Mouse Y") * Time.deltaTime) * 10f * Mathf.Abs(distance);
				previousZoom+= Input.GetAxis("Mouse Y");
				//Debug.Log(previousZoom);
	
	        }
	
	        
	
	        transform.rotation = Quaternion.Euler(y, x, 0);
			
			Vector3 offset = new Vector3 (0.0f, 2.0f, -distance);
	
	        transform.position = transform.rotation * offset + target.position;
	

	         
	
	    }
	}
	
	static float ClampAngle (float angle, float min, float max){

    if (angle < -360)

        angle += 360;

    if (angle > 360)

        angle -= 360;

    return Mathf.Clamp (angle, min, max);

}
}
