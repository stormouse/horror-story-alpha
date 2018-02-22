using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Hunter : NetworkBehaviour {

	//Move Spped
	public float moveSpeed = 5f;
	public float hookRange = 20f;
	public float hookSpeed = 50f;
	public bool hasHook = true;

	//private
	private GameObject hook;
	private GameObject hooks;
	private float angle = 0f;
	private int freshCounter = 0;
	private float yDiff = 0.8f;
	private float mDiff = 0.8f;
	//animation
	private Animation anima;

	// Use this for initialization
	void Start () {
		hook = Resources.Load ("Hook_prefab") as GameObject;
		anima = GetComponent<Animation> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate(){
		if (isLocalPlayer) {
			if ((Input.GetKeyUp (KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) && (hasHook))  {
				CmdAnimation("Standby");
			}
			if (Input.GetKey (KeyCode.W) && hasHook) {
				CmdMove (transform.position.x, transform.position.y, transform.position.z + moveSpeed * Time.deltaTime, 0f);
				CmdAnimation("run");
			}
			if (Input.GetKey (KeyCode.S) && hasHook) {
				CmdMove (transform.position.x, transform.position.y, transform.position.z - moveSpeed * Time.deltaTime, 180f);
				CmdAnimation("run");
			}
			if (Input.GetKey (KeyCode.D) && hasHook) {
				CmdMove (transform.position.x + moveSpeed * Time.deltaTime, transform.position.y, transform.position.z, 90f);
				CmdAnimation("run");
			}
			if (Input.GetKey (KeyCode.A) && hasHook) {
				CmdMove (transform.position.x - moveSpeed * Time.deltaTime, transform.position.y, transform.position.z, 270f);
				CmdAnimation("run");
			}
			if (Input.GetKey (KeyCode.Space) && hasHook) {
				CmdHook ();
			}
			//Test skills
			if (Input.GetKey (KeyCode.Q)) {
				CmdAnimation("Attack1");
			}
			if (Input.GetKey (KeyCode.Y)) {
				CmdAnimation("Attack2");
			}
			if (Input.GetKey (KeyCode.E)) {
				CmdAnimation("Attack3");
			}
			if (Input.GetKey (KeyCode.R)) {
				CmdAnimation("Attack20");
			}
			if (Input.GetKey (KeyCode.T)) {
				CmdAnimation("Roar");
			}
			if (Input.GetKey (KeyCode.F)) {
				CmdAnimation("Death");
			}
		}
		if (freshCounter > 50) {
			CmdUpdate (transform.position);
			freshCounter = 0;
		} else {
			freshCounter++;
		}
	}

	[Command]
	void CmdAnimation(string s){
		anima.CrossFade (s);
		RpcAnimation (s);
	}

	[Command]
	void CmdUpdate(Vector3 position){
		transform.position = position;
		RpcUpdate (position);
	}

	[Command]
	void CmdMove(float x, float y, float z, float rotation){
		transform.position = new Vector3 (x, y, z);
		transform.rotation = Quaternion.Euler (0f, rotation, 0f);
		angle = rotation;
		RpcMove (x, y, z, rotation);
	}

	[Command]
	void CmdHook(){
		if (!hasHook) {
			return;
		}
		hooks = Instantiate (hook);
		hooks.transform.rotation = Quaternion.Euler (180f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
		hooks.GetComponent<Hook> ().hunter = this.gameObject;
		if (angle == 0f)
			hooks.transform.position = new Vector3 (transform.position.x, transform.position.y + yDiff, transform.position.z + mDiff);
		else if (angle == 90f)
			hooks.transform.position = new Vector3 (transform.position.x + mDiff, transform.position.y + yDiff, transform.position.z);
		else if (angle == 180f)
			hooks.transform.position = new Vector3 (transform.position.x, transform.position.y + yDiff, transform.position.z - mDiff);
		else if (angle == 270f)
			hooks.transform.position = new Vector3 (transform.position.x - mDiff, transform.position.y + yDiff, transform.position.z);
		hasHook = false;
		RpcHook ();
	}

	[ClientRpc]
	void RpcAnimation(string s){
		anima.CrossFade (s);
	}

	[ClientRpc]
	void RpcUpdate(Vector3 position){
		transform.position = position;
	}

	[ClientRpc]
	void RpcHook(){
		if (!hasHook) {
			return;
		}
		hooks = Instantiate (hook);
		hooks.transform.rotation = Quaternion.Euler (180f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
		hooks.GetComponent<Hook> ().hunter = this.gameObject;
		if (angle == 0f)
			hooks.transform.position = new Vector3 (transform.position.x, transform.position.y + yDiff, transform.position.z + mDiff);
		else if (angle == 90f)
			hooks.transform.position = new Vector3 (transform.position.x + mDiff, transform.position.y + yDiff, transform.position.z);
		else if (angle == 180f)
			hooks.transform.position = new Vector3 (transform.position.x, transform.position.y + yDiff, transform.position.z - mDiff);
		else if (angle == 270f)
			hooks.transform.position = new Vector3 (transform.position.x - mDiff, transform.position.y + yDiff, transform.position.z);
		hasHook = false;
	}

	[ClientRpc]
	void RpcMove(float x, float y, float z, float rotation){
		transform.rotation = Quaternion.Euler (0f, rotation, 0f);
		transform.position = new Vector3 (x, y, z);
		angle = rotation;
	}
}
