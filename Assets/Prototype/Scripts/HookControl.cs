using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookControl : MonoBehaviour {

    //private
    private float x, z;
    private Vector3 fwd;
    private float hookSpeed = 30f;
    private float hookRange = 20f;
    private string hunterTag = "Hunter";

    //public hunter
    public GameObject hunter;

    // Use this for initialization
    void Start()
    {
        //Get born position and move direction
        x = transform.position.x;
        z = transform.position.z;
        hookSpeed = hunter.GetComponent<HunterSkills>().hookSpeed;
        hookRange = hunter.GetComponent<HunterSkills>().hookRange;
        fwd = transform.forward;
        GetComponent<Rigidbody>().velocity = fwd * hookSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position.z - z) * (transform.position.z - z) + (transform.position.x - x) * (transform.position.x - x)
            > hookRange * hookRange)
        {
            hunter.GetComponent<HunterSkills>().ReturnHook();
            Debug.Log("Update Destroy");
            Destroy(this.gameObject);
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == hunterTag)
        {
            collision.collider.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
        }
        hunter.GetComponent<HunterSkills>().ReturnHook();
        Debug.Log("Collision Destroy");
        Destroy(this.gameObject);
    }

}
