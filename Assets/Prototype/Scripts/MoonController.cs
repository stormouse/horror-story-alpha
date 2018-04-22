using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonController : MonoBehaviour {

    public float RotateSpeed = .3f;

	// Use this for initialization
	void Start () {
		
	}
	

	void FixedUpdate () {
        transform.Rotate(new Vector3(1f, 1f, 0f), RotateSpeed*Mathf.Deg2Rad);
	}
}
