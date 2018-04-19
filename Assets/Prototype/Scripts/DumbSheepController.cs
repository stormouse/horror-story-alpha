using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DumbSheepController : MonoBehaviour {

    [SerializeField] float m_MovingTurnSpeed = 360;
    [SerializeField] float m_StationaryTurnSpeed = 180;
    [SerializeField] float m_SmallestSpeedToTurn = 1;

    private Animator m_Animator;
    private Rigidbody m_Rigidbody;
    private float m_ForwardAmount;
    private Vector3 m_OriginalPosition;
    private float m_TurnAmount;
    // Use this for initialization
    void Start () {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();

        m_OriginalPosition = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        UpdateAnimatorSpeed();
        ApplyRotation();
       
    }

    void UpdateAnimatorSpeed()
    {
        Vector3 vDirection = transform.InverseTransformDirection(m_Rigidbody.velocity.normalized);
        if (vDirection.z > 0)
            m_Animator.SetFloat("Speed", m_Rigidbody.velocity.magnitude);
        else
            m_Animator.SetFloat("Speed", m_Rigidbody.velocity.magnitude * -1);
    }

    void ApplyRotation()
    {
        if (m_Rigidbody.velocity.magnitude < m_SmallestSpeedToTurn)
            return;

        Vector3 vDirection = transform.InverseTransformDirection(m_Rigidbody.velocity.normalized);

        if (vDirection.z > 0)
            m_TurnAmount = Mathf.Atan2(vDirection.x, vDirection.z);
        else
            m_TurnAmount = Mathf.Atan2(-vDirection.x, -vDirection.z);

        float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
        transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
    }
}
