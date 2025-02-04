using UnityEngine;

public class Projectile : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Proiettile ha colpito: " + collision.gameObject.name);

        CubeShatter cubeShatter = collision.gameObject.GetComponent<CubeShatter>();
        if (cubeShatter != null)
        {
            cubeShatter.SendMessage("Shatter");
        }
        else
        {
            Debug.LogWarning("Il cubo non ha lo script CubeShatter!");
        }

        Destroy(gameObject); 
    }
}
