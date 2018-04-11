using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


[NetworkSettings(sendInterval = 0)]
public class SpeedMultiplier : NetworkBehaviour {

    public List<ISpeedEffectSource> sources;

    [SyncVar] // changing this will automatically set dirty bit
    private float currentLowestMultiplier = 1.0f;

    public float value { get { return currentLowestMultiplier; } }


    [ServerCallback]
    private void Awake()
    {
        sources = new List<ISpeedEffectSource>();
    }

    // server only
    public void AddSource(ISpeedEffectSource source)
    {
        if (!sources.Contains(source)) {
            sources.Add(source);
            if(source.Multiplier() < currentLowestMultiplier)
            {
                currentLowestMultiplier = source.Multiplier();
            }
        }
    }

    // server only
    public void RemoveSource(ISpeedEffectSource source)
    {
        if (sources.Contains(source))
        {
            sources.Remove(source);
            RecalculateLowestMultiplier();
        }
    }

    // server only
    private void RecalculateLowestMultiplier()
    {
        currentLowestMultiplier = 1.0f;
        for(int i = 0; i < sources.Count; i++)
        {
            var t = sources[i].Multiplier();
            if (t < currentLowestMultiplier)
            {
                currentLowestMultiplier = t;
            }
        }
    }

}
