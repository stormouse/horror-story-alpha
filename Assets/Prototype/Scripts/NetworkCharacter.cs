using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum CharacterState
{
    Wait, // wait for game to start
    Normal,
    Stunned,
    Casting,
    Dead
}

public enum MoveMethod
{
    Teleport,
    Tween
}

public class ActionArgument { }
public class StunArgument : ActionArgument { public float time; }
public class MoveArgument : ActionArgument { public float h, v; }
public delegate void ActionFunction(GameObject sender, ActionArgument args);


[RequireComponent(typeof(Rigidbody)), NetworkSettings(sendInterval = 0f)]
public class NetworkCharacter : NetworkBehaviour {

    /* Static Keywords */
    public static readonly string Wake     = "Wake";
    public static readonly string Attack   = "Attack";
    public static readonly string Die      = "Die";
    public static readonly string Stun     = "Stun";
    public static readonly string EndCasting = "EndCasting";
    public static readonly string StopMovement = "StopMovement";


    /* Private Components */
    private Rigidbody m_rigidbody;
    private Animator m_animator;
    public Animator Animator { get { return m_animator; } } // ...

    
    /* Public Property */
    [SerializeField]
    private CharacterState currentState;
    public CharacterState CurrentState { get { return currentState; }}

    private GameEnum.TeamType team;
    public GameEnum.TeamType Team { get { return team; } }
    


    /* Character State Machine */
    private Dictionary<CharacterState, Dictionary<string, ActionFunction>> actions;
    private Coroutine currentCoroutine;


    #region Builtin_Functions
    // use awake to initialize action map
    void Awake()
    {
        InitializeVariables();
        SetupComponents();
        SetupActionMethods();
    }


    void Update()
    {
        UpdateAnimatorSpeed();
    }
    #endregion Builtin_Functions


    #region State_Machine_Functionality
    public void Perform(string action, GameObject sender, ActionArgument args)
    {
        ActionFunction method;
        bool valid = actions[currentState].TryGetValue(action, out method);
        if (valid)
        {
            method(sender, args);
        }
    }
    

    public void Transit(CharacterState newState)
    {
        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
        currentState = newState;
    }


    public void SwitchCoroutine(Coroutine c)
    {
        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = c;
        }
    }


    public void Register(CharacterState state, string action, ActionFunction method)
    {
        actions[state][action] = method;
    }


    public void Unregister(CharacterState state, string action)
    {
        Dictionary<string, ActionFunction> stateFunctions;
        if(actions.TryGetValue(state, out stateFunctions))
        {
            ActionFunction func;
            if (actions[state].TryGetValue(action, out func))
            {
                actions[state][action] = null;
                actions[state].Remove(action);
            }
        }
    }
    #endregion State_Machine_Functionality


    #region Character_Setup
    private void SetupComponents()
    {
        m_animator = GetComponent<Animator>();
        if(m_animator == null)
        {
            m_animator = GetComponentInChildren<Animator>();
        }
        if(m_animator == null)
        {
            Debug.LogWarning("No animator attached to character " + gameObject.name); 
        }
        m_rigidbody = GetComponent<Rigidbody>();
    }


    private void InitializeVariables()
    {
        currentState = CharacterState.Normal;
        currentCoroutine = null;
        actions = new Dictionary<CharacterState, Dictionary<string, ActionFunction>>();
        foreach(CharacterState state in Enum.GetValues(typeof(CharacterState)))
        {
            actions[state] = new Dictionary<string, ActionFunction>();
        }
    }


    private void SetupActionMethods()
    {
        Register(CharacterState.Normal, Stun, StunMethod);
        Register(CharacterState.Normal, StopMovement, StopMovementMethod);
        Register(CharacterState.Casting, EndCasting, EndCastingMethod);
        Register(CharacterState.Casting, Stun, StunMethod);
        Register(CharacterState.Stunned, Stun, StunMethod);
        Register(CharacterState.Stunned, Wake, WakeMethod);
        Register(CharacterState.Normal, Die, DieMethod);
        Register(CharacterState.Stunned, Die, DieMethod);
        Register(CharacterState.Casting, Die, DieMethod);
    }

    public void SetTeam(GameEnum.TeamType _team)
    {
        team = _team;
    }

    #endregion Charater_Setup


    #region Stun_Logic
    // we don't need [Command] for Stun
    // because stun can only be triggered on server
    // on server
    private void StunMethod(GameObject sender, ActionArgument args)
    {
        float stunTime = (args as StunArgument).time;
        RpcStun(stunTime);
    }

    // downward
	[ClientRpc]
    private void RpcStun(float time)
    {
        _StunMethod(time);
    }

    // local implementation
    private void _StunMethod(float time)
    {
        Transit(CharacterState.Stunned);

        m_animator.SetBool("Stunned", true);
        currentCoroutine = StartCoroutine(StunCoroutine(time));
    }

    // helper coroutine
    private IEnumerator StunCoroutine(float stunTime)
    {
        float startTime = Time.time;
        while (true)
        {
            float now = Time.time;
            if (now - startTime < stunTime)
            {
                yield return new WaitForEndOfFrame();
            }
            else break;
        }
        if (isServer)
        {
            Perform(Wake, gameObject, null);
        }
        currentCoroutine = null;
    }
    #endregion


    #region Wake_Logic
    // server only
    private void WakeMethod(GameObject sender, ActionArgument args)
    {
        RpcWake();
    }

    [ClientRpc]
    private void RpcWake()
    {
        _WakeMethod();
    }

    private void _WakeMethod()
    {
        m_animator.SetBool("Stunned", false);
        Transit(CharacterState.Normal);
    }
    #endregion Wake_Logic


    #region Skill_Logic
    private void EndCastingMethod(GameObject sender, ActionArgument args)
    {
        if (isServer)
        {
            RpcEndCastingMethod();
            if (currentState == CharacterState.Casting)
                Transit(CharacterState.Normal);
        }
    }

    [ClientRpc]
    private void RpcEndCastingMethod()
    {
        if (isServer) return;
        if(currentState == CharacterState.Casting)
            Transit(CharacterState.Normal);
    }
    #endregion Skill_Logic


    #region Die_Logic
    // server only
    private void DieMethod(GameObject sender, ActionArgument args)
    {
        if (isServer)
        {
            _DieServer();
            RpcDie();
            _Die();
        }
    }

    [ClientRpc]
    void RpcDie()
    {
        if (isServer) return;
        _Die();
    }


    void _DieServer()
    {
        m_animator.SetTrigger("Die");
        Transit(CharacterState.Dead);

        Observation ob = new Observation();
        ob.subject = gameObject;
        ob.what = Observation.Death;
        LevelManager.Singleton.Observe(ob);
    }


    void _Die()
    {
        // for host, same thing has been done on server
        if (isClient && isServer)
            return;

        m_animator.SetTrigger("Die");
        Transit(CharacterState.Dead);
    }


    #endregion Die_Logic


    #region Move_Logic
    /*
     * We don't write move logic in that fancy way
     * since it could be called very frequently (every frame).
     * Also, it's the only thing that local player owns the authority
     * thus instead, we check character.currentState at PlayerControl
     */

    private void UpdateAnimatorSpeed()
    {
		/*zx modified for AI*/
		if (GetComponent<AIStateController> () != null) {
			if (isServer) {
				RpcUpdateAIAnimatorSpeed (GetComponent<AIStateController> ().navMeshAgent.velocity.magnitude);
			}
		} else {
        	m_animator.SetFloat("Speed", m_rigidbody.velocity.magnitude);//original script line
		}
    }
	/*zx modified for AI*/
	[ClientRpc]
	void RpcUpdateAIAnimatorSpeed(float speed) {
		m_animator.SetFloat("Speed", speed);
	}
    // only server can call this
    public void MoveTo(Vector3 destination, MoveMethod method, float duration = 0.0f)
    {
        if (!isServer) return;
        RpcMoveToPosition(destination, method, duration);
    }


    [ClientRpc]
    private void RpcMoveToPosition(Vector3 destination, MoveMethod method, float duration)
    {
        _MoveToPosition(destination, method, duration);
    }


    private void _MoveToPosition(Vector3 destination, MoveMethod method, float duration)
    {
        // all because of localPlayerAuthority
        if (!isLocalPlayer) return;

        if (duration <= 0.01f || method == MoveMethod.Teleport)
        {
            m_rigidbody.MovePosition(destination);
            m_rigidbody.velocity = new Vector3(0, m_rigidbody.velocity.y, 0);
        }
        else if (method == MoveMethod.Tween && duration > 0.01f)
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(MoveTweenCoroutine(destination, duration));
        }
    }

    //// This cannot be invoked since rpc cannot pass Transform object
    //private void _MoveToTarget(Transform target, MoveMethod method, float duration, float offset)
    //{
    //    var off = offset * (transform.position - target.position).normalized;
    //    if (duration <= 0.01f || method == MoveMethod.Teleport)
    //    {
    //        m_rigidbody.MovePosition(target.position + off);
    //        m_rigidbody.velocity = new Vector3(0, m_rigidbody.velocity.y, 0);
    //    }
    //    else if(method == MoveMethod.Tween && duration > 0.01f)
    //    {
    //        if (currentCoroutine != null)
    //        {
    //            StopCoroutine(currentCoroutine);
    //        }
    //        currentCoroutine = StartCoroutine(MoveTweenCoroutine(target, off, duration));
    //    }
    //}


    private IEnumerator MoveTweenCoroutine(Vector3 destination, float duration)
    {
        float startTime = Time.time;
        float now = startTime;
        Vector3 original = transform.position;
        while (now - startTime < duration)
        {
            now = Time.time;
            float p = Mathf.Clamp01((now - startTime) / duration);
            m_rigidbody.MovePosition(Vector3.Lerp(original, destination, p));
            m_rigidbody.velocity = new Vector3(0, m_rigidbody.velocity.y, 0);
            yield return new WaitForEndOfFrame();
        }
        currentCoroutine = null;
    }


    private IEnumerator MoveTweenCoroutine(Transform target, Vector3 offset, float duration)
    {
        float startTime = Time.time;
        float now = startTime;
        Vector3 original = transform.position;
        while (now - startTime < duration)
        {
            now = Time.time;
            float p = (now - startTime) / duration;
            if(target == null)
            {
                yield break; // if we lose target
            }
            m_rigidbody.MovePosition(Vector3.Lerp(original, target.position + offset, p));
            yield return new WaitForEndOfFrame();
        }
        currentCoroutine = null;
    }

    private void StopMovementMethod(GameObject sender, ActionArgument args)
    {
        m_rigidbody.velocity = Vector3.zero;
    }

    #endregion Move_Logic


}

