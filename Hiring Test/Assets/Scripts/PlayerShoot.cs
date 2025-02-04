using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject projectilePrefab; 
    public float shootForce = 20f; 
    public LayerMask interactableLayer; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position + transform.forward * 1.5f, transform.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = transform.forward * shootForce;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous; 
            Debug.Log("Proiettile sparato con velocità: " + rb.linearVelocity);
        }
        else
        {
            Debug.LogError("Il prefab del proiettile non ha un Rigidbody!");
        }
    }
}


