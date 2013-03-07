using UnityEngine;
using System.Collections;
public class cameraTurn : MonoBehaviour {
	public float target;
	public float left;
	public float right;
	public float cameraNo=0;
    public float speed = 20.0F;
	public float sum = 50.0F;
	private int threshold;
	public int suspicion =0;
	private float flashTime=0;
	private int prevSuspicion;
	private bool alarm=false;
	
	private bool started = false;
	// Use this for initialization
	float mod(float x, float m) {
    return (x%m + m)%m;
}
	void OnPreRender(){
		if (started) {
			flashTime+=Time.deltaTime;
			if(flashTime>0.5){
				if(suspicion>threshold){
					alarm=!alarm;
				}else{
					alarm=false;
				}
				flashTime=0;
			}
			GameObject.Find("AlarmLight(Clone)").light.enabled=alarm;
		}
	}
	void OnPostRender(){
		GameObject.Find("AlarmLight(Clone)").light.enabled=false;
	}
	
	public void setThreshold() {
		GameObject camCont = GameObject.FindGameObjectWithTag("CameraController");
		threshold=camCont.GetComponent<cameraController>().priorityThreshold;
		started = true;
	}
	
	void Start () {
		prevSuspicion=suspicion;
		left=transform.eulerAngles.y-50.0F;
		right=transform.eulerAngles.y+50.0F;
		if(Random.Range(0,2)==1){speed*=-1.0F; sum*=-1.0F;}
		flashTime=Random.Range(0,(float)0.5);
		//GameObject camCont = GameObject.FindGameObjectWithTag("CameraController");
		//threshold=camCont.GetComponent<cameraController>().priorityThreshold;
	}
	
	// Update is called once per frame

    void Update() {
		if(prevSuspicion!=suspicion){
			Debug.Log("suspicionUpdate");
			GameObject.Find ("Camera Controller").GetComponent<cameraController>().suspicionChanged(gameObject.camera);
			prevSuspicion=suspicion;
		}
		/*if (Input.GetButtonDown ("Jump")) {
        // choose the margin randomly
        var margin = Random.Range(0.0F, 0.3F);
        // setup the rectangle
        camera.rect =new Rect(margin, 0, 1 - margin * 2, 1);
    }*/
		//if(cameraNo==2){
		// Debug.Log("before"+sum);
	if(speed>0.0F){
			if(sum>100.0F){
				speed*=-1.0F;
				sum=0.0F;
				}
	}
		else{
			if(sum<-100.0F){
				speed*=-1.0F;
				sum=0.0F;
				}
		}
		Vector3 v=new Vector3(0,speed,0);
		sum+=(speed*Time.deltaTime);
		transform.Rotate(v*Time.deltaTime);
		Plane[] planes=GeometryUtility.CalculateFrustumPlanes(gameObject.camera);
		GameObject[] Terrorists=GameObject.FindGameObjectsWithTag("Terrorist");
		/*for(int i=0;i<Terrorists.Length;i++){
			if(GeometryUtility.TestPlanesAABB(planes,Terrorists[i].collider.bounds)){
				suspicion=8;
				//Debug.Log("SPHERE");
			}
			else{
			suspicion=0;	
			}
		}*/
	}
//}
}
