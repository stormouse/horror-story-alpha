using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeArea : MonoBehaviour {


    public Transform boat;

    List<GameObject> playerInside = new List<GameObject>();
    bool bActivated = false;


    public void Activate()
    {
        bActivated = true;
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
                character.MoveTo(boat.position, MoveMethod.Teleport);
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
