using System.Collections.Generic;
using UnityEngine;

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
    public int numBoidsToSpawn = 10;
    public List<Boid> boids = null;

    private void Start()
    {
        //Spawn boids
        for(int i = 0; i < numBoidsToSpawn; i++) 
        {
            Vector3 position = new Vector3(Random.Range(-1.4f, 1.4f), Random.Range(0f, 1.4f), Random.Range(-0.9f, 0.9f));
            Quaternion rotation = Random.rotation;

            GameObject spawnedBoid = Instantiate(boidPrefab, position, rotation);
            boids.Add(spawnedBoid.GetComponent<Boid>());

            spawnedBoid.GetComponent<Renderer>().material.SetColor("_BaseColor", Random.ColorHSV(0, 1, 0f, 1f, 0.5f, 1f));

            spawnedBoid.GetComponent<Rigidbody>().linearVelocity = Random.onUnitSphere * 0.3f;

            spawnedBoid.transform.localScale *= Random.Range(0.9f, 3f);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
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
    }
}
