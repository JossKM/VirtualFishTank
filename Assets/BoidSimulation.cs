using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void SetControlMode(int mode)
    {
        controlMode = (ControlMode)mode;
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
    }

    private void SpawnFood()
    {
        Instantiate(foodPrefab, targetObject.transform.position, Random.rotation); //Instantiate a copy of a prefab for food
    }

}
