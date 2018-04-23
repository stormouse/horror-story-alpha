using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class TrapControl : NetworkBehaviour {

	public float trapStunTime = 3.0f;
    public float m_StartSprintForceFactor = 10.0f;

    public AudioSource trapActivateAudio;

    private bool triggered = false;

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
            otherCharacter.Perform("Stun", other.gameObject, args);

            ActivateTrap(otherCharacter.gameObject);
            RpcActivateTrap(otherCharacter.netId);

            Invoke("Byebye", trapStunTime);
		}
	}

    
    [ClientRpc]
    void RpcActivateTrap(NetworkInstanceId hitId)
    {
        if (!isServer)
        {
            var victim = LevelManager.Singleton.GetPlayerObjectByNetId(hitId.Value);
            ActivateTrap(victim);
        }
    }


    void ActivateTrap(GameObject victim)
    {
        GetComponent<Animator>().SetBool("close", true);
        if (trapActivateAudio)
            trapActivateAudio.Play();

        if (victim != null)
        {
            var snappables = victim.GetComponents<ISnappable>();
            for(int i = 0; i < snappables.Length; i++)
            {
                if(snappables[i].ParentName() == SnappableParts.Leg)
                {
                    transform.SetParent(snappables[i].ParentTransform());
                    transform.localPosition = Vector3.zero;
                }
            }
        }
    }


    void Byebye() {
        NetworkServer.Destroy(gameObject);
    }
}
