﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour {

    public Transform player;

    private void Start()
    {
        transform.parent = null;
    }

    private void LateUpdate()
    {
        if (player != null)
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
