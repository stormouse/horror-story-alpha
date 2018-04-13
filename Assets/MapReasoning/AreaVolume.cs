using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaVolume : MonoBehaviour {

    public string areaName;


    private void OnDrawGizmos()
    {
        var box = GetComponent<BoxCollider>();
        var original = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Vector3.zero, box.size);
        Gizmos.matrix = original;
    }
}
