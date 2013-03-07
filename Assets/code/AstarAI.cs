using UnityEngine;
using System.Collections;
//Note this line, if it is left out, the script won't know that the class 'Path' exists and it will throw compiler errors
//This line should always be present at the top of scripts which use pathfinding
using Pathfinding;
public class AstarAI : MonoBehaviour {
	
	//CivilianAIEntity aiControl;
	
    //The point to move to
    public Vector3 targetPosition;
    
    private Seeker seeker;
    private CharacterController controller;
 
    //The calculated path
    public Path path;
    
    //The AI's speed per second
    public float speed = 500;
    
    //The max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 3;
 
    //The waypoint we are currently moving towards
    private int currentWaypoint = 0;
 
    public void Start () {
		if (Network.isServer) {
			//aiControl = new CivilianAIEntity(new Vector(100,100)); //This position won't really work, and it will need to convert easily to / from Vector3
			Vector destination = new Vector(UnityEngine.Random.Range(-200,200),UnityEngine.Random.Range(-200,200));
			while (queryPoint.isWalkable(destination) == false){
				destination = new Vector(UnityEngine.Random.Range(-200,200),UnityEngine.Random.Range(-200,200));
			}
			//aiControl.setDestinationWaypoint(destination);
			
	        seeker = GetComponent<Seeker>();
	        controller = GetComponent<CharacterController>();
	        
	        //Start a new path to the targetPosition, return the result to the OnPathComplete function
	        //seeker.StartPath (transform.position,targetPosition, OnPathComplete);
			newPath();
		} else {
			enabled = false;
		}
    }
    
    public void OnPathComplete (Path p) {
        //Debug.Log ("Yey, we got a path back. Did it have an error? "+p.error);
        if (!p.error) {
            path = p;
            //Reset the waypoint counter
            currentWaypoint = 0;
        }
    }
	
	public void newPath(){
		
		Vector destination = new Vector(UnityEngine.Random.Range(-200,200),UnityEngine.Random.Range(-200,200));
			while (queryPoint.isWalkable(destination) == false){
				destination = new Vector(UnityEngine.Random.Range(-200,200),UnityEngine.Random.Range(-200,200));
			}
		
		/*targetPosition.x = (float)aiControl.getDestinationWaypoint().x();
		targetPosition.z = (float)aiControl.getDestinationWaypoint().y();*/
		targetPosition.x = (float)destination.x();
		targetPosition.z = (float)destination.y();
		targetPosition.y = 1f;
		seeker.StartPath (transform.position,targetPosition, OnPathComplete);
	}
 
    public void Update () {
		double dt = Time.deltaTime;
        if (path == null) {
            //We have no path to move after yet
            return;
        }
        
        if (currentWaypoint >= path.vectorPath.Count) {
            //Debug.Log ("End Of Path Reached");
			currentWaypoint = 0;
			//aiControl.setPosition(new Vector(targetPosition.x, targetPosition.z));
			//aiControl.update(dt);
			newPath();
            return;
        }
		
		
		//aiControl.update(dt);
        
        //Direction to the next waypoint
        Vector3 dir = (path.vectorPath[currentWaypoint]-transform.position).normalized;
        dir *= speed * Time.fixedDeltaTime;
        
        controller.SimpleMove (dir);
		
		
		//aiControl.setPosition(new Vector(transform.position.x,transform.position.z));
		
        
        //Check if we are close enough to the next waypoint
        //If we are, proceed to follow the next waypoint
        if (Vector3.Distance (transform.position,path.vectorPath[currentWaypoint]) < nextWaypointDistance) {
            currentWaypoint++;
            return;
        }
    }
} 