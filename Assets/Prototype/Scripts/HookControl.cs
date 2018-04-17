using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(sendInterval = 0)]
public class HookControl : NetworkBehaviour {


    public AudioSource throwAudio;
    public AudioSource hitObjectAudio;
    public AudioSource hitSheepAudio;



    //private
    private Vector3 origin;
    [HideInInspector]
    public float hookSpeed;
    [HideInInspector]
    public float hookRange;
    

    public float additionalStunTime = 0.8f;

    //public hunter
    public GameObject hunter;
    

    void Start()
    {
        origin = transform.position;
        if (throwAudio)
        {
            throwAudio.Play();
        }
    }
    

    public void Throw()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * hookSpeed;
    }


    void Update()
    {
        if (!isServer) return;

        if (Vector3.SqrMagnitude(transform.position - origin) > hookRange * hookRange)
        {
            hunter.GetComponent<HunterSkills>().ReturnHook();
            LevelManager.Singleton.DestoryNetworkObject(gameObject, 0.3f);
        }
    }
    

    void OnTriggerEnter(Collider other)
    {
        if (!isServer)
            return;

        if (other.gameObject.tag == "PowerSource")
            return;

        if (other.gameObject.tag == "AreaVolume")
            return;

        var otherCharacter = other.GetComponent<NetworkCharacter>();
        if (otherCharacter && otherCharacter.Team == GameEnum.TeamType.Survivor)
        {
            var dir = (transform.position - origin).normalized;
            var offset = dir * 1.5f;
            var duration = Vector3.Distance(transform.position, origin) / hookSpeed;

            var args = new StunArgument();
            args.time = duration + additionalStunTime;
            otherCharacter.Perform("Stun", gameObject, args);
            otherCharacter.MoveTo(hunter.transform.position + offset, MoveMethod.Tween, duration);

            if(hitSheepAudio)
                hitSheepAudio.Play();
            RpcPlayHitSheepAudio();
            GetComponent<Rigidbody>().velocity = -GetComponent<Rigidbody>().velocity;
        }
        else
        {
            if (hitObjectAudio)
                hitObjectAudio.Play();
            RpcPlayHitObjectAudio();
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        
        hunter.GetComponent<HunterSkills>().ReturnHook();
        GetComponent<Collider>().enabled = false;
        
        LevelManager.Singleton.DestoryNetworkObject(gameObject, 0.3f);

        //NetworkServer.Destroy(gameObject);
        //Destroy(gameObject);
    }

    [ClientRpc]
    void RpcPlayHitObjectAudio()
    {
        if (!isServer && hitObjectAudio)
        {
            hitObjectAudio.Play();
        }
    }

    [ClientRpc]
    void RpcPlayHitSheepAudio()
    {
        if (!isServer && hitSheepAudio)
        {
            hitSheepAudio.Play();
        }
    }

}
