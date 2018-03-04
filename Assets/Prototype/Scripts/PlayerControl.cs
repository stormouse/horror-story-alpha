using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


[RequireComponent(typeof(Rigidbody), typeof(NetworkCharacter)), NetworkSettings(sendInterval = 0f)]
public class PlayerControl : NetworkBehaviour {

    public int m_PlayerNumber = 1;
    [Header("Movement Variables")]
    public float m_Speed = 5.0f;
    public float m_TurnSpeed = 45.0f;

    private Rigidbody m_Rigidbody;              // Reference used to move the tank.
    private NetworkCharacter character;

    private string m_MovementAxisName;          // The name of the input axis for moving forward and back.
    private string m_TurnAxisName;              // The name of the input axis for turning.
    private float m_MovementInputValue;         // The current value of the movement input.
    private float m_TurnInputValue;             // The current value of the turn input.


    // Use this for initialization
    void Start()
    {
        if(!isLocalPlayer)
        {
			Destroy (this);
            return;
        }
        SetupComponents();
    }


    void SetupComponents()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        character = GetComponent<NetworkCharacter>();
        m_MovementAxisName = "Vertical";
        m_TurnAxisName = "Horizontal";
    }

    
    void Update()
    {
        ReceivePlayerControl();
    }

    void FixedUpdate()
    {
        Move();
        Turn();
    }


    void ReceivePlayerControl()
    {
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);
    }


    void Move()
    {
        if (character.CurrentState != CharacterState.Normal)
            return;

        // Apply movement to the rigidbody's position.
        m_Rigidbody.velocity = transform.forward * m_MovementInputValue * m_Speed;
    }


    void Turn()
    {
        if (character.CurrentState != CharacterState.Normal)
            return;

        // Determine the number of degrees to be turned based on the input, speed and time between frames.
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

        // Make this into a rotation in the y axis.
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        // Apply this rotation to the rigidbody's rotation.
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }

}
