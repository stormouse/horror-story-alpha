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


public class ActionArgument { }
public class StunArgument : ActionArgument { public float time; }
public class MoveArgument : ActionArgument { public float h, v; }
public delegate void ActionFunction(GameObject sender, ActionArgument args);


[RequireComponent(typeof(Rigidbody), typeof(Animator)), NetworkSettings(sendInterval = 0f)]
public class NetworkCharacter : NetworkBehaviour {

    /* Static Keywords */
    public static readonly string Wake     = "Wake";
    public static readonly string Attack   = "Attack";
    public static readonly string Die      = "Die";
    public static readonly string Stun     = "Stun";
    public static readonly string EndCasting = "EndCasting";


    /* Private Components */
    private Rigidbody m_rigidbody;
    private Animator m_animator;
    public Animator Animator { get { return m_animator; } } // ...

    
    /* Public Property */
    [SerializeField]
    private CharacterState currentState;
    public CharacterState CurrentState { get { return currentState; }}
    public float attackAnimationLength;


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
        Register(CharacterState.Normal, Attack, AttackMethod);
        Register(CharacterState.Casting, EndCasting, EndCastingMethod);
        Register(CharacterState.Stunned, Stun, StunMethod);
        Register(CharacterState.Stunned, Wake, WakeMethod);
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


    #region Attack_Logic

    private void AttackMethod(GameObject sender, ActionArgument args)
    {
        CmdAttack();
    }

    [Command]
    private void CmdAttack()
    {
        ServerAttack();
        RpcAttack();
    }

    [ClientRpc]
    private void RpcAttack()
    {
        _AttackMethod();
    }

    private void ServerAttack()
    {
        // deal real damage if hit
        // maybe call RpcSplashBlood()
    }

    private void _AttackMethod()
    {
        m_rigidbody.velocity = Vector3.zero;
        m_animator.SetTrigger(Attack);
        Transit(CharacterState.Casting);
        currentCoroutine = StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        float startTime = Time.time;
        while (true)
        {
            float now = Time.time;
            if (now - startTime < attackAnimationLength)
            {
                yield return new WaitForEndOfFrame();
            }
            else break;
        }
        currentCoroutine = null;
        // do not directly transit: Transit(CharacterState.Normal); 
        // be ready for counter-skill that can stun hunters while they attack
        Perform(EndCasting, gameObject, null);
    }

    #endregion


    #region Skill_Logic
    private void EndCastingMethod(GameObject sender, ActionArgument args)
    {
        if (isServer)
        {
            RpcEndCastingMethod();
        }
    }

    private void RpcEndCastingMethod()
    {
        Transit(CharacterState.Normal);
    }
    #endregion Skill_Logic

    #region Move_Logic
    /*
     * We don't write move logic in that fancy way
     * since it could be called very frequently (every frame).
     * Also, it's the only thing that local player owns the authority
     * thus instead, we check character.currentState at PlayerControl
     */

    private void UpdateAnimatorSpeed()
    {
        m_animator.SetFloat("Speed", m_rigidbody.velocity.magnitude);
    }

    #endregion Move_Logic


}

