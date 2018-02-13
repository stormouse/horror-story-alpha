using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ViewControl : NetworkBehaviour {
	// Use this for initialization
	void Start () {
		
		if (isLocalPlayer) {
			GetComponent<MeshRenderer> ().material.shader = Shader.Find ("Standard");
		} else {
			if (!isServer) {
				GetComponent<FieldOfViewNearby> ().enabled = false;
				if (GetComponent<FieldOfView> () != null) {
					GetComponent<FieldOfView> ().enabled = false;
				}
			}
		}
	}
}
