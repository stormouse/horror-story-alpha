using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class HunterSkill : NetworkBehaviour {
	Animator anim;
	HunterInfo info;
	public GameObject hook;
	public Transform hookSpawn;
	public float hookSpeed = 10f;
	void Start() {
		anim = GetComponent<Animator> ();
		info = GetComponent<HunterInfo> ();
	}
	public void Skill(int skill, Vector3 dir) {
		switch (skill) {
		case 1: 
			CmdHookSkill (dir);
			break;
		default:
			break;
		}
	}
	[Command]
	void CmdHookSkill(Vector3 dir) {
		if (!info.GetHasHook ()) {
			return;
		}
		anim.SetTrigger ("Hook");
		//RpcHookAnimatorUpdate ();
		GameObject hooks = Instantiate (hook);
		hooks.transform.position = hookSpawn.position;
		hooks.GetComponent<Rigidbody> ().velocity = dir * hookSpeed;
		hooks.GetComponent<HookSys> ().hunter = this.gameObject;
		info.SetHasHook (false);
		NetworkServer.Spawn (hooks);
	}
	/*
	[ClientRpc]
	void RpcHookAnimatorUpdate() {
	}
	*/
}
