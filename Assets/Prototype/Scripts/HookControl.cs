using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(sendInterval = 0)]
public class HookControl : NetworkBehaviour {

    //private
    private Vector3 origin;
    [HideInInspector]
    public float hookSpeed;
    [HideInInspector]
    public float hookRange;

    //public hunter
    public GameObject hunter;
    

    void Start()
    {
        origin = transform.position;
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
            NetworkServer.Destroy(this.gameObject);
        }
    }
    

    void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;

        var otherCharacter = other.GetComponent<NetworkCharacter>();
        if (otherCharacter)
        {
            var dir = (transform.position - origin).normalized;
            var offset = dir * 3.5f;
            var duration = Vector3.Distance(transform.position, origin) / hookSpeed;
            var args = new StunArgument();
            args.time = duration + 0.3f;
            otherCharacter.Perform("Stun", gameObject, args);
            otherCharacter.MoveTo(hunter.transform.position + offset, MoveMethod.Tween, duration);
        }
        hunter.GetComponent<HunterSkills>().ReturnHook();
        NetworkServer.Destroy(gameObject);
        //Destroy(gameObject);
    }

}
