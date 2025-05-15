using UnityEngine;

public class BoidSimulationControl : MonoBehaviour
{
  
    void Start()
    {
        for (int i = 0; i < numBoidsToSpawn; i++)
        {
            Vector3 position = new Vector3(
                Random.Range(-1.4f, 1.4f),
                Random.Range(0f, 1.4f),
                Random.Range(-0.9f, 0.9f)
            );

            Quaternion rotation = Random.rotation;

            GameObject spawnedBoid = Instantiate(boidPrefab, position, rotation);
            spawnedBoid.transform.localScale = Vector3.one * Random.Range(0.9f, 2f);

            Renderer rend = spawnedBoid.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material = new Material(rend.material);
                rend.material.color = Random.ColorHSV(0f, 1f, 0.6f, 1f, 0.6f, 1f);
            }

            Rigidbody rb = spawnedBoid.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = Random.onUnitSphere * Random.Range(0.5f, 1.2f);
        }
    }
}

