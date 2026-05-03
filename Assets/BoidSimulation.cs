using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class BoidSimulationControl : MonoBehaviour
{
    //To select and switch what the mouse buttons do for Assignment 1.
    public enum ControlMode
    {
        Seek,
        Pursue,
        Food,
        Obstacle
    }

    //Which mode are we in (selects what mouse buttons do)
    public ControlMode controlMode = ControlMode.Seek;

    public GameObject boidPrefab = null;
    public GameObject foodPrefab = null;
    public GameObject obstaclePrefab = null;
    public GameObject splashPrefab = null;
    public GameObject targetObject = null;
    public int numBoidsToSpawn = 10;
    public float obstacleAvoidanceSpeedScale = 0.5f;
    public float obstacleAvoidanceMinRange = 0.02f;
    public List<Boid> boids = null;

    private void Start()
    {
        targetObject = GameObject.Find("target");

        //Spawn boids
        for (int i = 0; i < numBoidsToSpawn; i++)
        {
            Vector3 position = new Vector3(Random.Range(-1.4f, 1.4f), Random.Range(0f, 1.4f), Random.Range(-0.9f, 0.9f));
            Quaternion rotation = Random.rotation;

            GameObject spawnedBoid = Instantiate(boidPrefab, position, rotation); //Instantiate a copy of the prefab

            Boid boidComponent = spawnedBoid.GetComponent<Boid>(); // Get boid component from the newly instantiated copy
            //Randomize properties of each boid within a range!
            boidComponent.speedMax = Random.Range(0.3f, 1.2f);
            boidComponent.accelMax = Random.Range(0.5f, 1.5f);

            boids.Add(boidComponent); // Add the new object to a list we can use later

            spawnedBoid.GetComponent<Renderer>().material.SetColor("_BaseColor", Random.ColorHSV(0, 1, 0f, 1f, 0.5f, 1f));

            spawnedBoid.GetComponent<Rigidbody>().linearVelocity = Random.onUnitSphere * 0.3f;

            spawnedBoid.transform.localScale *= Random.Range(0.9f, 3f);
        }
    }

    public void SetControlMode(int mode)
    {
        controlMode = (ControlMode)mode;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            controlMode = ControlMode.Seek;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            controlMode = ControlMode.Pursue;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            controlMode = ControlMode.Food;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            controlMode = ControlMode.Obstacle;
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ResetSim();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (controlMode == ControlMode.Food)
            {
                SpawnFood();

            }
            else if (controlMode == ControlMode.Obstacle)
            {
                SpawnObstacle();
            }
        }
    }

    void ResetSim()
    {
        SceneManager.LoadScene(0);
    }

    private void SpawnObstacle()
    {
        Instantiate(obstaclePrefab, targetObject.transform.position, Random.rotation); //Instantiate a copy of a prefab for food
    }

    private void FixedUpdate()
    {
        //reset acceleration vectors to zero
        for (int i = 0; i < boids.Count; i++) // For each boid...
        {
            boids[i].currentLinearAcceleration = Vector3.zero;
        }

        ObstacleAvoidanceBehaviour();

        FoodArrivalBehaviour();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo; // Will store information about the intersection if any
        bool didHit = Physics.Raycast(ray, out hitInfo, 100); // Raycast will return false if it misses, true if it hits,
                                                              // while it will also fill in the passed parameter hitInfo
                                                              // with useful information about what it hit

        if (didHit)
        {
            targetObject.transform.position = hitInfo.point; // Teleport an object to the point of intersection from the raycast

            //Spawn ripples for visual indication of mouse click
            if (Input.GetMouseButton(1) || Input.GetMouseButton(0))
            {
                GameObject instantiated = Instantiate(splashPrefab, targetObject.transform.position, Quaternion.identity);
                Destroy(instantiated, 1.0f);
            }
        
        }

        switch (controlMode)
        {
            case ControlMode.Seek:
                {
                    SeekModeControl();
                    break;
                }

            case ControlMode.Pursue:
                {
                    PursueModeControl();
                    break;
                }
        }

        //If not doing anything, let the boids wander
        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            WanderBehaviour();
        }

        

        //enforce acceleration limits and apply velocity change
        for (int i = 0; i < boids.Count; i++) // For each boid...
        {
            boids[i].currentLinearAcceleration = Vector3.ClampMagnitude(boids[i].currentLinearAcceleration, boids[i].accelMax);
            boids[i].rigidBody.linearVelocity += boids[i].currentLinearAcceleration * Time.fixedDeltaTime;
        }
    }

    private void SpawnFood()
    {
        Instantiate(foodPrefab, targetObject.transform.position, Random.rotation); //Instantiate a copy of a prefab for food
    }

    public void ObstacleAvoidanceBehaviour()
    {
        for (int i = 0; i < boids.Count; i++) // For each boid...
        {
            Vector3 accel = boids[i].ObstacleAvoidance(obstacleAvoidanceMinRange + obstacleAvoidanceSpeedScale * boids[i].rigidBody.linearVelocity.magnitude, boids[i].accelMax);
            boids[i].currentLinearAcceleration += accel;
          //  Debug.DrawRay(boids[i].transform.position, accel, Color.green); // Draw acceleration
        }
    }

    public void WanderBehaviour()
    {
        for (int i = 0; i < boids.Count; i++) // For each boid...
        {
            Vector3 accel = boids[i].Wander(0.005f, 0.02f, boids[i].accelMax * 0.2f);
            boids[i].currentLinearAcceleration += accel;
           // Debug.DrawRay(boids[i].transform.position, accel, Color.green); // Draw acceleration
        }
    }

    public void FoodArrivalBehaviour()
    {
        //Have all fish seek food
        for (int i = 0; i < boids.Count; i++) // For each boid...
        {
            float foodSeekRadius = 0.95f;
            Collider[] colliders = Physics.OverlapSphere(boids[i].transform.position, foodSeekRadius);

            Food closestFood = null;
            float closestFoodDistance = float.PositiveInfinity;
            foreach (Collider collider in colliders) // for each collider in this radius...
            {
                Food food = collider.GetComponent<Food>(); // Check if it has a Food component
                if (food != null) // If it has this component, it will not be null. If not null, we can seek the food.
                {
                    float distanceToFood = Vector3.Distance(food.transform.position, boids[i].transform.position);
                    if(distanceToFood < closestFoodDistance)
                    {
                        closestFoodDistance = distanceToFood;
                        closestFood = food;
                    }
                }
            }
            
            //If there is a food nearby
            if(closestFood != null)
            {
                Vector3 accel = boids[i].Arrive(closestFood.transform.position, boids[i].accelMax, 0.03f, 0.3f);
                boids[i].currentLinearAcceleration += accel;
                Debug.DrawRay(boids[i].transform.position, accel, Color.green); // Draw acceleration
            }
        }
    }

    private void SeekModeControl()
    {
        for (int i = 0; i < boids.Count; i++) // For each boid...
        {
            //Call the Seek function on each boid to calculate an acceleration vector
            if (Input.GetMouseButton(0))
            {
                Vector3 accel = boids[i].Seek(targetObject.transform.position, boids[i].accelMax);
                boids[i].currentLinearAcceleration += accel;
                Debug.DrawRay(boids[i].transform.position, accel, Color.green); // Draw acceleration
            }
            else if (Input.GetMouseButton(1))
            {
                Vector3 accel = boids[i].Seek(targetObject.transform.position, boids[i].accelMax);
                boids[i].currentLinearAcceleration -= accel;
                Debug.DrawRay(boids[i].transform.position, accel, Color.green); // Draw acceleration
            }
        }
    }


    private void PursueModeControl()
    {
        for (int i = 0; i < boids.Count; i++) // For each boid...
        {
            //Call the Pursue function on each boid to calculate an acceleration vector

            if (Input.GetMouseButton(0))
            {
                Vector3 accel = boids[i].Pursue(targetObject.transform.position, boids[i].accelMax, boids[i].speedMax);
                boids[i].rigidBody.linearVelocity += accel * Time.fixedDeltaTime; // Apply acceleration
                Debug.DrawRay(boids[i].transform.position, accel, Color.green); // Draw acceleration
            }
            else if (Input.GetMouseButton(1))
            {
                Vector3 accel = boids[i].Pursue(targetObject.transform.position, boids[i].accelMax, boids[i].speedMax);
                boids[i].rigidBody.linearVelocity -= accel * Time.fixedDeltaTime; // Apply acceleration
                Debug.DrawRay(boids[i].transform.position, accel, Color.green); // Draw acceleration
            }
        }
    }
}
