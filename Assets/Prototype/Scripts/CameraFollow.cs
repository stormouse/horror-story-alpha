using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(sendInterval = 0f)]
public class CameraFollow : NetworkBehaviour {

    public Vector3 cameraOffset;
    public float targetHeight = 1.0f;
    [Range(0.01f, 1.0f)]
    public float smoothFactor = 0.5f;
    [Range(1.0f, 20.0f)]
    public float focalDistance = 5.0f;

    private LayerMask playerLayer;
    private Transform mainCamera;
    

    private void Start()
    {
        if(!isLocalPlayer)
        {
            Destroy(this);
            return;
        }

        playerLayer = LayerMask.NameToLayer("Player");
        mainCamera = Camera.main.transform;
        mainCamera.position = transform.position + cameraOffset;
        mainCamera.LookAt(transform.position + focalDistance * transform.forward);
        CameraMove();
    }

    private void FixedUpdate()
    {
        CameraMove();
    }

    void CameraMove()
    {
        // the final camera position
        var targetCameraPosition = transform.position + transform.TransformDirection(cameraOffset);

        // correct final camera position
        targetCameraPosition = GetUnblockedCameraPosition(targetCameraPosition, 1.0f);

        // the target focal point to look at
        var targetFocalPoint = transform.position + focalDistance * transform.forward;

        // desired camera position after smoothing
        var desiredCameraPosition = Vector3.Slerp(mainCamera.position, targetCameraPosition, 1.0f / smoothFactor * Time.deltaTime);

        // correct desired camera position
        desiredCameraPosition = GetUnblockedCameraPosition(desiredCameraPosition, 1.0f);

        // update camera position
        mainCamera.position = desiredCameraPosition;
        mainCamera.rotation = Quaternion.LookRotation(targetFocalPoint - desiredCameraPosition);
    }

    Vector3 GetUnblockedCameraPosition(Vector3 targetPosition, float skippingRadius)
    {
        RaycastHit hit;
        var objectCenter = (transform.position + Vector3.up * targetHeight);
        var rayDirectionFromTarget = (targetPosition - objectCenter).normalized;
        var rayFromTarget = new Ray(objectCenter, rayDirectionFromTarget);
        var distance = Vector3.Distance(objectCenter, targetPosition);
        if (Physics.Raycast(rayFromTarget, out hit, distance, ~(1<<playerLayer)))
        {
            var extension = 0.1f;
            targetPosition = hit.point + hit.normal * extension;
        }
        return targetPosition;
    }

}
