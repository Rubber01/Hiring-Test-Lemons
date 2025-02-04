using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public Transform cameraTransform; 
    private CharacterController controller;

    private bool isFreeLook = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        isFreeLook = Input.GetMouseButton(1); //tasto destro è premuto

        Vector3 moveDirection;

        if (isFreeLook)
        {
            // Se è in modalità VR (tasto destro premuto), segui la direzione del corpo
            moveDirection = transform.right * moveX + transform.forward * moveZ;
        }
        else
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0f; 
            right.y = 0f;
            moveDirection = (forward * moveZ + right * moveX).normalized;
        }

        controller.Move(moveDirection * speed * Time.deltaTime);
    }
}
