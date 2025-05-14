using UnityEngine;

public class Boid : MonoBehaviour
{
    public Rigidbody rigidBody;
    public Renderer boidrenderer;
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.linearVelocity = Random.insideUnitSphere;
        GetComponent<Renderer>().material.SetColor("_BaseColor", Random.ColorHSV(0, 1, 0.5f, 1, 0.5f, 1));
    }
}
