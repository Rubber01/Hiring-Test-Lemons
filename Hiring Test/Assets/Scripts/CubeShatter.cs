using UnityEngine;

public class CubeShatter : MonoBehaviour
{
    public GameObject shatteredPrefab; 
    public float explosionForce = 20f; 
    public float explosionRadius = 2f;
    public int numberOfFragments = 10; 
    public float forceVariation = 5f; 

    public void Shatter()
    {
        if (shatteredPrefab != null)
        {
            Vector3 reducedScale = transform.localScale / 8f;

            for (int i = 0; i < numberOfFragments; i++)
            {
                GameObject shatteredObject = Instantiate(shatteredPrefab, transform.position, transform.rotation);

                shatteredObject.transform.localScale = reducedScale;

                Rigidbody rb = shatteredObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 randomDirection = Random.onUnitSphere + new Vector3(Random.Range(-forceVariation, forceVariation),
                                                                               Random.Range(-forceVariation, forceVariation),
                                                                               Random.Range(-forceVariation, forceVariation)); 
                    rb.AddForce(randomDirection * explosionForce, ForceMode.Impulse); 
                }
            }
        }

        Destroy(gameObject); 
    }
}
