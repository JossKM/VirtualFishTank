using System.Collections;
using UnityEngine;

public class Food : MonoBehaviour
{
    float lifetime = 10f; // time before decay
    Vector3 originalScale;
    float foodAmount = 1; // how much "food" is left in this piece
    ParticleSystem crumbs;

    //Save scale so we can scale according to food amount
    private void Start()
    {
        crumbs = GetComponent<ParticleSystem>();
        originalScale = transform.localScale;

        StartCoroutine(Dissolve());
    }

    private void Bite(float amount = 0.05f)
    {
        foodAmount -= 0.05f; // Take some hit
        transform.localScale = originalScale * foodAmount;

        if (foodAmount <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //When a boid collides with food, reduce it by a bit.
        if (collision.gameObject.GetComponent<Boid>() != null)
        {
            Bite();
            crumbs.Play(); //Play visual effect.
        }
    }

    IEnumerator Dissolve()
    {
        yield return new WaitForSeconds(lifetime);

        while (foodAmount > 0)
        {
            yield return new WaitForSeconds(0.1f);
            Bite(0.01f);
        }
    }
}
