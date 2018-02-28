using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterInfo : MonoBehaviour {
	private bool hasHook;
	void Start() {
		hasHook = true;
	}
	public void SetHasHook(bool val) {
		hasHook = val;
		if (val) {
			GetComponent<Rigidbody> ().isKinematic = false;
		} else {
			GetComponent<Rigidbody> ().isKinematic = true;
		}
	}
	public bool GetHasHook() {
		return hasHook;
	}
}
