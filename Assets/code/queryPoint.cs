using UnityEngine;
using System.Collections;
using Pathfinding;

public class queryPoint{
	
	// Returns boolean for if destination Vector3 is valid
	// Vector3 created with "Vector3 destination = new Vector3(x,y,z)", with y likely to be 0
	
	static public bool isWalkable(Vector destination){
		Vector3 temp = new Vector3((float)destination.x(),1.0f,(float)destination.y());
		Node node = AstarPath.active.GetNearest(temp).node;
		if(node.walkable){
			//Debug.Log("Valid Position");
			return true;
		}else{
			//Debug.Log("Invalid Position");
			return false;
		}
	}
	
	
	
}
