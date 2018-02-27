using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanCharacter : MonoBehaviour {

	[SerializeField] float m_MovingTurnSpeed = 360;
	[SerializeField] float m_StationaryTurnSpeed = 180;
	[SerializeField] float m_MoveSpeedMultiplier = 4.0f;
	Rigidbody m_Rigidbody;
	float m_TurnAmount;
	float m_ForwardAmount;
	//Animator Initialization
	Animator m_anim;
	int m_speedHash = Animator.StringToHash("Speed");
	// Use this for initialization
	void Start () {
		m_Rigidbody = GetComponent<Rigidbody> ();
		m_anim = GetComponent<Animator> ();
	}
	public void Move(Vector3 move) {
		if (move.magnitude > 1f)
			move.Normalize ();
		//Vector3 move0 = move;
		move = transform.InverseTransformDirection (move);

		m_TurnAmount = Mathf.Atan2 (move.x, move.z);
		m_ForwardAmount = move.z;
		ApplyExtraTurnRotation ();
		if (move.magnitude > 0f) {
			ApplyTranslation (move);
		}
		float animSpeed = move.magnitude;
		m_anim.SetFloat (m_speedHash, animSpeed);
	}
	void ApplyTranslation(Vector3 move) {
		//Vector3 v = m_MoveSpeedMultiplier * move;
		//m_Rigidbody.velocity = v; 
		transform.Translate (move * m_MoveSpeedMultiplier * Time.deltaTime);
	}
	void ApplyExtraTurnRotation()
	{
		// help the character turn faster (this is in addition to root rotation in the animation)
		float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
		transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
	}
}
