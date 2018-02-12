using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager_CameraController : NetworkManager
{

    public Transform sceneCamera;
    public float cameraRotationRadius = 24f;
    public float cameraRotationSpeed = 3f;
    bool canRotate = true;

    float rotation;

    public override void OnStartClient(NetworkClient client)
    {
        canRotate = false;
    }

    public override void OnStartHost()
    {
        canRotate = false;
    }

    public override void OnStopClient()
    {
        canRotate = true;
    }

    public override void OnStopHost()
    {
        canRotate = true;
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!canRotate)
            return;

        rotation += cameraRotationSpeed * Time.deltaTime;
        if (rotation >= 360f)
            rotation -= 360f;

        sceneCamera.position = Vector3.zero;
        sceneCamera.rotation = Quaternion.Euler(0f, rotation, 0f);
        sceneCamera.Translate(0f, cameraRotationRadius, -cameraRotationRadius);
        sceneCamera.LookAt(Vector3.zero);
    }
}
