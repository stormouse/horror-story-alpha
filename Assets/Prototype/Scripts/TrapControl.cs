using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class TrapControl : NetworkBehaviour {

	public float trapStunTime = 3.0f;
    private bool triggered = false;
    public float m_StartSprintForceFactor = 10.0f;

    public AudioSource trapActivateAudio;

    private void Start()
    {
        //GetComponent<Rigidbody>().AddForce(transform.TransformDirection(Vector3.back * m_StartSprintForceFactor));
    }

    private void Update()
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
            otherCharacter.Perform("Stun", other.gameObject, args);
            

            GetComponent<Animator> ().SetBool("close", true);
            if (trapActivateAudio)
                trapActivateAudio.Play();

            RpcActivateTrap();

            Invoke("Byebye", trapStunTime);
            //NetworkServer.Destroy (gameObject);
		}
	}


    [ClientRpc]
    void RpcActivateTrap()
    {
        if (!isServer)
        {
            GetComponent<Animator>().SetBool("close", true);
            if (trapActivateAudio)
                trapActivateAudio.Play();
        }
    }


    void Byebye() {
        NetworkServer.Destroy(gameObject);
    }
}
