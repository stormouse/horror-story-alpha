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

    [Header("Mouse Variables")]
    public float XSensitivity = 2f;
    //public float YSensitivity = 2f;
    //public bool clampVerticalRotation = true;
    //public float MinimumX = -90F;
    //public float MaximumX = 90F;
    public bool smooth;
    public float smoothTime = 5f;
    public bool lockCursor = true;

    private bool m_cursorIsLocked = true;
    private Quaternion m_CameraTargetRot;

    [Header("Camera Variables")]
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
    private CameraFollowParameters lookbackParameters;

    private Vector3 mouseForward;
    private Vector3 mouseOffset;

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
        

        mouseForward = transform.forward;
        mouseOffset = cameraOffset;

        mainCamera.position = transform.position + cameraOffset;
        mainCamera.LookAt(transform.position + focalDistance * mouseForward);

        m_CameraTargetRot = Quaternion.Euler(0f, 0f, 0f);

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


    public void ActiveLookbackPerspective()
    {
        ApplyCameraParameters(lookbackParameters);
    }


    public void Deactivate()
    {
        ApplyCameraParameters(originalParameters);
    }

    public void LookRotation()
    {
        float yRot = Input.GetAxis("Mouse X") * XSensitivity;
        //float xRot = Input.GetAxis("Mouse Y") * YSensitivity;
        m_CameraTargetRot *= Quaternion.Euler(0f, yRot, 0f);
       
        if (smooth)
        {
            mainCamera.localRotation = Quaternion.Slerp(mainCamera.localRotation, m_CameraTargetRot, smoothTime * Time.deltaTime);
        } else
        {
            mainCamera.localRotation = m_CameraTargetRot;
        }

        mouseOffset = mainCamera.transform.TransformDirection(cameraOffset);
        mouseForward = mainCamera.transform.forward;

        UpdateCursorLock();
    }

    public void MoveRotate(Quaternion rot)
    {
        m_CameraTargetRot *= rot;
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

        // lookback
        lookbackParameters.cameraOffset = new Vector3(0.2f, 2.0f, 4.0f);
        lookbackParameters.focalDistance = -10.0f;
        lookbackParameters.targetHeight = 1.0f;
        lookbackParameters.smoothFactor = 0.2f;


        //overviewParameters
        //frenzyParameters;
    }

    void CameraMove()
    {
        // the final camera position
        var targetCameraPosition = transform.position + mouseOffset;

        // correct final camera position
        targetCameraPosition = GetUnblockedCameraPosition(targetCameraPosition, 1.0f);

        // the target focal point to look at
        var targetFocalPoint = transform.position + focalDistance * mouseForward;

        // desired camera position after smoothing
        var desiredCameraPosition = Vector3.Slerp(mainCamera.position, targetCameraPosition, 1.0f / smoothFactor * Time.deltaTime);

        // correct desired camera position
        desiredCameraPosition = GetUnblockedCameraPosition(desiredCameraPosition, 1.0f);

        // update camera position
        mainCamera.position = desiredCameraPosition;
        LookRotation();
        //mainCamera.rotation = Quaternion.LookRotation(targetFocalPoint - desiredCameraPosition);
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

    void UpdateCursorLock()
    {
        if (lockCursor)
            InternalLockUpdate();
    }

    void InternalLockUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            m_cursorIsLocked = false;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            m_cursorIsLocked = true;
        }

        if (m_cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (!m_cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    #endregion privates

}
