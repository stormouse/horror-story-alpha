using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class HumanController : NetworkBehaviour {
	
	HumanCharacter m_Player;
	HumanInfo info;
	// Use this for initialization
	void Start () {
		m_Player = GetComponent<HumanCharacter> ();
		info = GetComponent<HumanInfo> ();
	}

	// Update is called once per frame
	void Update () {

	}
	void FixedUpdate() {
		if (!isLocalPlayer) {
			return;
		}

		float h = Input.GetAxis ("Horizontal");
		float v = Input.GetAxis ("Vertical");
		Vector3 move = h * Vector3.right + v * Vector3.forward;
		//m_Player.Move (move);
		CmdMove (move);

	}
	[Command]
	void CmdMove(Vector3 move) {
		if (!info.GetLoseControl ()) {
			RpcMove (move);
		}
	}
	[ClientRpc]
	void RpcMove(Vector3 move) {
		m_Player.Move (move);
	}
}
