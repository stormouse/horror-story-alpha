using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(sendInterval = 0)]
public class SurvivorSkills : NetworkBehaviour {
	// properties
	public string m_InteractionButtonName = "Interaction";
	public float moveSpeed = 5f;
	public float attackRange = 5.0f;
	public float attackAngle = 90.0f;
	public float deployAnimationLength = 2f;
	public bool hasHook = true;

	// components
	public GameObject trapPrefab;
	public Transform trapSpawn;
	private Rigidbody m_rigidbody;
	private NetworkCharacter character;

	// private
	bool m_Charging = false;
	PowerSourceController m_InteractingPowerSource;

	private GameObject trap;

	private float angle = 0f;
	private int freshCounter = 0;


	#region Builtin_Functions
	void Start()
	{
		SetupComponents();
		AttachSkillsToCharacter();
	}


	void Update()
	{
		ChargePowerSource (); //Maybe a little problem about server side?
		if (isLocalPlayer)
		{
			ReceivePlayerControl();
		}
	}
	#endregion


	#region Setup_Skills
	void SetupComponents()
	{
		m_rigidbody = GetComponent<Rigidbody>();
		character = GetComponent<NetworkCharacter>();
	}

	void AttachSkillsToCharacter()
	{
		character.Register(CharacterState.Normal, "Deploy", DeployMethod);
		character.Register(CharacterState.Normal, "Charge", ChargeMethod);
	}
	#endregion


	void ReceivePlayerControl()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			character.Perform("Deploy", gameObject, null);
		}
		/*
		if (Input.GetButton (m_InteractionButtonName)) {  //Logic problem!!
			character.Perform ("Charge", gameObject, null);
		} else {
			m_Charging = false;
		}
		*/
	}
	#region DeployTrap
	void DeployMethod(GameObject sender, ActionArgument args) {
		CmdDeploy ();
	}
	[Command]
	void CmdDeploy() {
		RpcDeploy ();
		_DeployMethodServer ();
		//_DeployMethod ();
	}

	[ClientRpc]
	void RpcDeploy() {
		_DeployMethod (); 
	}
	void _DeployMethodServer() {
		
		trap = Instantiate (trapPrefab);
		trap.transform.position = trapSpawn.position;
		NetworkServer.Spawn (trap);
	}
	void _DeployMethod() {
		character.Perform("StopMovement", gameObject, null);
		// TODO: limit transition command from outside!
		character.Animator.SetTrigger ("Deploy");
		character.Transit(CharacterState.Casting);
		character.SwitchCoroutine (StartCoroutine(_DeployAnimationDelay()));
	}
	IEnumerator _DeployAnimationDelay() {
		float startTime = Time.time;
		while (true)
		{
			float now = Time.time;
			if (now - startTime < deployAnimationLength)
			{
				yield return new WaitForEndOfFrame();
			}
			else break;
		}
		character.SwitchCoroutine(null);
		// do not directly transit: Transit(CharacterState.Normal); 
		// be ready for counter-skill that can stun hunters while they attack
		character.Perform("EndCasting", gameObject, null);
	}
	#endregion

	#region ChargePowerSource
	void ChargePowerSource() {
		if (m_Charging && m_InteractingPowerSource != null)
		{
			m_InteractingPowerSource.Charge();
			character.Perform("StopMovement", gameObject, null);
			// TODO: limit transition command from outside!
			character.Transit(CharacterState.Casting);
			character.Animator.SetTrigger ("Charge");

		}
	}
	void ChargeMethod(GameObject sender, ActionArgument args) {
		bool originalCharging = m_Charging;
		if (m_InteractingPowerSource != null) {
			m_Charging = true;
		} else {
			m_Charging = false;
		}
		if (originalCharging != m_Charging) {
			CmdCharge (m_Charging);
		}
	}
	[Command]
	void CmdCharge(bool isCharging)
	{
		m_Charging = isCharging;
	}
	void OnTriggerEnter(Collider collider)
	{
		PowerSourceController psc = collider.GetComponent<PowerSourceController>();
		if (psc != null)
			m_InteractingPowerSource = psc;
	}

	void OnTriggerExit(Collider collider)
	{
		PowerSourceController psc = collider.GetComponent<PowerSourceController>();
		if (psc != null)
			m_InteractingPowerSource = null;
	}
	#endregion
}
