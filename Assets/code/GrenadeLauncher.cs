using UnityEngine;
using System.Collections;

public class GrenadeLauncher : MonoBehaviour {
	
	public Rigidbody grenade;
	public Transform angle;
	public Collider playerCollider;
	public GUIText text;
	
	private bool equipped = false;
	private bool availableGrenade = true;
	private float reload;
	

	
	
	void throwGrenade(){
		if(availableGrenade){
			float speed = 20.0f;
			
			Rigidbody instantiatedProjectile = (Rigidbody) Network.Instantiate(grenade, transform.position, transform.rotation, 0);
			
			float vertspeed = speed * -Mathf.Sin (angle.eulerAngles.x * 0.0174f);
			speed *= Mathf.Cos(angle.eulerAngles.x * 0.0174f);
					
			instantiatedProjectile.velocity = transform.TransformDirection(new Vector3(0, vertspeed + 10, speed));
			
			availableGrenade = false;
			reload = 3f;
		}
	}
	
	
	
	
	
	
	
	// Use this for initialization
	void Start () {
		text = GameObject.FindGameObjectWithTag("WeaponText").guiText;
	}
	
	// Update is called once per frame
	void Update () {
		
		if(reload > 0){
			reload -= Time.deltaTime;
			return;
		}else{
			availableGrenade = true;
		}
		
		if (equipped) {
			
			if (Input.GetButtonDown("Fire1")) {
				throwGrenade();
			}
		}
		
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			if (equipped) {
				equipped = false;
				text.text = "None";
			} else {
				equipped = true;
				text.text = "Grenade";			
			}
		}
	}
}