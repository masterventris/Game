using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public class cameraController : MonoBehaviour {
	public Camera[] cameras;
	private List<Camera> normalDisplay=new List<Camera>();
	private List<Camera> priorityDisplay=new List<Camera>();
	private List<Camera> currentDisplay=new List<Camera>();
	
	private int minSuspicion=0;
	public int numberToDisplay=4;
	private int rows;
	private float dimen;
	private Boolean cameraChange=false;
	public int priorityThreshold=7;
	private int x=0;
	private int y=0;
	private int next=0;
	private double timelapse=0;
	// Use this for initialization
	void Start () {
		rows=(int)Math.Sqrt(numberToDisplay);
		var tempcameras =GameObject.FindGameObjectsWithTag("Security Camera");
		cameras=new Camera[tempcameras.Length];
		for(int i=0;i<tempcameras.Length;i++){
		cameras[i]=tempcameras[i].camera;
			
		}
		dimen=1/(float)rows;
		next=numberToDisplay;
		camerasToDisplay();
	}
	
	public void suspicionChanged(Camera c){
		Debug.Log ("suspicionChanged");
		int suspicion=c.GetComponent<cameraTurn>().suspicion;
		if(priorityDisplay.Contains(c)){
			if(suspicion<priorityThreshold){
				cameraChange=true;	
			}
		}
		else if(normalDisplay.Contains(c)){
			if(suspicion>=priorityThreshold||suspicion<minSuspicion){
					cameraChange=true;
			}
			
		}
		else if((normalDisplay.Count>0 && suspicion>=minSuspicion)||(priorityDisplay.Count>0 && suspicion>=priorityThreshold)){
			cameraChange=true;	
		}
		Debug.Log(cameraChange);
	}
	
	//Will need changing for dynamic numbertodisplay
	void setCurrentDisplay(){
		for(int j=0;j<rows;j++){
			for(int i=0;i<rows;i++){
				Debug.Log ("j: "+j+" i: "+i);
				if(j*rows+i<priorityDisplay.Count){
					if(currentDisplay.Count<4||currentDisplay[j*rows+i]!=priorityDisplay[j*rows+i]){
						priorityDisplay[j*rows+i].rect=new Rect((float)(i*(1.0/rows)),(float)((rows-j-1)*(1.0/rows)),(float)(1.0/rows),(float)(1.0/rows));
						priorityDisplay[j*rows+i].depth=1;
						if(rows*j+i<currentDisplay.Count){
							currentDisplay[rows*j+i].depth=0;
							currentDisplay[rows*j+i]=priorityDisplay[rows*j+i];
						}
						else{
							currentDisplay.Add (priorityDisplay[j*rows+i]);
						}
					}
				}
				else{
					normalDisplay[j*rows+i-priorityDisplay.Count].rect=new Rect((float)(i*(1.0/rows)),(float)((rows-j-1)*(1.0/rows)),(float)(1.0/rows),(float)(1.0/rows));
					normalDisplay[j*rows+i-priorityDisplay.Count].depth=1;
					if(rows*j+i<currentDisplay.Count){
						currentDisplay[rows*j+i].depth=0;
						currentDisplay[rows*j+i]=normalDisplay[j*rows+i-priorityDisplay.Count];
					}
					else{
						currentDisplay.Add (normalDisplay[j*rows+i-priorityDisplay.Count]);
					}
				}
			//	currentDisplay[j*rows+i].rect=new Rect((float)(i*(1.0/rows)),(float)(j*(1.0/rows)),(float)(1.0/rows),(float)(1.0/rows));
			//	currentDisplay[j*rows+i].rect=new Rect((float)(i*(1.0/rows)),(float)(j*(1.0/rows)),(float)(1.0/rows),(float)(1.0/rows));
			}
		}
		
	}
	
	void camerasToDisplay(){
		Debug.Log ("camerasToDisplay");
		Array.Sort(cameras,new CameraSorter());
		
		List<Camera> tempPriorityDisplay=new List<Camera>();
		List<Camera> tempNormalDisplay=new List<Camera>();
		List<Camera> tempCurrentDisplay=new List<Camera>();
		//priorityPrevDisplay=priorityDisplay;
		priorityDisplay.Clear();
		
		//normalPrevDisplay=normalDisplay;
		normalDisplay.Clear();
		
		
		for(int i=0;i<cameras.Length;i++){
			int suspicion=cameras[i].GetComponent<cameraTurn>().suspicion;
			if(i>numberToDisplay-1){
				if(suspicion<minSuspicion)
					break;
			}
			
			if(suspicion>priorityThreshold){
				tempPriorityDisplay.Add(cameras[i]);
				minSuspicion=priorityThreshold;
			}
			else{
				tempNormalDisplay.Add(cameras[i]);
				minSuspicion=suspicion;
			}
			
		}
		
		/*********************************************
		 THINK OF WAY TO WORK OUT WHICH TO CHANGE
		 ***********************************************/
		for(int i=0;i<currentDisplay.Count;i++){
			
			//Deals with priority
			if(tempPriorityDisplay.Contains(currentDisplay[i])){
				priorityDisplay.Add(currentDisplay[i]);
			}
			else{
				if(tempPriorityDisplay.Count<numberToDisplay){
					if(tempNormalDisplay.Contains(currentDisplay[i])){
						normalDisplay.Add(currentDisplay[i]);
					}
				}
			}
		}
		if(priorityDisplay.Count+normalDisplay.Count!=numberToDisplay){
			x=0; 
			y=0;
		}
		if(priorityDisplay.Count<numberToDisplay){
			next=normalDisplay.Count;	
		}
		else{
			next=priorityDisplay.Count;
		}
		
		for(int i=0;i<tempPriorityDisplay.Count;i++){
			if(!currentDisplay.Contains(tempPriorityDisplay[i])){
				priorityDisplay.Add(tempPriorityDisplay[i]);
			}
		}
		for(int i=0;i<tempNormalDisplay.Count;i++){
			if(!currentDisplay.Contains(tempNormalDisplay[i])){
				normalDisplay.Add(tempNormalDisplay[i]);
			}
		}
		setCurrentDisplay();
		
	}
	// Update is called once per frame
	void Update () {
		//Debug.Log ("Priority "+priorityDisplay.Count);
		//Debug.Log ("Normal "+normalDisplay.Count);
		
		
		timelapse+=Time.deltaTime;
		if(cameraChange){
			//("camerachange"+cameraChange);
			camerasToDisplay();
			timelapse=0;
			cameraChange=false;
		}
		else if(timelapse>2){
			timelapse=0;
			
			if(x==rows){
			//	Debug.Log ("i==rows");
				x=0;
				y++;
			}
			//Debug.Log ("j: "+j+" i: "+i);
			if(y>=rows){
				y=0;	
			}
			//Debug.Log ("j: "+j+" i: "+i);

			Debug.Log ("y: "+y+" x: "+x);
			if(priorityDisplay.Count+normalDisplay.Count!=numberToDisplay){
			Debug.Log ("entersif");

			//if less priority than spaces change normaldisplay index;
				if(priorityDisplay.Count<numberToDisplay){
				//rotate normal display
					if(rows*y+x>priorityDisplay.Count-1){
						if(!(next<normalDisplay.Count)){
							next=0;
						}
						Debug.Log ("here y: "+y+" x: "+x+"   "+(float)(x*(1.0/rows))+" "+(float)((rows-y-1)*(1.0/rows))+" "+(float)(1.0/rows)+" "+(float)(1.0/rows));
						normalDisplay[next].rect=new Rect((float)(x*(1.0/rows)),(float)((rows-y-1)*(1.0/rows)),(float)(1.0/rows),(float)(1.0/rows));
						normalDisplay[next].depth=1;
						//Debug.Log ("here y: "+y+" x: "+x);
						//Debug.Log (currentDisplay.Count);
						currentDisplay[rows*y+x].depth=0;
						currentDisplay[rows*y+x]=normalDisplay[next];
						next++;
					}
				}
				else{
					if(!(next<priorityDisplay.Count)){
						next=0;
					}
					priorityDisplay[next].rect=new Rect((float)(x*(1.0/rows)),(float)((rows-y-1)*(1.0/rows)),(float)(1.0/rows),(float)(1.0/rows));
					priorityDisplay[next].depth=1;
					currentDisplay[rows*y+x].depth=0;
					currentDisplay[rows*y+x]=priorityDisplay[next];
					next++;
				}
			x++;
			}
		}
		
		/*Debug.Log (1);
		for(int i=0; i<rows;i++){
			for(int j=0; j<rows;j++){*/
				/*if(((i*3)+j)<priorityDisplay.Count){
					priorityDisplay[i*3+j].rect= new Rect(i*dimen,j*dimen,dimen,dimen);
				}
				if(){
			}*/
			/*}
		}*/
		
		//i++;
		//j++;
	
	}
}
public class CameraSorter  : IComparer{
	int IComparer.Compare(object a, object b)
   	{
		Camera c1=(Camera)a;
      	Camera c2=(Camera)b;
      	if (c1.GetComponent<cameraTurn>().suspicion > c2.GetComponent<cameraTurn>().suspicion)
         	return 1;
      	if (c1.GetComponent<cameraTurn>().suspicion > c2.GetComponent<cameraTurn>().suspicion)
        	return -1;
      	else
        	return 0;
   }
	
}
