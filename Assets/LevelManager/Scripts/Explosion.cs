using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

    public float delay = 2f;
    public float radius = 5f;
    public float explosionForce = 700f;
    float countdown;

    public GameObject explosionEffect;

	// Use this for initialization
	void Start () {
        countdown = delay;

	}
	
	// Update is called once per frame
	void Update () {
        countdown -= Time.deltaTime;
        if (countdown <= 0f)
        {
            Explode();
        }
	}

    void Explode()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, transform.rotation);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (var nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, radius);
            }
        }
    
                
    }
}
