using UnityEngine;
using System.Collections;

public class equipment : MonoBehaviour {
	
	public Rigidbody grenadeMesh;
	public Collider playerCollider;
	float grenadeSpeed = 20f;
	public GUIText equippedWeapon;
	public Transform angle;
	
	bool equipped;
	
	

	// Use this for initialization
	void Start () {
		equipped = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Alpha1)){
	  		if(equipped){
		  		equipped = false;
		  		equippedWeapon.text = "None";
		  	}else{
		  		equipped = true;
		  		equippedWeapon.text = "Grenade";
	  		}
  		}
		
		if(equipped){
			if( Input.GetButtonDown("Fire1")){
				fireGrenade();
			}
		}
		
	}
	
	void fireGrenade(){
		Rigidbody newGrenade = (Rigidbody)Instantiate(grenadeMesh, transform.position, transform.rotation);
		float vertspeed = grenadeSpeed * -Mathf.Sin((float)angle.eulerAngles.x*0.0174f);
		float speed = grenadeSpeed * Mathf.Cos((float)angle.eulerAngles.x*0.0174f);
		
		newGrenade.velocity = transform.TransformDirection(new Vector3(0f, vertspeed+10f, speed));
		
		//Physics.IgnoreCollision( newGrenade.collider, playerCollider);
	}
}
