using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody), typeof(NetworkCharacter)), NetworkSettings(sendInterval = 0f)]
public class HunterSkills : NetworkBehaviour {

    // properties
    public float moveSpeed = 5f;
    public float hookRange = 5.0f;
    public float hookSpeed = 1.0f;
    public bool hasHook = true;

    // components
    public GameObject hookPrefab;
    private Rigidbody m_rigidbody;
    private NetworkCharacter character;

    // private
    private GameObject hook;
    private float angle = 0f;
    private int freshCounter = 0;
    private float mDiff = 1.5f;

    #region Builtin_Functions
    void Start()
    {
        SetupComponents();
        AttachSkillsToCharacter();
    }


    void Update()
    {
        if (isLocalPlayer)
        {
            ReceivePlayerControl();
        }
    }
    #endregion Builtin_Functions


    #region Setup_Skills
    void SetupComponents()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        character = GetComponent<NetworkCharacter>();
    }

    void AttachSkillsToCharacter()
    {
        var character = GetComponent<NetworkCharacter>();
        if (!character) return;
        character.Register(CharacterState.Normal, "Hook", HookMethod);
    }
    #endregion Setup_Skills


    void ReceivePlayerControl()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            character.Perform("Hook", gameObject, null);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            character.Perform("Attack", gameObject, null);
        }
    }



    #region Hook_Cast
    void HookMethod(GameObject sender, ActionArgument args)
    {
        if (!hasHook)
        {
            return;
        }
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit info;
        if (Physics.Raycast(mouseRay, out info))
        {
            Vector3 dest = Vector3.Scale(info.point, new Vector3(1, 0, 1));
            Vector3 origin = Vector3.Scale(transform.position, new Vector3(1, 0, 1));
            Vector3 dir = dest - origin;
            CmdHook(dir.normalized);
        }
    }


    [Command]
    void CmdHook(Vector3 dir)
    {
        if (!hasHook)
        {
            return;
        }
        _HookMethodServer(dir);
        _HookMethod(dir);
        RpcHook(dir);
    }


    [ClientRpc]
    void RpcHook(Vector3 dir)
    {
        _HookMethod(dir);
    }


    void _HookMethodServer(Vector3 dir)
    {
        // TODO: limit switch coroutine from outside
        character.SwitchCoroutine(StartCoroutine(_ThrowHookDelay(dir, 0.4f)));
    }


    IEnumerator _ThrowHookDelay(Vector3 dir, float delay)
    {
        yield return new WaitForSeconds(delay);
        hook = Instantiate(hookPrefab, transform.position + mDiff * dir, Quaternion.LookRotation(dir));
        var hc = hook.GetComponent<HookControl>();
        hc.hunter = gameObject;
        hc.hookSpeed = hookSpeed;
        hc.hookRange = hookRange;
        hc.Throw();
        NetworkServer.Spawn(hook);
    }


    void _HookMethod(Vector3 dir)
    {
        character.Perform("StopMovement", gameObject, null);
        // TODO: limit transition command from outside!
        character.Transit(CharacterState.Casting);
        character.Animator.SetBool("Throwing", true);
        hasHook = false;
    }
    #endregion Hook_Cast


    #region Hook_Return
    public void ReturnHook()
    {
        if (!isServer) return;
        if (hasHook) return;
        hasHook = true;
        RpcReturnHook();
    }

    [ClientRpc]
    void RpcReturnHook()
    {
        hasHook = true;
        character.Animator.SetBool("Throwing", false);
        character.Perform("EndCasting", gameObject, null);
    }
    #endregion Hook_Return
    

}
