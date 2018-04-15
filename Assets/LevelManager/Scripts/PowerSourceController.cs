using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PowerSourceController : NetworkBehaviour
{
	public float m_MaxPower = 100f;
	public float m_StartingPower = 0f; 
	public float m_PowerLeakingSpead = 1f;
	public float m_PowerLeakingDelay = 30f;
    public float m_PowerSpeed = 2f;
    public float m_PowerAlertTurnOnDelay = 10f;
    public float m_PowerAlertTurnOffDelay = 5f;

    public GameObject m_DestructableVersion;
    public Renderer m_PowersourceMeshRenderer;

    public Material m_PowersourceSeenThroughWallsMaterial;
    public Material m_PowersourceChargingSeenThroughWallsMaterial;
    public Material m_PowersourceNormalMaterial;

    public CanvasGroup canvasGroup;
	public Slider m_Slider;                            
	public Image m_FillImage;                          
	public Color m_FullPowerColor = Color.green;       
	public Color m_ZeroPowerColor = Color.red;    
	[SyncVar]
	private float m_CurrentPower;
    [SyncVar]
    public float m_chargingAge = 0f;
    [SyncVar]
	private float m_age = 0;

    public bool Charged
    {
        get { return m_Charged; }
    }
    private bool m_Charged = false;
    private bool m_Charging = false;



    private bool IsInWolfView()
    {
        foreach (var p in FindObjectsOfType<NetworkCharacter>())
            if (p.isLocalPlayer)
                if (p.Team == GameEnum.TeamType.Hunter)
                    return true;
                else
                    return false;
        return false;
    }

    private void Start()
	{
		m_CurrentPower = m_StartingPower;
		m_Charged = false;
        m_Charging = false;
		m_age = 0f;

        m_PowersourceMeshRenderer.material = IsInWolfView() ? m_PowersourceSeenThroughWallsMaterial : m_PowersourceNormalMaterial;

        SetPowerUI();
	}

	void Update() {
        if (m_Charged) {
            return;
        }
        if (m_Charging) {
            
           
            if (m_CurrentPower < m_MaxPower)
            {
                m_CurrentPower += Time.deltaTime * m_PowerSpeed;
            }


            if (m_CurrentPower >= m_MaxPower && !m_Charged)
            {
                m_CurrentPower = m_MaxPower;
                OnCharged();
            }

        }
		m_age += Time.deltaTime;
        if (m_age >0.2f)
        {
            m_Charging = false;
        }

        if (m_age < m_PowerAlertTurnOnDelay)
        {
            m_chargingAge += Time.deltaTime;
        } else
        {
            m_chargingAge = 0;
        }

        if (m_age > m_PowerAlertTurnOffDelay && !m_Charged)
        {
            // only wolf
            m_PowersourceMeshRenderer.material = IsInWolfView() ? m_PowersourceSeenThroughWallsMaterial : m_PowersourceNormalMaterial;
        }

		if (m_CurrentPower > 0f && m_age > m_PowerLeakingDelay && !m_Charged) {
			m_CurrentPower -= m_PowerLeakingSpead * Time.deltaTime;
			if (m_CurrentPower < 0f) {
				m_CurrentPower = 0f;
			}
		}
		SetPowerUI ();
	}

	void FixedUpdate() {
		
	}


	private void SetPowerUI ()
	{
		m_Slider.value = m_CurrentPower;
		m_FillImage.color = Color.Lerp (m_ZeroPowerColor, m_FullPowerColor, m_CurrentPower / m_MaxPower);
        if (m_Slider.value <= 0f)
        {
            canvasGroup.alpha = 0f;
        } else
        {
            canvasGroup.alpha = 255f;
        }
	}


    [ServerCallback]
	private void OnCharged ()
	{
		m_Charged = true;

        //gameObject.SetActive (false);
        RpcDeactivate();

        gameObject.SetActive(false);
        GetComponent<Collider>().enabled = false;
        gameObject.tag = "Untagged";
    }

    [ClientRpc]
    void RpcDeactivate()
    {
        m_Charged = true;
        gameObject.SetActive(false);
        GetComponent<Collider>().enabled = false;
        gameObject.tag = "Untagged";
        CreateDestruction();
        if (PlayerUIManager.singleton)
        {
            PlayerUIManager.singleton.DecreaseBatteryCount();
        }
    }

    void CreateDestruction()
    {
        Instantiate(m_DestructableVersion, transform.position, transform.rotation);
    }



	public void Charge ()
	{
        m_age = 0f;
        m_Charging = true;

        if (m_Charging && m_chargingAge > m_PowerAlertTurnOnDelay)
        {
            // wolf only
            m_PowersourceMeshRenderer.material = IsInWolfView() ? m_PowersourceChargingSeenThroughWallsMaterial : m_PowersourceNormalMaterial;
        }
		

		SetPowerUI ();
	}
}

