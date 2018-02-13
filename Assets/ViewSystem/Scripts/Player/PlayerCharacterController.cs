using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PlayerCharacterController : NetworkBehaviour {

	PlayerCharacter m_Player;
	// Use this for initialization
	void Start () {
		m_Player = GetComponent<PlayerCharacter> ();
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
		m_Player.Move (move);
		CmdMove (move);

	}
	[Command]
	void CmdMove(Vector3 move) {
		RpcMove (move);
	}
	[ClientRpc]
	void RpcMove(Vector3 move) {
		if (!isLocalPlayer) {
			m_Player.Move (move);
		}
	}
}
