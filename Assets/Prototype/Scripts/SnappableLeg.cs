using UnityEngine;


public class SnappableLeg : MonoBehaviour, ISnappable
{
    public Transform legTransform;

    public string ParentName()
    {
        return SnappableParts.Leg;
    }

    public Transform ParentTransform()
    {
        return legTransform;
    }
}
