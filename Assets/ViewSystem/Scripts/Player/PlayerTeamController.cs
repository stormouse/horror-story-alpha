using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PlayerTeamController : NetworkBehaviour {

	// Use this for initialization
	void Start () {
		//Simple color change

		if (isLocalPlayer) {
			GetComponent<MeshRenderer> ().material.color = Color.green;
			this.transform.Find ("MinimapPlayer").GetComponent<MeshRenderer> ().material.color = Color.green;
		} else {
			
			GetComponent<MeshRenderer> ().material.color = Color.red;
			this.transform.Find ("MinimapPlayer").GetComponent<MeshRenderer> ().material.color = Color.red;
		}
	}

}
