using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePlayerControl : MonoBehaviour {


    public float pushPower;
    public float gravityScale;
    public float maxSpeed;

    Rigidbody m_rigidBody;
    BoxCollider m_collider;
    float m_size;
    Transform m_camera;

    private void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        m_collider = GetComponentInChildren<BoxCollider>();
        m_size = m_collider.size.x;
        m_camera = Camera.main.transform;

        AlignWithCamera();
    }


    private void Update()
    {
        Vector3 pos = transform.position + Vector3.up * m_size * 0.5f;
        Vector3 cameraForward = Vector3.Scale(m_camera.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 cameraRight = Vector3.Scale(m_camera.right, new Vector3(1, 0, 1)).normalized;

        if (Input.GetKey(KeyCode.W))
            m_rigidBody.AddForceAtPosition(cameraForward * pushPower, pos, ForceMode.Force);
        if (Input.GetKey(KeyCode.A))
            m_rigidBody.AddForceAtPosition(-cameraRight * pushPower, pos, ForceMode.Force);
        if (Input.GetKey(KeyCode.S))
            m_rigidBody.AddForceAtPosition(-cameraForward * pushPower, pos, ForceMode.Force);
        if (Input.GetKey(KeyCode.D))
            m_rigidBody.AddForceAtPosition(cameraRight * pushPower, pos, ForceMode.Force);

        m_rigidBody.AddForce(Vector3.down * gravityScale, ForceMode.Acceleration);
        if(m_rigidBody.velocity.magnitude > maxSpeed)
        {
            m_rigidBody.velocity = m_rigidBody.velocity.normalized * maxSpeed;
        }
    }

    private void AlignWithCamera()
    {
        Vector3 pos = transform.position + Vector3.up * m_size * 0.5f;
        Vector3 cameraForward = Vector3.Scale(m_camera.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 cameraRight = Vector3.Scale(m_camera.right, new Vector3(1, 0, 1)).normalized;

        transform.Rotate(Quaternion.FromToRotation(transform.forward, cameraForward).eulerAngles);
        m_rigidBody.velocity = Vector3.zero;
    }



}
