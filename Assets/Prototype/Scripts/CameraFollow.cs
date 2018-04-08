using UnityEngine;
using UnityEngine.Networking;


public struct CameraFollowParameters
{
    public Vector3 cameraOffset;
    public float targetHeight;
    public float smoothFactor;
    public float focalDistance;
}


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

    private CameraFollowParameters originalParameters;
    private CameraFollowParameters defaultParameters;
    private CameraFollowParameters aimingParameters;
    private CameraFollowParameters overviewParameters;
    private CameraFollowParameters frenzyParameters;


    #region builtin functions
    private void Start()
    {
        if(!isLocalPlayer)
        {
            Destroy(this);
            return;
        }
        InitCameraParameters();
        originalParameters = StoreCameraParameters();
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
    #endregion Builtin functions




    #region publics
    
    public void ActivateAimingPerspective()
    {
        ApplyCameraParameters(aimingParameters);
    }


    public void ActivateOverviewPerspective()
    {
        ApplyCameraParameters(overviewParameters);
    }


    public void Deactivate()
    {
        ApplyCameraParameters(originalParameters);
    }

    #endregion publics




    #region privates

    void InitCameraParameters()
    {
        defaultParameters = StoreCameraParameters();

        // aiming
        aimingParameters.cameraOffset = new Vector3(0.6f, 2.0f, -0.5f);
        aimingParameters.focalDistance = 20.0f;
        aimingParameters.targetHeight = 1.0f;
        aimingParameters.smoothFactor = 0.1f;

        //overviewParameters
        //frenzyParameters;
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


    CameraFollowParameters StoreCameraParameters()
    {
        var previousParameters = new CameraFollowParameters();
        previousParameters.cameraOffset = cameraOffset;
        previousParameters.focalDistance = focalDistance;
        previousParameters.smoothFactor = smoothFactor;
        previousParameters.targetHeight = targetHeight;
        return previousParameters;
    }


    void ApplyCameraParameters(CameraFollowParameters parameters)
    {
        cameraOffset = parameters.cameraOffset;
        focalDistance = parameters.focalDistance;
        smoothFactor = parameters.smoothFactor;
        targetHeight = parameters.targetHeight;
    }
    #endregion privates

}
