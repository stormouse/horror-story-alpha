using System.Collections.Generic;
using UnityEngine;

public class TreeControl : MonoBehaviour{

    public ParticleSystem particles;
    public TrailRenderer trail;
    List<NetworkCharacter> possessors = new List<NetworkCharacter>();

    void Awake()
    {
        HideSprites();
    }


    public void Possess(NetworkCharacter character)
    {
        if (!possessors.Contains(character))
        {
            possessors.Add(character);
        }

        ShowSprites();
    }

    public void Leave(NetworkCharacter character)
    {
        if(possessors.Contains(character))
        {
            possessors.Remove(character);
        }

        if(possessors.Count == 0)
        {
            HideSprites();
        }
    }


    private void ShowSprites()
    {
        particles.Play();
        trail.enabled = true;
    }

    private void HideSprites()
    {
        particles.Stop();
        trail.enabled = false;
    }

}
