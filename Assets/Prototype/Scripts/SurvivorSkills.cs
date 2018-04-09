using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(sendInterval = 0)]
public class SurvivorSkills : NetworkBehaviour {
	
    // properties
	public string m_InteractionButtonName = "Interaction";

    // skills

    public float flashCooldown = 6.0f;
    public int flashCount = 3;
    private float lastFlashTime;
    public bool FlashReady { get { return Time.time - lastFlashTime > flashCooldown && flashCount > 0; } }

    public float deployAnimationLength = 2f;
    public float trapCooldown = 5.0f;
    public int trapCount = 2;
    private float lastTrapTime;
    public bool TrapReady { get { return Time.time - lastTrapTime > trapCooldown && trapCount > 0; } }

    public float camouflageAnimationLength = 1.0f;
    public float camouflageCooldown = 20.0f;
    public int camouflageCount = 2;
    private float lastCamouflageTime;
    public bool CamouflageReady { get { return Time.time - lastCamouflageTime > camouflageCooldown && camouflageCount > 0; } }

    public int toyCarCount = 1;
    public bool ToyCarReady { get { return toyCarCount > 0; } }


    // components
    public GameObject trapPrefab;
	public Transform trapSpawn;

    private Rigidbody m_rigidbody;
	private NetworkCharacter character;
    private CameraFollow cameraFx;
    


	[HideInInspector] public bool m_Charging = false;
	[HideInInspector] public PowerSourceController m_InteractingPowerSource;

	private GameObject trap;

	private float angle = 0f;
	private int freshCounter = 0;
    private bool lookingback = false;


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
        cameraFx = GetComponent<CameraFollow>();
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
			DeployPerform ();
		}

		if (Input.GetButton (m_InteractionButtonName)) {  //Logic problem!!
			ChargePerform();
		} else {
			m_Charging = false;
		}

        // look back
        if (Input.GetMouseButton(1))
        {
            if (!lookingback)
            {
                if (cameraFx != null)
                {
                    cameraFx.ActiveLookbackPerspective();
                }
                lookingback = true;
            }
        }
        else
        {
            if (lookingback)
            {
                cameraFx.Deactivate();
                lookingback = false;
            }
        }
            

	}

	public void DeployPerform() {
        if (TrapReady)
        {
            character.Perform("Deploy", gameObject, null);
        }
	}
	public void ChargePerform() {
		character.Perform ("Charge", gameObject, null);
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
		_DeployMethodClient (); 
	}
	void _DeployMethodServer() {
		trap = Instantiate (trapPrefab);
		trap.transform.position = trapSpawn.position;
		NetworkServer.Spawn (trap);
	}
	void _DeployMethodClient() {
		character.Perform("StopMovement", gameObject, null);
		character.Animator.SetTrigger ("Deploy");
		character.Transit(CharacterState.Casting);
		character.SwitchCoroutine (StartCoroutine(_DeployAnimationDelay()));

        lastTrapTime = Time.time;
        trapCount -= 1;
        if (PlayerUIManager.singleton != null)
        {
            PlayerUIManager.singleton.UpdateItemCount(1, trapCount);
            if (trapCount > 0)
            {
                PlayerUIManager.singleton.EnterCooldown(1, trapCooldown);
            }
        }
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
			//character.Perform("StopMovement", gameObject, null);
			// TODO: limit transition command from outside!
			//character.Transit(CharacterState.Casting);
			character.Animator.SetTrigger ("Charge");

		}
		m_Charging = false;
	
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
		RpcCharge (isCharging);
	}
	[ClientRpc]
	void RpcCharge(bool isCharging){
		if (!isLocalPlayer) {
			m_Charging = isCharging;
		}
	}
	void OnTriggerEnter(Collider collider)
	{
		PowerSourceController psc = collider.GetComponent<PowerSourceController>();
		if (psc != null)
			//Debug.Log (psc);
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
