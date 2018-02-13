using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour {

	//private
	private float x, z;
	private Vector3 fwd;
	private float hookSpeed = 30f;
	private float hookRange = 20f;
	private string hunterTag = "Hunter";

	//public hunter
	public GameObject hunter;

	// Use this for initialization
	void Start () {
		//Get born position and move direction
		x = transform.position.x;
		z = transform.position.z;
		hookSpeed = hunter.GetComponent<Hunter> ().hookSpeed;
		hookRange = hunter.GetComponent<Hunter> ().hookRange;
		fwd = transform.TransformDirection (Vector3.forward);
		GetComponent<Rigidbody> ().velocity = fwd * hookSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		if ((transform.position.z - z)*(transform.position.z - z) + (transform.position.x - x) * (transform.position.x - x) 
			> hookRange * hookRange) {
			//钩子消失，重回hunter手中，摧毁本钩子
			hunter.GetComponent<Hunter>().hasHook = true;
			Destroy (this.gameObject);
		}
	}

	void OnCollisionEnter(Collision collision){
		if (collision.collider.tag == hunterTag) {
			collision.collider.gameObject.GetComponent<Rigidbody> ().velocity = new Vector3 (0f, 0f, 0f);
		}
		hunter.GetComponent<Hunter>().hasHook = true;
		Destroy (this.gameObject);
	}
}
