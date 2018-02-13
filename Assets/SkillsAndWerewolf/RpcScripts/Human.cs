using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Human : NetworkBehaviour {

	public bool hooked = false;
	public float force = 30f;
	public bool alive = true;

	private string hookTag = "Hook";
	private string hunterTag = "Hunter";
	private Vector3 fwd;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		CmdMove (transform.position);
	}

	[Command]
	void CmdMove(Vector3 position){
		transform.position = position;
		RpcMove (position);
	}

	[ClientRpc]
	void RpcMove(Vector3 position){
		transform.position = position;
	}

	void OnCollisionEnter(Collision collision){
		if (collision.collider.tag == hookTag && collision.collider.gameObject.GetComponent<Hook>().hunter != this.gameObject) {
			hooked = true;
			fwd = transform.TransformDirection (collision.collider.gameObject.GetComponent<Hook>().hunter.transform.position - this.transform.position);
			GetComponent<Rigidbody> ().velocity = fwd.normalized * force;
		}
		if (collision.collider.tag == hunterTag) {
			hooked = false;
			alive = false;
			GetComponent<Rigidbody> ().velocity = new Vector3 (0f, 0f, 0f);
			collision.collider.gameObject.GetComponent<Rigidbody> ().velocity = new Vector3 (0f, 0f, 0f);
		}
	}
}
