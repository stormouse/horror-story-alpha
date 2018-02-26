using UnityEngine;

public class SoftAttachment : MonoBehaviour {
    public Transform attachTo = null;
    public Vector3 offset = Vector3.zero;
    public bool followRotation = false;

    private void Update()
    {
        if (attachTo != null)
        {
            transform.position = attachTo.transform.position + offset;
            if (followRotation)
            {
                transform.rotation = attachTo.transform.rotation;
            }
        }

    }

}
