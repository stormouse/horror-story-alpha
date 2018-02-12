using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeArea : MonoBehaviour {

    List<GameObject> playerInside = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!playerInside.Contains(other.gameObject))
            {
                playerInside.Add(other.gameObject);
                LevelManager.Singleton.PlayerEscape();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (playerInside.Contains(other.gameObject))
            {
                playerInside.Remove(other.gameObject);
                LevelManager.Singleton.PlayerNotEscape();
            }

        }
    }
}
