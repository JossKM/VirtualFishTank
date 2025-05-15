using UnityEngine;

public class Boid : MonoBehaviour
{
    public float limitX = 1.4f;
    public float limitY = 1.4f;
    public float limitZ = 0.9f;
    public float turnSpeed = 3f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 pos = transform.position;
        Vector3 vel = rb.linearVelocity;

        Vector3 steering = Vector3.zero;

        if (pos.x > limitX) steering.x = -1;
        if (pos.x < -limitX) steering.x = 1;
        if (pos.y > limitY) steering.y = -1;
        if (pos.y < 0f) steering.y = 1;
        if (pos.z > limitZ) steering.z = -1;
        if (pos.z < -limitZ) steering.z = 1;

        if (steering != Vector3.zero)
        {
            Vector3 desired = (vel.normalized + steering.normalized).normalized;
            rb.linearVelocity = desired * vel.magnitude;
        }

        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            Mathf.Clamp(rb.linearVelocity.y, -0.2f, 0.2f),
            rb.linearVelocity.z
        );

       
        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(rb.linearVelocity.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
        }
    }
}
