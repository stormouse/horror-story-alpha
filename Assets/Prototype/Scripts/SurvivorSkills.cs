using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(sendInterval = 0)]
public class SurvivorSkills : NetworkBehaviour {

    public bool hooked = false;
    public float force = 30f;
    public bool alive = true;

    private string hookTag = "Hook";
    private Vector3 fwd;

    void OnCollisionEnter(Collision collision)
    {
        if (!hooked && collision.collider.tag == hookTag && collision.collider.gameObject.GetComponent<HookControl>().hunter != this.gameObject)
        {
            hooked = true;
            fwd = collision.collider.gameObject.GetComponent<HookControl>().hunter.transform.position - this.transform.position;
            GetComponent<Rigidbody>().velocity = fwd.normalized * force;
        }
        else if(hooked)
        {
            hooked = false;
            alive = false;
            GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            collision.collider.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
        }
    }
}
