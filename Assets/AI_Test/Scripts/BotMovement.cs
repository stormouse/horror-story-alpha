using UnityEngine;



[RequireComponent(typeof(Rigidbody))]
public class BotMovement : MonoBehaviour {

    public enum MovementType
    {
        Manual,
        Seek,
        Flee,
        Arrival,
        Wander,
        Pursuit,
        Evade,
        AvoidCollision,
        FollowPath,
        FollowLeader,
        Queue
    };

    public float moveSpeed;
    public float steeringForce = 10.0f;
    public MovementType movementType;

    private float mass = 5.0f;
    private Rigidbody m_rigidbody;
    private Transform target;

    private void Start()
    {
        SetupComponents();
    }

    private void Update()
    {
        if(movementType == MovementType.Seek)
        {
            SeekUpdate();
        }
        else if(movementType == MovementType.Pursuit)
        {
            PursuitUpdate();
        }
    }

    private void SetupComponents()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        mass = m_rigidbody.mass;
    }


    public void Move(float h, float v, float speed = -1.0f)
    {
        movementType = MovementType.Manual;
        Vector3 dir = new Vector3(h, 0, v).normalized;
        Vector3 movement = dir * (speed > 0 ? speed : moveSpeed);
        m_rigidbody.velocity = movement;
    }


    public void Seek(Transform target)
    {
        this.target = target;
        movementType = MovementType.Seek;
    }

    public void Pursue(Transform target)
    {
        this.target = target;
        movementType = MovementType.Pursuit;
    }


    public void Wander()
    {
        movementType = MovementType.Wander;
    }


    private void SeekUpdate()
    {
        if (target) {
            Vector3 currentVelocity = m_rigidbody.velocity;
            Vector3 desiredVelocity = moveSpeed * (target.position - transform.position);
            Vector3 steering = desiredVelocity - currentVelocity;
            float acc = steeringForce / mass;
            if(steering.sqrMagnitude > acc * acc)
                steering = steering.normalized * acc;
            m_rigidbody.velocity = Vector3.ClampMagnitude(currentVelocity + steering, moveSpeed);
            m_rigidbody.MoveRotation(Quaternion.LookRotation(m_rigidbody.velocity));
        }
        else
        {
            movementType = MovementType.Manual;
        }
    }

    private void PursuitUpdate()
    {
        if (target)
        {
            if (target)
            {
                Vector3 lookAhead = Vector3.zero;
                var prey = target.GetComponent<Rigidbody>();
                if (prey) {
                    Vector3 preyVelocity = prey.velocity;
                    float timeAhead = Vector3.Distance(target.position, transform.position) / moveSpeed;
                    lookAhead = preyVelocity * timeAhead;
                }

                Vector3 destination = target.position + lookAhead;
                Vector3 currentVelocity = m_rigidbody.velocity;
                Vector3 desiredVelocity = moveSpeed * (destination - transform.position);
                Vector3 steering = desiredVelocity - currentVelocity;
                float acc = steeringForce / mass;
                if (steering.sqrMagnitude > acc * acc)
                    steering = steering.normalized * acc;
                m_rigidbody.velocity = Vector3.ClampMagnitude(currentVelocity + steering, moveSpeed);
                m_rigidbody.MoveRotation(Quaternion.LookRotation(m_rigidbody.velocity));
            }
            else
            {
                movementType = MovementType.Manual;
            }
        }
    }

}
