using UnityEngine;
using UnityEngine.Networking;


[NetworkSettings(sendInterval = 0)]
public class AnimatorStates : NetworkBehaviour {

    public Animator m_animator;

    private void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    public void Attack()
    {
        CmdAttack();
    }

    public void Hook()
    {
        CmdHook();
    }

    public void Stun()
    {
        CmdStun();
    }

    public void Die()
    {
        CmdDie();
    }

    [Command]
    public void CmdStun()
    {
        m_animator.SetTrigger("Stun");
        RpcStun();
    }

    [ClientRpc]
    public void RpcStun()
    {
        m_animator.SetTrigger("Stun");
        m_animator.SetBool("stunned", true);
    }

    [Command]
    public void CmdAttack()
    {
        m_animator.SetTrigger("Attack");
        RpcAttack();
    }

    [ClientRpc]
    public void RpcAttack()
    {
        m_animator.SetTrigger("Attack");
    }
    

    [Command]
    public void CmdDie()
    {
        m_animator.SetTrigger("Die");
        RpcDie();
    }

    [ClientRpc]
    public void RpcDie()
    {
        m_animator.SetTrigger("Die");
    }


    [Command]
    public void CmdHook()
    {
        m_animator.SetBool("castingSkills", true);
        RpcHook();
    }

    [ClientRpc]
    public void RpcHook()
    {

    }

}
