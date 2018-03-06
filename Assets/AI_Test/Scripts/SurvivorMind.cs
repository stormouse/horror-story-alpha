using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorMind : MonoBehaviour, ISensible
{

    public List<GameObject> hunterList = null;
    public List<GameObject> powerSourceList = null;
    float lastDecisionTime = 0.0f;
    public float decisionInterval = 1.0f;
    public BotMovement movement = null;
    public NetworkCharacter character = null;
    public Transform target = null;
    public Transform evadeTarget = null;
    public float startEvasionThreshold = 10.0f;
    private bool selectNearestTarget = true;
    bool activated = false;

    public void Hear(GameObject obj)
    {
        throw new NotImplementedException();
    }

    public void LoseHearing(GameObject obj)
    {
        throw new NotImplementedException();
    }

    public void LoseSight(GameObject obj)
    {
        throw new NotImplementedException();
    }

    public void See(GameObject obj)
    {
        throw new NotImplementedException();
    }


    private void Start()
    {
        if (!activated)
        {
            Activate();
        }
    }

    public void Activate()
    {
        GetGameRefs();
        activated = true;
    }

	
    void GetGameRefs()
    {
        hunterList = new List<GameObject>();
        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            var ch = player.GetComponent<NetworkCharacter>();
            if (ch && ch.Team == GameEnum.TeamType.Hunter) // production environment
            {
                hunterList.Add(ch.gameObject);
            }
            else if (player.gameObject.name == "AI_Hunter") // debug environment
            {
                hunterList.Add(player.gameObject);
            }
        }

        powerSourceList = new List<GameObject>();
        foreach (var ps in GameObject.FindGameObjectsWithTag("PowerSource"))
        {
            powerSourceList.Add(ps);
        }
    }

	// Update is called once per frame
	void Update () {
        if (!activated) return;
        if (character && character.CurrentState != CharacterState.Normal) return;

        if (Time.time - lastDecisionTime > decisionInterval)
        {
            MakeDecision();
            lastDecisionTime = Time.time;
        }
    }

    public void OnMovementBeginTwitch()
    {
        lastDecisionTime += decisionInterval;
        selectNearestTarget = false;
        target = null;
        evadeTarget = null;
        MakeDecision();
        selectNearestTarget = true;
    }

    public void OnMovementReachedTarget()
    {
        Debug.Log("On Movement Reached Target");
        if (character && !ImInDanger())
        {
            movement.StopMovement();
            StartCoroutine(ChargingCoroutine());   
        }
    }


    IEnumerator ChargingCoroutine()
    {
        while (target && !ImInDanger())
        {
            character.Perform("Charge", gameObject, null);
            if (character.CurrentState != CharacterState.Casting && 
                character.CurrentState != CharacterState.Normal) break;
            var psc = target.GetComponent<PowerSourceController>();
            if(psc == null || (psc && psc.Charged))
            {
                target = null;
                break;
            }
            yield return new WaitForEndOfFrame();   
        }
        GetGameRefs();

        character.Perform("EndCasting", gameObject, null);

        if (character.CurrentState == CharacterState.Normal)
        {
            movement.ResumeMovement();
            MakeDecision();
        }

    }


    bool ImInDanger()
    {
        if(evadeTarget && Vector3.Distance(evadeTarget.position, transform.position) < startEvasionThreshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void MakeDecision()
    {
        float minDist = float.MaxValue;

        if (evadeTarget == null)
        {
            Transform closestHunter = null;
            if (hunterList != null)
            {
                foreach (var hunter in hunterList)
                {
                    float dist = Vector3.Distance(hunter.transform.position, transform.position);
                    if (dist < minDist)
                    {
                        closestHunter = hunter.transform;
                        minDist = dist;
                    }
                }
            }
            evadeTarget = closestHunter;
        }
        
        if (target == null)
        {
            if (powerSourceList != null)
            {
                if (selectNearestTarget)
                {
                    minDist = float.MaxValue;
                    Transform closestPowerSource = null;
                    foreach (var ps in powerSourceList)
                    {
                        float dist = Vector3.Distance(ps.transform.position, transform.position);
                        if (dist < minDist)
                        {
                            closestPowerSource = ps.transform;
                            minDist = dist;
                        }
                    }
                    target = closestPowerSource;
                }
                else
                {
                    float maxDist = 0.0f;
                    Transform farthest = null;
                    foreach (var ps in powerSourceList)
                    {
                        float dist = Vector3.Distance(ps.transform.position, transform.position);
                        if (dist > maxDist)
                        {
                            farthest = ps.transform;
                            maxDist = dist;
                        }
                    }
                    target = farthest;
                }
            }
        }

        if (target != null && evadeTarget != null)
        {
            if (movement)
            {
                movement.SeekAndEvade(target, evadeTarget);
            }
        }
        else if(target != null && evadeTarget == null)
        {
            if (movement)
            {
                movement.Seek(target);
            }
        }
        else if(target == null && evadeTarget != null)
        {
            if (movement)
            {
                movement.Evade(evadeTarget);
            }
        }
        else
        {
            if (movement)
            {
                movement.Wander();
            }
        }
    }
}
