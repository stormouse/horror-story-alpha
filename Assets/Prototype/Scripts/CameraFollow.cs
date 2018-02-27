using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(sendInterval = 0f)]
public class CameraFollow : NetworkBehaviour {
    
    public float cameraDistance = 16f;
    public float cameraHight = 16f;

    private Transform mainCamera;
    private Vector3 cameraOffset;

    private void Start()
    {
        if(!isLocalPlayer)
        {
            Destroy(this);
            return;
        }

        cameraOffset = new Vector3(0f, cameraHight, -cameraDistance);
        mainCamera = Camera.main.transform;
        CameraMove();
    }

    private void FixedUpdate()
    {
        CameraMove();
    }

    void CameraMove()
    {
        mainCamera.position = transform.position;
        mainCamera.rotation = transform.rotation;
        mainCamera.Translate(cameraOffset);
        mainCamera.LookAt(transform);
    }
}
