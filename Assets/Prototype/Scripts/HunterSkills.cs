using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody), typeof(NetworkCharacter)), NetworkSettings(sendInterval = 0f)]
public class HunterSkills : NetworkBehaviour {

    // skills
    public float hookCooldown = 10.0f;
    public float hookRange = 5.0f;
    public float hookSpeed = 1.0f;
    public float hookSpellTime = 0.4f;
    public bool hasHook = true;
    public bool HookReady { get { return Time.time - lastHookTime > hookCooldown; } }
    private float lastHookTime = 0.0f;

    public float wardCooldown = 5.0f;
    public float wardCount = 2;
    public float wardAnimationLength = 2.0f;
    private float lastWardTime = 0.0f;
    public bool WardReady { get { return Time.time - lastWardTime > wardCooldown && wardCount > 0; } }

    public float warSenseCooldown = 45.0f;
    public float warSenseTimeLength = 6.0f;
    public int warSenseScanPass = 3;
    public float warSenseRadius = 10.0f;
    private float lastWarSenseTime = -45.0f;
    public bool WarSenseReady { get { return Time.time - lastWarSenseTime > warSenseCooldown; } }

    public float attackRange = 5.0f;
    public float attackAngle = 90.0f;
    public float attackSpellTime = 0.17f;
    public float attackAnimationLength;
    public bool AttackReady { get { return true; } }

    // components
    public GameObject hookPrefab;
    private Rigidbody m_rigidbody;
    private NetworkCharacter character;
    private CameraFollow cameraFx;

    // private
    private GameObject hook;
    private float angle = 0f;
    private int freshCounter = 0;
    private float mDiff = 2.0f;

    private bool aiming = false;
    

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
        cameraFx = GetComponent<CameraFollow>();
        character = GetComponent<NetworkCharacter>();
    }

    void AttachSkillsToCharacter()
    {
        character.Register(CharacterState.Normal, "Hook", HookMethod);
        character.Register(CharacterState.Normal, "Attack", AttackMethod);
        character.Register(CharacterState.Normal, "WarSense", WarSenseMethod);
    }
    #endregion Setup_Skills


    void ReceivePlayerControl()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (HookReady)
            {
                character.Perform("Hook", gameObject, null);
            }
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (AttackReady)
            {
                character.Perform("Attack", gameObject, null);
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (WarSenseReady)
            {
                character.Perform("WarSense", gameObject, null);
            }
        }

        if (Input.GetMouseButton(1))
        {
            if (!aiming)
            {
                if (character.CurrentState == CharacterState.Normal)
                {
                    cameraFx.ActivateAimingPerspective();
                    aiming = true;
                }
            }
        }
        else
        {
            if (aiming)
            {
                cameraFx.Deactivate();
                aiming = false;
            }
        }
    }



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
        _AttackMethod();
    }

    [ClientRpc]
    private void RpcAttack()
    {
        if (isServer) return;
        _AttackMethod();
    }

    private void ServerAttack()
    {
        Invoke("_DoDamage", attackSpellTime);
    }

    private void _DoDamage()
    {
        var survivors = LevelManager.Singleton.GetAllSurvivorCharacters();
        foreach (var survivor in survivors)
        {
            if (Reachable(survivor.transform.position))
            {
                survivor.Perform("Die", gameObject, null);
            }
        }
    }


    private void _AttackMethod()
    {
        character.Transit(CharacterState.Casting);
        character.SwitchCoroutine(StartCoroutine(AttackCoroutine()));
    }


    private IEnumerator AttackCoroutine()
    {
        float startTime = Time.time;
        bool punched = false;
        character.Animator.SetTrigger("Attack");
        m_rigidbody.velocity = m_rigidbody.velocity * 0.85f;
        var originalVelocity = m_rigidbody.velocity;
        while (true)
        {
            float now = Time.time;
            float r = (now - startTime - attackSpellTime) / (attackAnimationLength - attackSpellTime);

            if (!punched)
            {
                if(now - startTime > attackSpellTime)
                {
                    m_rigidbody.MovePosition(transform.position + transform.forward * Vector3.Dot(m_rigidbody.velocity, transform.forward) * 0.15f);
                    punched = true;
                }
            }
            else
            {
                m_rigidbody.velocity = Vector3.zero;//Vector3.Lerp(originalVelocity, new Vector3(0, m_rigidbody.velocity.y, 0), (1.0f - r) * (1.0f - r) * (1.0f - r));
            }

            if (now - startTime < attackAnimationLength)
            {
                yield return new WaitForEndOfFrame();
            }
            else break;
        }
        character.SwitchCoroutine(null);
        // do not directly transit: Transit(CharacterState.Normal); 
        // be ready for counter-skill that can stun hunters while they attack
        character.Perform("EndCasting", gameObject, null);
    }


    bool Reachable(Vector3 position)
    {
        float sqrDistance = (position.x - transform.position.x) * (position.x - transform.position.x) + (position.y - transform.position.y) * (position.y - transform.position.y);
        if (sqrDistance < attackRange * attackRange)
        {
            if(Vector3.Angle(position-transform.position, transform.forward) < attackAngle * 0.5f)
            {
                return true;
            }
        }
        return false;
    }
    #endregion Attack_Logic


    #region Hook_Cast
    void HookMethod(GameObject sender, ActionArgument args)
    {
        if (!hasHook)
        {
            return;
        }

        if (GetComponent<AICharacter>() != null)
        {
            DirectionArgument dirc = (DirectionArgument)args;
            CmdHook(dirc.direction.normalized);
            return;
        }

        else
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit info;
            if (Physics.Raycast(mouseRay, out info))
            {
                Vector3 dest = Vector3.Scale(info.point, new Vector3(1, 0, 1));
                Vector3 origin = Vector3.Scale(transform.position, new Vector3(1, 0, 1));
                Vector3 dir = dest - origin;
                CmdHook(dir.normalized);

                // flicking support for human players
                character.SwitchCoroutine(StartCoroutine(_ThrowHookDelayPlayer(dir, hookSpellTime)));
            }
        }
    }


    [Command]
    void CmdHook(Vector3 dir)
    {
        if (!hasHook)
        {
            return;
        }
        // help AI players sync the animation
        if (GetComponent<AICharacter>() != null)
        {
            _HookMethodAIServer(dir);
        }
        _HookMethod(dir);
        RpcHook(dir);
    }


    [ClientRpc]
    void RpcHook(Vector3 dir)
    {
        if(!isServer)
        _HookMethod(dir);
    }

    [Command]
    void CmdSpawnHook(Vector3 dir)
    {
        if (character.CurrentState == CharacterState.Normal || character.CurrentState == CharacterState.Casting)
        {
            _ThrowHook(dir);
        }
    }


    void _HookMethodAIServer(Vector3 dir)
    {
        // TODO: limit switch coroutine from outside
        character.SwitchCoroutine(StartCoroutine(_ThrowHookDelayAI(dir, hookSpellTime)));
    }


    void _ThrowHook(Vector3 dir)
    {
        hook = Instantiate(hookPrefab, transform.position + mDiff * dir, Quaternion.LookRotation(dir));
        var hc = hook.GetComponent<HookControl>();
        hc.hunter = gameObject;
        hc.hookSpeed = hookSpeed;
        hc.hookRange = hookRange;
        hc.Throw();
        NetworkServer.Spawn(hook);
    }

    // flicking support
    IEnumerator _ThrowHookDelayPlayer(Vector3 original_dir, float delay)
    {
        yield return new WaitForSeconds(delay);

        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit info;
        if (Physics.Raycast(mouseRay, out info))
        {
            Vector3 dest = Vector3.Scale(info.point, new Vector3(1, 0, 1));
            Vector3 origin = Vector3.Scale(transform.position, new Vector3(1, 0, 1));
            Vector3 dir = (dest - origin).normalized;
            CmdSpawnHook(dir);
        }
        else
        {
            CmdSpawnHook(original_dir);
        }
    }

    // original hook delay method
    IEnumerator _ThrowHookDelayAI(Vector3 dir, float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("Throwing Hook");
        _ThrowHook(dir);
    }


    void _HookMethod(Vector3 dir)
    {
        character.Perform("StopMovement", gameObject, null);
        // TODO: limit transition command from outside!
        character.Transit(CharacterState.Casting);
        character.Animator.SetBool("Throwing", true);
        hasHook = false;

        lastHookTime = Time.time;
        if (isLocalPlayer)
        {
            if (PlayerUIManager.singleton != null)
                PlayerUIManager.singleton.EnterCooldown(1, hookCooldown);
        }
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



    #region WarSense Logic

    private void WarSenseMethod(GameObject sender, ActionArgument args)
    {
        // cmd activate war sense
        CmdWarSense();
    }


    [Command]
    private void CmdWarSense()
    {
        if (WarSenseReady)
        {
            RpcWarSense();
            _WarSenseMethod();
        }
    }

    [ClientRpc]
    private void RpcWarSense()
    {
        if (isServer)
            return;
        _WarSenseMethod();
    }


    private void _WarSenseMethod()
    {
        lastWarSenseTime = Time.time;

        if (isLocalPlayer)
        {
            // apply image effects
            var warsense = gameObject.AddComponent<WarSenseEffect>();
            warsense.Scan(warSenseTimeLength, warSenseRadius, warSenseScanPass);
            var survivors = LevelManager.Singleton.GetAllSurvivorCharacters();
            for(int i = 0; i < survivors.Count; i++)
            {
                survivors[i].gameObject.AddComponent<OutlineHighlight>();
            }
            StartCoroutine(ScanCoroutine(warSenseTimeLength, warSenseRadius, warSenseScanPass));
            // play audio
        }
    }

    IEnumerator ScanCoroutine(float timeLength, float scanRadius, int scanPassCount)
    {
        float startTime = Time.time;
        float now = Time.time;
        float timeOnePass = timeLength / scanPassCount;
        float timeCurrentPass = 0.0f;
        var survivors = LevelManager.Singleton.GetAllSurvivorCharacters();
        while (now - startTime < timeLength)
        {
            timeCurrentPass += Time.time - now;
            if (timeCurrentPass > timeOnePass)
                timeCurrentPass -= timeOnePass;
            float targetRadius = Mathf.Lerp(0.1f, scanRadius, timeCurrentPass / timeOnePass);
            for(int i=0; i < survivors.Count; i++)
            {
                if(Vector3.Distance(survivors[i].transform.position, this.transform.position) < targetRadius && !InLineOfSight(survivors[i].transform))
                {
                    survivors[i].GetComponent<OutlineHighlight>().Activate();
                }
                else
                {
                    survivors[i].GetComponent<OutlineHighlight>().Deactivate();
                }
            }
            now = Time.time;
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < survivors.Count; i++)
        {
            var highlight = survivors[i].GetComponent<OutlineHighlight>();
            highlight.Deactivate();
            Destroy(highlight);
        }
    }

    private bool InLineOfSight(Transform t)
    {
        if(Physics.Raycast(transform.position, t.position, Vector3.Distance(transform.position, t.position), ~(1 << LayerMask.NameToLayer("Player"))))
        { 
            return false;
        }
        return true;
    }

    #endregion WarSense Logic



}
