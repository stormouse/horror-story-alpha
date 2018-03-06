using UnityEngine;
using UnityEngine.AI;



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
        Queue,
        SeekAndEvade
    };

    public float moveSpeed;
    public float steeringForce = 10.0f;
    [Range(0.1f, 100.0f)]
    public float seekEagernessConstant = 1.0f;
    [Range(0.1f, 100.0f)]
    public float evadeEagernessFactor = 1.0f;
    [Range(0.1f, 100.0f)]
    public float collisionAvoidanceEagernessFactor = 1.0f;
    public MovementType movementType;
    public float twitchMagnitudeThreshold = 0.1f;
    public float stopDistance = 5.0f;
    public SurvivorMind mind = null;
    public bool activated = true;
    public NavMeshAgent agent = null;

    private float mass = 5.0f;
    private Rigidbody m_rigidbody;
    private Transform target, evadeTarget;

    private void Start()
    {
        SetupComponents();
    }

    private void Update()
    {
        if (!activated) return;

        if(movementType == MovementType.Seek)
        {
            SeekUpdate();
        }
        else if(movementType == MovementType.Pursuit)
        {
            PursuitUpdate();
        }
        else if (movementType == MovementType.Evade)
        {
            EvadeUpdate();
        }
        else if (movementType == MovementType.SeekAndEvade)
        {
            SeekAndEvadeUpdate();
        }
    }

    private void SetupComponents()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        mass = m_rigidbody.mass;
    }


    public void ResumeMovement()
    {
        activated = true;
    }


    public void StopMovement()
    {
        activated = false;
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

    public void Evade(Transform target)
    {
        this.evadeTarget = target;
        movementType = MovementType.Evade;
    }

    public void SeekAndEvade(Transform targetToSeek, Transform targetToEvade)
    {
        target = targetToSeek;
        evadeTarget = targetToEvade;
        movementType = MovementType.SeekAndEvade;
    }


    public void Wander()
    {
        movementType = MovementType.Wander;
    }


    private void SeekUpdate()
    {
        if (target)
        {
            if (Vector3.Distance(target.position, transform.position) < stopDistance)
            {
                Wander();
                if (mind)
                {
                    mind.OnMovementReachedTarget();
                }
            }
            else
            {
                Vector3 currentVelocity = m_rigidbody.velocity;
                Vector3 desiredVelocity = moveSpeed * (target.position - transform.position + CollisionAvoidanceVector()).normalized;
                Vector3 steering = desiredVelocity - currentVelocity;
                float acc = steeringForce / mass;
                if (steering.sqrMagnitude > acc * acc)
                    steering = steering.normalized * acc;
                m_rigidbody.velocity = Vector3.ClampMagnitude(currentVelocity + steering, moveSpeed);
                m_rigidbody.MoveRotation(Quaternion.LookRotation(m_rigidbody.velocity));
            }
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

    private void SeekAndEvadeUpdate()
    {
        if (!evadeTarget)
        {
            movementType = MovementType.Seek;
            return;
        }
        if (!target)
        {
            movementType = MovementType.Evade;
            return;
        }
        if(target && evadeTarget)
        {
            Vector3 currentVelocity = m_rigidbody.velocity;
            Vector3 seekDesiredVector = (target.position - transform.position) * seekEagernessConstant;
            float evadeEagerness = evadeEagernessFactor / (Vector3.Distance(evadeTarget.position, transform.position) + 0.1f);
            Vector3 evadeDesiredVector = -(evadeTarget.position - transform.position) * evadeEagerness;
            Vector3 desiredDirection = (seekDesiredVector + evadeDesiredVector + CollisionAvoidanceVector());

            if (Vector3.Distance(target.position, transform.position) < stopDistance)
            {
                Wander();
                if (mind)
                {
                    mind.OnMovementReachedTarget();
                }
            }
            else if (desiredDirection.magnitude < twitchMagnitudeThreshold)
            {
                // begins twitching, stop movement and let our mind decide
                Wander();
                if (mind)
                {
                    mind.OnMovementBeginTwitch();
                }
            }
            else
            {
                // still moving smoothly
                Vector3 desiredVelocity = moveSpeed * desiredDirection.normalized;
                Vector3 steering = desiredVelocity - currentVelocity;
                float acc = steeringForce / mass;
                if (steering.sqrMagnitude > acc * acc)
                    steering = steering.normalized * acc;
                m_rigidbody.velocity = Vector3.ClampMagnitude(currentVelocity + steering, moveSpeed);
                m_rigidbody.MoveRotation(Quaternion.LookRotation(m_rigidbody.velocity));
            }
        }
    }


    private void EvadeUpdate() {
        Vector3 currentVelocity = m_rigidbody.velocity;
        float evadeEagerness = evadeEagernessFactor / (Vector3.Distance(evadeTarget.position, transform.position) + 0.1f);
        Vector3 evadeDesiredVector = -(evadeTarget.position - transform.position) * evadeEagerness;
        Vector3 desiredVelocity = moveSpeed * evadeDesiredVector.normalized;
        Vector3 steering = desiredVelocity - currentVelocity;
        float acc = steeringForce / mass;
        if (steering.sqrMagnitude > acc * acc)
            steering = steering.normalized * acc;
        m_rigidbody.velocity = Vector3.ClampMagnitude(currentVelocity + steering, moveSpeed);
        m_rigidbody.MoveRotation(Quaternion.LookRotation(m_rigidbody.velocity));
    }


    private void GoTo(Transform target)
    {
        if (agent)
        {
            agent.SetDestination(target.position);
        }
    }

    

    private Vector3 CollisionAvoidanceVector()
    {
        Vector3 ret = Vector3.zero;
        RaycastHit hit;
        if(Physics.Raycast(transform.position + transform.forward * 0.8f, transform.position + transform.forward * 10.0f, out hit))
        {
            if(hit.collider.tag != "PowerSource")
            {
                var scale2d = new Vector3(1, 0, 1);
                var norm = Vector3.Scale(hit.normal.normalized, scale2d);
                var dist = Vector3.Distance(hit.point, transform.position);
                var dirc = Vector3.Scale((hit.point - transform.position).normalized, scale2d);
                var refl = norm * 2.0f - dirc;
                var tang = refl - Vector3.Dot(refl, norm) * norm;
                ret = tang.normalized * collisionAvoidanceEagernessFactor / (dist + 0.01f);
            }
        }
        Debug.Log("CAV: " + ret.ToString());
        return ret;
    }
}
