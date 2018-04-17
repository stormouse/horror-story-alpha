using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeArea : MonoBehaviour {

    public GameObject m_Boat;
    public GameObject m_Bridge;
    public GameObject m_Stone;
    public GameObject m_Cave;

    public int m_NumberOfPowerSourceToOpen;

    List<GameObject> playerInside = new List<GameObject>();
    bool bActivated = false;


    public void Activate()
    {
        bActivated = true;
        if (m_Bridge)
        {
            m_Bridge.GetComponent<MeshRenderer>().enabled = true;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!bActivated) return;

        if (other.tag == "Player")
        {
            if (!playerInside.Contains(other.gameObject))
            {
                playerInside.Add(other.gameObject);
                var character = other.GetComponent<NetworkCharacter>();
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
