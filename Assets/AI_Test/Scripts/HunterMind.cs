using System;
using UnityEngine;

public class HunterMind : MonoBehaviour, ISensible {

    /* Components */
    public BotMovement movement = null;


    GameObject playerInSight = null;
    
    public void Hear(GameObject obj)
    {
    }

    public void LoseHearing(GameObject obj)
    {
    }

    public void LoseSight(GameObject obj)
    {
        if (playerInSight == obj)
        {
            playerInSight = null;
            movement.Wander();
            GetComponent<Renderer>().material.color = new Color(163 / 255.0f, 77 / 255.0f, 77 / 255.0f);
        }
    }

    public void See(GameObject obj)
    {
        if (playerInSight == null)
        {
            playerInSight = obj;
            if (movement)
            {
                movement.Pursue(playerInSight.transform);
            }
            GetComponent<Renderer>().material.color = Color.red;
        }
    }


}
