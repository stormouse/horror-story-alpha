using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAround : MonoBehaviour {

    public Transform target;
    public float radius;
    public float variationR;
    public float variationY;
    public float spinSpeed;
    public float waveSpeed;

    private float originalY;

    private void Start()
    {
        originalY = transform.position.y;
    }

    private void Update()
    {
        if (!target) return;

        float deg = Time.time * spinSpeed;
        if (deg > 360.0f) deg -= 360.0f;

        float wav = Mathf.Sin(Time.time * waveSpeed);
        Vector3 dir = Quaternion.AngleAxis(deg, Vector3.up) * Vector3.right;
        Vector3 dest = target.position + dir * (radius + wav * variationR);
        dest.y = originalY + wav * variationY;

        transform.position = dest;
    }
}
