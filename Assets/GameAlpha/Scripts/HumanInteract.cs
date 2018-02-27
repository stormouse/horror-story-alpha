using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanInteract : MonoBehaviour {
	private Rigidbody m_Rig;
	public string hunterTag = "Hunter";
	void Start() {
		m_Rig = GetComponent<Rigidbody> ();
	}
	public void isHooked(Vector3 v) {
		m_Rig.GetComponent<HumanInfo> ().SetLoseControl (true);
		m_Rig.useGravity = false;
		m_Rig.velocity = v;

	}
	void  OnCollisionEnter(Collision collision) {
		GameObject hit = collision.gameObject;
		if (hit.tag.Equals (hunterTag)) {
			hit.GetComponent<HunterInfo> ().SetHasHook (true);
			Destroy (this.gameObject);
		}
	}
}
