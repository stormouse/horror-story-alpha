using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PowerInteraction : NetworkBehaviour
{

    public string m_InteractionButtonName = "Interaction";

    bool m_Charging = false;

    PowerSourceController m_InteractingPowerSource;



    // Use this for initialization
    void Awake()
    {
        m_Charging = false;
    }

    // Update is called once per frame (100 calls/sec)
    void Update()
    {


        if (m_Charging && m_InteractingPowerSource != null)
        {

            m_InteractingPowerSource.Charge();
			GetComponent<Animator> ().SetTrigger ("Charge");//zx_change for Animator
            // CmdCharge (m_CurrentChargingSpeed * Time.deltaTime);
        }

        if (!isLocalPlayer)
        {
            return;
        }

        var originalCharging = m_Charging;
        if (Input.GetButton(m_InteractionButtonName) && m_InteractingPowerSource != null)
        {
            m_Charging = true;
        }
        else
        {
            m_Charging = false;
        }

        if (originalCharging != m_Charging)
            CmdCharge(m_Charging);


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
}
