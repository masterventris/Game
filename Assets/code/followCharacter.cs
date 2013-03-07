using UnityEngine;

public class followCharacter : MonoBehaviour{
	public Transform Target;

	void LateUpdate(){
        transform.position = new Vector3(Target.position.x, transform.position.y, Target.position.z);
    }
}