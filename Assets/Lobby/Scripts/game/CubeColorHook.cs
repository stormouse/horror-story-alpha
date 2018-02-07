using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(sendInterval = 0.0f)]
public class CubeColorHook : NetworkBehaviour {

    [SyncVar(hook="ChangeColor")]
    public Color color;

    public Light light;

    void ChangeColor(Color _color)
    {
        color = _color;
        light.color = _color;
    }

    void Start()
    {
        light.color = color;
    }


}
