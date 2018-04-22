using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeArea : MonoBehaviour {

    public GameObject m_Boat;
    public GameObject m_Bridge;
    public GameObject m_Stone;
    public GameObject m_Cave;

    public Material m_CaveSeeThroughableMaterial;
    public Material m_BridgeSeeThroughableMaterial;
    public Material m_BoatSeeThroughableMaterial;

    public int m_NumberOfPowerSourceToOpen;

    List<GameObject> playerInside = new List<GameObject>();
    bool bActivated = false;


    public void Activate()
    {
        bActivated = true;
        if (m_Bridge)
        {
            m_Bridge.GetComponent<MeshRenderer>().enabled = true;
            m_Bridge.GetComponent<MeshRenderer>().material = m_BridgeSeeThroughableMaterial;
            m_Boat.GetComponent<MeshRenderer>().material = m_BoatSeeThroughableMaterial;
        }
        if (m_Stone)
        {
            m_Stone.GetComponent<MeshRenderer>().enabled = false;
            m_Cave.GetComponent<MeshRenderer>().material = m_CaveSeeThroughableMaterial;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!bActivated) return;
        var character = other.GetComponent<NetworkCharacter>();

        if (other.tag == "Player" && character != null && character.Team == GameEnum.TeamType.Hunter)
        {
            if (!playerInside.Contains(other.gameObject))
            {
                playerInside.Add(other.gameObject);
                if (m_Boat)
                    character.MoveTo(m_Boat.transform.position, MoveMethod.Teleport);
                LevelManager.Singleton.PlayerEscape();
            }
        }
    }


    //private void OnTriggerExit(Collider other)
    //{
    //    if (!bActivated) return;

    //    if (other.tag == "Player")
    //    {
    //        if (playerInside.Contains(other.gameObject))
    //        {
    //            playerInside.Remove(other.gameObject);
    //            LevelManager.Singleton.PlayerNotEscape();
    //        }

    //    }
    //}
}
