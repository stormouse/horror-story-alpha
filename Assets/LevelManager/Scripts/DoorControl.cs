using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControl : MonoBehaviour
{

    public Animator m_DoorAnimator;

    private bool m_DoorOpen = false;

    public bool DoorOpen
    {
        get
        {
            return m_DoorOpen;
        }
    }

    // Use this for initialization
    void Start()
    {
        if (m_DoorAnimator == null)
            m_DoorAnimator = GetComponent<Animator>();
    }

    public void OpenDoor()
    {
        if (!m_DoorAnimator) return;
        m_DoorAnimator.SetTrigger("OpenDoor");
        m_DoorOpen = true;
    }

    public void CloseDoor()
    {
        if (!m_DoorAnimator) return;
        m_DoorAnimator.SetTrigger("CloseDoor");
        m_DoorOpen = false;
    }
}
