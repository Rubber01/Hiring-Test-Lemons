using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 100f;
    private float xRotation = 0f;
    private bool isFreeLook = false;

    [Header("Player & Ray Settings")]
    public Transform playerBody;
    public float rayLength = 10f; 
    public LayerMask interactableLayer;    
    public LayerMask buttonLayer;    

    [Header("Interazione Oggetti")]
    public float dragDistance = 2f; 
    public float minDragDistance = 1f; 
    public float maxDragDistance = 5f; 
    private GameObject currentHighlighted = null;  
    private GameObject draggedObject = null;         
    private Color originalColor;                     /

    [Header("Toggle Visibilità Raggio (Debug)")]
    private bool isRayVisible = false; 

    [Header("Canvas Manager")]
    private GameObject interactableBox;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (playerBody == null)
            playerBody = GameObject.FindGameObjectWithTag("Player").transform;
        interactableBox = GameObject.Find("InteractableObjectBox");
        interactableBox.SetActive(false);
    }

    void Update()
    {
        //premendo tasto destro
        isFreeLook = Input.GetMouseButton(1);

        // Toggle visibilità del raggio **debug** con tasto T
        if (Input.GetKeyDown(KeyCode.T))
        {
            isRayVisible = !isRayVisible;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (isFreeLook)
        {
            transform.localRotation = Quaternion.Euler(xRotation, transform.localEulerAngles.y + mouseX, 0f);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayLength, buttonLayer))
        {
            GameObject hitObj = hit.collider.gameObject;

            ButtonObjectScript button = hitObj.GetComponent<ButtonObjectScript>();
            if (button != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    button.OpenDoor();
                }
            }
        }else if (Physics.Raycast(ray, out hit, rayLength, interactableLayer))
        {
            GameObject hitObj = hit.collider.gameObject;

            
                
                if (currentHighlighted != hitObj)
                {
                    if (currentHighlighted != null && currentHighlighted != draggedObject)
                    {
                        ResetObjectColor(currentHighlighted);
                    }
                    currentHighlighted = hitObj;
                    SetObjectColor(currentHighlighted, Color.green);
                }
            
            else
            {
                if (currentHighlighted != hitObj)
                {
                    if (currentHighlighted != null && currentHighlighted != draggedObject)
                    {
                        ResetObjectColor(currentHighlighted);
                    }
                    currentHighlighted = hitObj;
                    SetObjectColor(currentHighlighted, Color.green);
                }
                interactableBox.SetActive(true);
                if (Input.GetMouseButtonDown(0))
                {
                    draggedObject = currentHighlighted;
                    Renderer rend = draggedObject.GetComponent<Renderer>();
                    if (rend != null)
                        originalColor = rend.material.color;
                }
            }
        }
        else
        {
            if (currentHighlighted != null && currentHighlighted != draggedObject)
            {
                ResetObjectColor(currentHighlighted);
                currentHighlighted = null;
                interactableBox.SetActive(false);
            }
        }

        if (draggedObject != null)
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (scrollInput != 0f)
            {
                dragDistance -= scrollInput; 
                dragDistance = Mathf.Clamp(dragDistance, minDragDistance, maxDragDistance);
            }
            draggedObject.transform.position = transform.position + transform.forward * dragDistance;
            if (Input.GetMouseButtonUp(0))
            {
                ResetObjectColor(draggedObject);
                draggedObject = null;
            }
        }

        if (isRayVisible)
        {
            Debug.DrawRay(transform.position, transform.forward * rayLength, Color.red);
        }
    }

    void SetObjectColor(GameObject obj, Color col)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = col;
        }
    }

    void ResetObjectColor(GameObject obj)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = Color.white;
        }
    }
}
