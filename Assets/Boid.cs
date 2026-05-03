using UnityEngine;

public class Boid : MonoBehaviour
{
    public Rigidbody rigidBody;

    public float speedMax = 2;
    public float accelMax = 3;

    public Vector3 currentLinearAcceleration = Vector3.zero;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, rigidBody.linearVelocity, Color.red);
    }

    private void FixedUpdate()
    {
        float speed = rigidBody.linearVelocity.magnitude;

        if (speed > speedMax)
        {
            //Enforce a top speed
            rigidBody.linearVelocity = rigidBody.linearVelocity * speedMax / speed;
        }

        //Orient toward velocity... but not if it is close to zero.
        if (rigidBody.linearVelocity.sqrMagnitude > 0.01f)
        {
            transform.forward = Vector3.Lerp(transform.forward, rigidBody.linearVelocity, 0.7f);
        }
    }

    public Vector3 Seek(Vector3 target, float acceleration)
    {
        throw new System.NotImplementedException();
    }

    public Vector3 Pursue(Vector3 target, float acceleration, float desiredSpeed)
    {
        throw new System.NotImplementedException();
    }

    public Vector3 Arrive(Vector3 target, float acceleration, float arriveInnerRadius, float arriveOuterRadius)
    {
        throw new System.NotImplementedException();
    }

    public Vector3 ObstacleAvoidance(float lookaheadDistance, float acceleration)
    {
        throw new System.NotImplementedException();
    }
}
