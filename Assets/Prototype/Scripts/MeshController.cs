using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshController : MonoBehaviour {

    public MeshRenderer mesh;
    public bool isRender = false;
    private float lastTime;
    private float currentTime;

    void Awake()
    {
        mesh = GetComponentInChildren<MeshRenderer>();
    }

    void Start()
    {

    }

    void Update()
    {
        if (currentTime != 0)
        {
            isRender = currentTime != lastTime ? true : false;
            lastTime = currentTime;
            //if (isRender)
            //{
            //    Debug.Log("在摄像机范围内");
            //}
            //else
            //{
            //    Debug.Log("在摄像机范围外");
            //}
        }
    }

    void OnWillRenderObject()
    {
        //是需要渲染的摄像机 
        if (Camera.current.name == Camera.main.name)
        {
            currentTime = Time.time;
        }
    }

}
