using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public Transform cameraTransform; 

    private bool isFreeLook = false;

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        //tasto destro 
        isFreeLook = Input.GetMouseButton(1);

        Vector3 moveDirection;

        if (isFreeLook)
        {
            // In modalità free look il movimento segue la direzione del corpo (transform)
            moveDirection = transform.right * moveX + transform.forward * moveZ;
        }
        else
        {
            // In modalità FPS il movimento segue la direzione della camera (senza includere la componente verticale)
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0f;
            right.y = 0f;
            moveDirection = (forward * moveZ + right * moveX).normalized;
        }

        transform.position += moveDirection * speed * Time.deltaTime;
    }
}
