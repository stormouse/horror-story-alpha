using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControl : MonoBehaviour {

	Animation ani;

	// Use this for initialization
	void Start () {
		ani = GetComponent<Animation> ();
	}
	
	// Update is called once per frame
	void Update () {
		ani.CrossFade ("walk");
	}
}
