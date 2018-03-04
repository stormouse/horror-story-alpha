using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class TrapControl : NetworkBehaviour {

	public float trapStunTime = 3.0f;

	void OnTriggerEnter(Collider other) {
		if (!isServer) {
			return;
		}
		var otherCharacter = other.GetComponent<NetworkCharacter>();
		if (otherCharacter && otherCharacter.Team == GameEnum.TeamType.Hunter) {
			var args = new StunArgument ();
			args.time = trapStunTime;
			otherCharacter.Perform ("Stun", other.gameObject, args);
			NetworkServer.Destroy (gameObject);
		}

	}
}
