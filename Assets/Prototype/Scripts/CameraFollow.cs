using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(sendInterval = 0f)]
public class CameraFollow : NetworkBehaviour {
    
    public float cameraDistance = 16f;
    public float cameraHeight = 16f;

    private Transform mainCamera;
    private Vector3 cameraOffset;

    private void Start()
    {
        if(!isLocalPlayer)
        {
            Destroy(this);
            return;
        }
        
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
        cameraOffset = new Vector3(0f, cameraHeight, -cameraDistance);
        mainCamera.Translate(cameraOffset);
        mainCamera.LookAt(transform);
    }
}
