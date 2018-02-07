using UnityEngine;

public class CapsulePlayerControl : MonoBehaviour {

    public float moveSpeed = 2.0f;

    private Animator _animator;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Move(h, v);
    }

    void Move(float h, float v)
    {
        Vector3 movement = new Vector3(h, 0, v) * moveSpeed;
        if (movement.magnitude > moveSpeed)
            movement = movement.normalized * moveSpeed;
        if (movement.magnitude > moveSpeed * 0.3f)
        {
            if (_animator) _animator.SetBool("isWalking", true);
            GetComponent<Rigidbody>().MoveRotation(Quaternion.LookRotation(movement));
        }
        else
        {
            if (_animator) _animator.SetBool("isWalking", false);
        }
        GetComponent<Rigidbody>().MovePosition(transform.position + movement * Time.deltaTime);
    }

}
