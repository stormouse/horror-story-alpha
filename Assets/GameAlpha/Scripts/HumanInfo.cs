using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanInfo : MonoBehaviour {
	private bool loseControl;
	void Start() {
		loseControl = false;
	}
	public void SetLoseControl(bool val) {
		loseControl = val;
	}
	public bool GetLoseControl() {
		return loseControl;
	}
}
