using UnityEngine;

public class Boid : MonoBehaviour
{
    public Rigidbody rigidBody;

    public float speedMax = 2;
    public float accelMax = 3;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, rigidBody.linearVelocity, Color.red);
    }

    public Vector3 Seek(Vector3 target, float acceleration)
    {
        //Get displacement vector to target
        Vector3 toTarget = target - transform.position;

        //Normalize to get the direction to the target
        Vector3 toTargetNormalized = toTarget.normalized;

        //Determine acceleration with a given magnitude
        Vector3 accel = toTargetNormalized * acceleration;

        return accel;
    }

    public Vector3 Pursue(Vector3 target, float acceleration, float desiredSpeed)
    {
        //Get displacement vector to target
        Vector3 toTarget = target - transform.position;

        //Normalize to get the direction to the target
        Vector3 toTargetNormalized = toTarget.normalized;

        //Determine Desired Velocity -- toward target!
        Vector3 desiredVelocity = toTargetNormalized * desiredSpeed;

        //Determine required change in velocity between current velocity and desired
        Vector3 deltaVel = desiredVelocity - rigidBody.linearVelocity;

        Vector3 accel = deltaVel.normalized * acceleration; // Accelerate in the direction of the required change in velocity
        return accel;
    }


    private void FixedUpdate()
    {
        float speed = rigidBody.linearVelocity.magnitude;

        if (speed > speedMax) 
        {
            //Enforce a top speed
            rigidBody.linearVelocity = rigidBody.linearVelocity * speedMax / speed;
        }

        //Orient toward velocity
        transform.forward = rigidBody.linearVelocity;
    }
}
