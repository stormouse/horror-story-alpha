using UnityEngine;


public static class SnappableParts
{
    public static readonly string Leg = "Leg";
    public static readonly string Body = "Body";
}


public interface ISnappable
{
    string ParentName();
    Transform ParentTransform();
}