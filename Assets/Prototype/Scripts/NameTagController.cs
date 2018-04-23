using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameTagController : MonoBehaviour {

    public Transform canvas;
    public Text nametag;
    public int Size = 24;
	// Use this for initialization
	void Start () {
        //nametag.text = this.name;
        if (this.GetComponent<NetworkCharacter>())
        {
            SetUpNameTag(this.GetComponent<NetworkCharacter>().playerName);
        }
	}
	
	// Update is called once per frame
	void Update () {
        canvas.rotation = Camera.main.transform.rotation;
    }

    public void SetUpNameTag(string aname)
    {
        nametag.fontSize = Size;
        nametag.text = aname;
    }
}
