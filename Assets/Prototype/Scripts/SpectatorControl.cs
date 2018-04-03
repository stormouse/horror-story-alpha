using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorControl : MonoBehaviour {

    public float moveSpeed = 30.0f;
    public float zoomSpeed = 2.0f;
    public float rotateSpeed = 20.0f;
    public CameraFollow cameraFollow;

    private void Start()
    {
        if (cameraFollow == null)
            cameraFollow = GetComponent<CameraFollow>();
    }

    void Update () {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, 0, v);
        if (dir.magnitude > 1.0f)
            dir = dir.normalized;

        this.transform.Translate(dir * moveSpeed * Time.deltaTime);


        float rotate = 0.0f;
        if (Input.GetKey(KeyCode.Q))
        {
            rotate -= 1.0f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotate += 1.0f;
        }
        this.transform.Rotate(Vector3.up, rotate * rotateSpeed * Time.deltaTime);
        
        float zoom = Input.GetAxis("Mouse ScrollWheel");
        if (cameraFollow)
        {
            float height = cameraFollow.cameraHeight - zoom * zoomSpeed;
            height = Mathf.Clamp(height, 10.0f, 100.0f);
            cameraFollow.cameraHeight = height;
        }
    }
}
