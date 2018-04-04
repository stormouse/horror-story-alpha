using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class TrapControl : NetworkBehaviour {

	public float trapStunTime = 3.0f;
    private bool triggered = false;

    private void Start()
    {
        
    }

    void OnTriggerEnter(Collider other) {
		if (!isServer || triggered) {
			return;
		}
		var otherCharacter = other.GetComponent<NetworkCharacter>();
		if (otherCharacter && otherCharacter.Team == GameEnum.TeamType.Hunter) {
            triggered = true;
			var args = new StunArgument ();
			args.time = trapStunTime;
            otherCharacter.Perform("StopMovement", other.gameObject, null);
            otherCharacter.Perform ("Stun", other.gameObject, args);
            

            GetComponent<Animator> ().SetBool("close", true);


            Invoke("Byebye", trapStunTime);
            //NetworkServer.Destroy (gameObject);
		}

	}

    void Byebye() {
        NetworkServer.Destroy(gameObject);
    }
}
