using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(sendInterval = 0)]
public class SmokeControl : NetworkBehaviour, ISpeedEffectSource {

    public ParticleSystem smokeParticles;
    public GameObject bulletBody;
    public Rigidbody m_rigidbody;
    public Collider influenceVolume;


    [HideInInspector]
    public Vector3 origin;

    [HideInInspector]
    public Vector3 destination;
    
    [HideInInspector]
    public float flySpeed;

    [HideInInspector]
    public float maxTimeInAir;

    [HideInInspector]
    public float smokeEffectiveTime;

    [HideInInspector]
    public float decelerationFactor;

    bool exploded = false;
    // characters affected by smoke
    private List<NetworkCharacter> affectedCharacters = new List<NetworkCharacter>();


    private void Start()
    {
        influenceVolume.enabled = false;
        Throw();
    }

    // main action
    public void Throw()
    {
        m_rigidbody.velocity = (destination - origin).normalized * flySpeed;
        transform.position = origin;
        Invoke("Explode", Mathf.Min(Vector3.Distance(origin, destination) / flySpeed, maxTimeInAir));
    }

    // explode
    void Explode()
    {
        if (isServer) {
            RpcExplode();
            Invoke("DestroySelf", smokeEffectiveTime);
        }
        
        exploded = true;
        influenceVolume.enabled = true;
        bulletBody.SetActive(false);
        
        var particleMain = smokeParticles.main;
        particleMain.startLifetime = smokeEffectiveTime;
        smokeParticles.Play();
        
    }


    // broadcast
    [ClientRpc]
    void RpcExplode()
    {
        if (!isServer) {
            bulletBody.SetActive(false);
            smokeParticles.Play();
        }
    }

    // destroy through network
    void DestroySelf()
    {
        for(int i = 0; i < affectedCharacters.Count; i++)
        {
            var s = affectedCharacters[i].GetComponent<SpeedMultiplier>();
            s.RemoveSource(this);
        }
        NetworkServer.Destroy(this.gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;
        if (!exploded) return;
        
        var character = other.GetComponent<NetworkCharacter>();
        if (character == null) return;
        //if (character.Team == GameEnum.TeamType.Hunter)
        {
            if (!affectedCharacters.Contains(character))
            {
                var sm = character.GetComponent<SpeedMultiplier>();
                if (sm)
                {
                    sm.AddSource(this);
                    affectedCharacters.Add(character);
                }
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (!isServer) return;
        if (!exploded) return;
        
        var character = other.GetComponent<NetworkCharacter>();
        if (character == null) return;
        //if (character.Team == GameEnum.TeamType.Hunter)
        {
            if (affectedCharacters.Contains(character))
            {
                var sm = character.GetComponent<SpeedMultiplier>();
                if (sm)
                {
                    sm.RemoveSource(this);
                    affectedCharacters.Remove(character);
                }
            }
        }
    }


    public float Multiplier()
    {
        return decelerationFactor;
    }
}
