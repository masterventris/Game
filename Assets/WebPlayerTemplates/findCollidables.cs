using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FindCollidables : MonoBehaviour {
	
	public GameObject[] buildings;
	Vector3 bounds;
	Vector3 center;
	double radius;
	
	
	void Start () {
		
		
			buildings = GameObject.FindGameObjectsWithTag("Obstacles");
			//Debug.Log(buildings.Length);
			iterate();		
		
		
	
	}
	
	void iterate(){
		foreach(GameObject building in buildings){
			bounds = building.collider.bounds.size;
			center = building.transform.position;
			//print("building is " + bounds + " " + center);
			
			radius = Mathf.Max(bounds.x,bounds.y)/2.0;
			
			ObjectBounds current = new ObjectBounds(building, center,bounds,radius);
			
			worldColliders.addObject(current);
		}
	}
		
	
	
}

public class ObjectBounds{
	GameObject objectID;
	Vector3 bounds;
	Vector3 center;
	double radius;
	
	public ObjectBounds(){
	}
	
	public ObjectBounds(GameObject building, Vector3 center1, Vector3 bounds1, double radius1){
		this.objectID = building;
		this.center = center1;
		this.bounds = bounds1;
		this.radius = radius1;
	}
	
	public Vector3 getCenter(){
		return this.center;
	}
	
	public Vector3 getBounds(){
		return this.bounds;
	}
	
	public double getRadius(){
		return this.radius;
	}
	
	public GameObject getObject(){
		return this.objectID;
	}
	
	public Vector2[] getVertices(){
		Vector3 colliderCentre  = this.objectID.boxCollider.center;
    	Vector3 colliderExtents = this.objectID.boxCollider.extents;
		
		Vector3[] vertices = new Vector3[4];
		
		for (int i = 0; i != 4 ; i++){
			
        Vector3 extents = colliderExtents;

        extents.Scale (new Vector3((i & 1) == 0 ? 1 : -1, 1, (i & 2) == 0 ? 1 : -1));

        Vector3 vertexPosLocal = colliderCentre + extents;
		
		//return world space coords
		Vector3 newVertex = boxCollider.transform.TransformPoint(vertexPosLocal);
		vertices[i]= new Vector2(newVertex.x, newVertex.z);
			
    	}
		return vertices;
		
	}
		
	
	
}

public class WorldColliders{
	private static List<ObjectBounds> worldObjects = null;
	
	public static void addObject(ObjectBounds bound){
		if(worldObjects == null){
			worldObjects = new List<ObjectBounds>();
		}
		worldObjects.Add(bound);
	}
	
	public static ObjectBounds[] getObjectBoundsArray(){
		return worldObjects.ToArray();
	}
	
	public static List<ObjectBounds> getObjectBoundsList(){
		return worldObjects;
	
	}
		
		
	
}
