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

    //Seek a random point ahead in a sphere ahead of the boid
    public Vector3 Wander(float distance, float radius, float acceleration)
    {
        Vector3 sphereCenter = transform.position + transform.forward * distance;
        Vector3 randomPoint = sphereCenter + (radius * Random.insideUnitSphere);

        return Seek(randomPoint, acceleration);
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

    public Vector3 Arrive(Vector3 target, float acceleration, float arriveInnerRadius, float arriveOuterRadius)
    {
        //Get displacement vector to target
        Vector3 toTarget = target - transform.position;

        //Get distance to target
        float distance = toTarget.magnitude;

        //Get direction to target
        Vector3 toTargetNormalized = toTarget.normalized; // careful not to divide by zero...

        //Get desired velocity
        Vector3 desiredVelocity = toTargetNormalized * speedMax;

        float distancePercentage = 1;

        //scale desired velocity based on distance percentage
        if (distance < arriveInnerRadius)
        {
            desiredVelocity = Vector3.zero;
            DebugDrawing.DrawCircle(transform.position, Quaternion.Euler(90, 0, 0), arriveInnerRadius, 8, Color.red, Time.fixedDeltaTime);
        } else if(distance < arriveOuterRadius)
        {
            //express distance as a percentage of the way to the outer radius...
            // example:
            // arriveOuterRadius = 10
            // distance = 8
            // distancePercentage = 8/10 or 80 percent
            distancePercentage = distance / arriveOuterRadius; //
            desiredVelocity *= distancePercentage;
            DebugDrawing.DrawCircleDotted(transform.position, Quaternion.Euler(90,0,0), arriveOuterRadius, 16, 0.01f, 0.01f, Color.red, Time.fixedDeltaTime);
        }

        Vector3 deltaVel = desiredVelocity - rigidBody.linearVelocity;

        Vector3 accel = deltaVel.normalized * acceleration * distancePercentage; // Accelerate in the direction of the required change in velocity
        return accel;
    }

    public Vector3 ObstacleAvoidance(float lookaheadDistance, float acceleration)
    {
        Vector3 accelOut = Vector3.zero;

        //Whiskers... left and right
        Ray whiskerLeft = new Ray(transform.position,  Quaternion.AngleAxis(-20, transform.up) * transform.forward);
        Ray whiskerRight = new Ray(transform.position, Quaternion.AngleAxis(20, transform.up) * transform.forward);

        //Whiskers... above and below
        Ray whiskerUp = new Ray(transform.position, Quaternion.AngleAxis(-20, transform.right) * transform.forward);
        Ray whiskerDown = new Ray(transform.position, Quaternion.AngleAxis(20, transform.right) * transform.forward);

        RaycastHit hitInfoLeft;
        RaycastHit hitInfoRight;
        RaycastHit hitInfoUp;
        RaycastHit hitInfoDown;

        bool didHitLeft = Physics.Raycast(whiskerLeft, out hitInfoLeft, lookaheadDistance);
        if (didHitLeft)
        {
            //Turn right
            accelOut = transform.right * acceleration;
            Debug.DrawLine(transform.position, hitInfoLeft.point, Color.red) ;
        } else
        {
            Debug.DrawRay(transform.position, whiskerLeft.direction * lookaheadDistance, Color.yellow);
        }

        bool didHitRight = Physics.Raycast(whiskerRight, out hitInfoRight, lookaheadDistance);
        if (didHitRight)
        {
            //Turn left
            accelOut = -transform.right * acceleration;
            Debug.DrawLine(transform.position, hitInfoRight.point, Color.red);
        }
        else
        {
            Debug.DrawRay(transform.position, whiskerRight.direction * lookaheadDistance, Color.yellow);
        }

        bool didHitUp = Physics.Raycast(whiskerLeft, out hitInfoUp, lookaheadDistance);
        if (didHitLeft)
        {
            //Turn down
            accelOut = -transform.up * acceleration;
            Debug.DrawLine(transform.position, hitInfoUp.point, Color.red);
        }
        else
        {
            Debug.DrawRay(transform.position, whiskerUp.direction * lookaheadDistance, Color.yellow);
        }

        bool didHitDown = Physics.Raycast(whiskerLeft, out hitInfoDown, lookaheadDistance);
        if (didHitLeft)
        {
            //Turn up
            accelOut = transform.up * acceleration;
            Debug.DrawLine(transform.position, hitInfoDown.point, Color.red);
        }
        else
        {
            Debug.DrawRay(transform.position, whiskerDown.direction * lookaheadDistance, Color.yellow);
        }

        if(didHitLeft && didHitRight && didHitUp && didHitDown)
        {
            //Turn around!
            accelOut = -transform.forward * acceleration;

            Debug.DrawLine(transform.position, transform.forward, Color.red);
        }

        return accelOut;
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
        if(rigidBody.linearVelocity.sqrMagnitude > 0.01f)
        {
            transform.forward = Vector3.Lerp(transform.forward, rigidBody.linearVelocity, 0.7f);
        }
    }
}
