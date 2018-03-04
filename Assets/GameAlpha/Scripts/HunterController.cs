using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class HunterController : NetworkBehaviour {

	HumanCharacter m_Player;
	HunterInfo info;
	public GameObject hook;
	public Transform hookSpawn;
	public float hookSpeed = 10f;
	HunterSkill m_Skill;
	

	// Use this for initialization
	void Start () {
		m_Player = GetComponent<HumanCharacter> ();
		info = GetComponent<HunterInfo> ();
		m_Skill = GetComponent<HunterSkill> (); 
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space) && isLocalPlayer) {
			//Vector3 mouseP = Camera.main.ScreenToWorldPoint (new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));

			//m_Skill.Skill (1, new Vector3(mouseP.x - transform.position.x, 0f,mouseP.z - transform.position.z).normalized);
			m_Skill.Skill(1, transform.forward);
		}
	}
	void FixedUpdate() {
		if (!isLocalPlayer) {
			return;
		}
		//if (info.GetHasHook()) {
			float h = Input.GetAxis ("Horizontal");
			float v = Input.GetAxis ("Vertical");
			Vector3 move = h * Vector3.right + v * Vector3.forward;
			//m_Player.Move (move);
			CmdMove (move);
		//}
	}
	[Command]
	void CmdMove(Vector3 move) {
		if (info.GetHasHook ()) {
			RpcMove (move);
		}
	}
	[ClientRpc]
	void RpcMove(Vector3 move) {
		m_Player.Move (move);
	}
	/*
	[Command]
	void CmdHook(){
		if (!info.GetHasHook()) {
			return;
		}
		GameObject hooks = Instantiate (hook);
		hooks.transform.position = hookSpawn.position;
		hooks.GetComponent<Rigidbody> ().velocity = transform.forward * hookSpeed;
		hooks.GetComponent<HookSys> ().hunter = this.gameObject;
		info.SetHasHook (false);
		NetworkServer.Spawn (hooks);
	}
	*/
}
