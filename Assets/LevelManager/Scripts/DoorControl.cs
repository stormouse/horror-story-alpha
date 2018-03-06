using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DoorControl : MonoBehaviour
{
    public int m_NumOfPowerToOpen = 3;

    private Animator m_DoorAnimator;
    private bool m_DoorOpen = false;

    public int NumberOfPowerToOpen
    {
        get
        {
            return m_NumOfPowerToOpen;
        }
    }
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
