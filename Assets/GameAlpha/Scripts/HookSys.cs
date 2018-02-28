using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class HookSys : MonoBehaviour {

	public GameObject hunter;
	public string target = "Human";
	private Rigidbody m_Rig;
	private float m_hookRange = 10f; 
	public float backSpeed = 10f;
	void Start() {
		m_Rig = GetComponent<Rigidbody> ();
	}
	void Update() {
		if ((transform.position - hunter.transform.position).magnitude > m_hookRange) {
			Vector3 dir = new Vector3 (hunter.transform.position.x - transform.position.x, 0, hunter.transform.position.z - transform.position.z).normalized;
			m_Rig.velocity = dir * backSpeed;
		}
	}
	void OnCollisionEnter(Collision collision) {
		GameObject hit = collision.gameObject;
		Vector3 dir = new Vector3(hunter.transform.position.x - hit.transform.position.x, 0,hunter.transform.position.z - hit.transform.position.z).normalized;
		if (hit == hunter) {
			hit.GetComponent<HunterInfo>().SetHasHook(true);
			Destroy(this.gameObject);
		}

		if (hit.tag.Equals (target)) {
			hit.GetComponent<HumanInteract> ().isHooked (dir * backSpeed);
			Destroy (this.gameObject);
		}
		m_Rig.velocity = dir * backSpeed;
	}
}
