using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MinimapController : MonoBehaviour {

    public Transform player;

    private void Start()
    {
        transform.parent = null;
    }

    private void LateUpdate()
    {
        if (player && player.GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            Vector3 newPos = player.position;
            newPos.y = transform.position.y;
            transform.position = newPos;

            transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

}
