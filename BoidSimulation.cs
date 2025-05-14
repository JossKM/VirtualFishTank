using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using UnityEngine;

public class BoidSimulationControl : MonoBehaviour
{
    public GameObject boidprefab;
    public int boidsToSpwan = 10;
    public List<Boid> boids = null;
    public Rigidbody rigidBody;
    public enum ControlMode
    {
        Seek,
        Pursue,
        Food,
        Obstacle
    }
    public ControlMode controlMode = ControlMode.Seek;
    private void Start()
    {
        boids = new List<Boid>();

        for (int i = 0; i < boidsToSpwan; i++)
        {
            GameObject newBoid = Instantiate(boidprefab, new Vector3(Random.Range(-1.4f, 1.4f), Random.Range(0, 0.7f), Random.Range(-0.4f, 0.4f)), Random.rotation);
            boids.Add(newBoid.GetComponent<Boid>());
        }
    }
    
   
    private void Update()
    {
        AlignToVelocity();
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            controlMode = ControlMode.Seek;
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            controlMode = ControlMode.Pursue;
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            controlMode = ControlMode.Food;
        }
        if(Input.GetKeyUp(KeyCode.Alpha4))
        {
            controlMode = ControlMode.Obstacle;
        }
    }
    public void AlignToVelocity()
    {
        transform.forward = Vector3.RotateTowards(transform.forward, rigidBody.linearVelocity.normalized, Mathf.Deg2Rad * 1800 * Time.deltaTime, 100);   
    }
}



