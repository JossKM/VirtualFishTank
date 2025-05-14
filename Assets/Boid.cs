using UnityEngine;

public class Boid : MonoBehaviour
{
    private GameObject targetObject;
    private Rigidbody rigidBody;

    public float speedMax = 2;
    public float accelMax = 3;

    private void Start()
    {
        targetObject = GameObject.Find("target");
        rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //Get displacement vector to target
        Vector3 toTarget = targetObject.transform.position - transform.position;

        //Normalize
        Vector3 toTargetNormalized = toTarget.normalized;

        //Determine acceleration
        Vector3 acceleration = toTargetNormalized * accelMax;

        rigidBody.linearVelocity += acceleration * Time.fixedDeltaTime;

        //Enforce a top speed
        rigidBody.linearVelocity = Vector3.ClampMagnitude(rigidBody.linearVelocity, speedMax);

        //Orient toward velocity
        transform.forward = rigidBody.linearVelocity;
    }
}
