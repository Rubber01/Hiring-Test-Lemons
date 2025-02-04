using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    public float dragDistance = 3.5f; 
    public float minDragDistance = 1f; 
    public float maxDragDistance = 5f; 
    private GameObject currentHighlighted = null;
    private GameObject draggedObject = null;       

    // Dizionario per salvare il colore originale di ogni oggetto
    private Dictionary<GameObject, Color> originalColors = new Dictionary<GameObject, Color>();

    [Header("Toggle Visibilità Raggio (Debug)")]
    private bool isRayVisible = false; 

    [Header("Canvas Manager")]
    private GameObject interactableBox;

    [Header("Teleport Settings")]
    public float teleportOffset = 1f; 

    private LineRenderer teleportLineRenderer;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (playerBody == null)
            playerBody = GameObject.FindGameObjectWithTag("Player").transform;
        interactableBox = GameObject.Find("InteractableObjectBox");
        if (interactableBox != null)
            interactableBox.SetActive(false);

        // Configurazione del LineRenderer per il teleport ray
        teleportLineRenderer = gameObject.AddComponent<LineRenderer>();
        teleportLineRenderer.startWidth = 0.05f;
        teleportLineRenderer.endWidth = 0.05f;
        teleportLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        teleportLineRenderer.startColor = Color.blue;
        teleportLineRenderer.endColor = Color.blue;
        teleportLineRenderer.positionCount = 2;
        teleportLineRenderer.enabled = false;
    }

    void Update()
    {
        // modalità free look tasto destro
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

        // --- Teletrasporto ---
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray teleportRay = new Ray(transform.position, transform.forward);
            RaycastHit teleportHit;

            teleportLineRenderer.enabled = true;
            teleportLineRenderer.SetPosition(0, transform.position);

            Vector3 newPosition;

            if (Physics.Raycast(teleportRay, out teleportHit, rayLength))
            {
                teleportLineRenderer.SetPosition(1, teleportHit.point);
                Collider col = teleportHit.collider;
                if (col != null)
                {
                    float newY = col.bounds.max.y + teleportOffset;
                    newPosition = new Vector3(teleportHit.point.x, newY, teleportHit.point.z);
                }
                else
                {
                    newPosition = teleportHit.point;
                }
            }
            else
            {
                Vector3 endPoint = transform.position + transform.forward * rayLength;
                teleportLineRenderer.SetPosition(1, endPoint);

                Ray groundRay = new Ray(endPoint + Vector3.up * 1f, Vector3.down);
                RaycastHit groundHit;
                if (Physics.Raycast(groundRay, out groundHit, 10f))
                {
                    newPosition = new Vector3(endPoint.x, groundHit.point.y, endPoint.z);
                }
                else
                {
                    newPosition = endPoint;
                }
            }

            Debug.Log("Teletrasporto a: " + newPosition);

            CharacterController cc = playerBody.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                playerBody.position = newPosition;
                cc.enabled = true;
            }
            else
            {
                playerBody.position = newPosition;
            }

            StartCoroutine(HideTeleportRay());
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
        }
        else if (Physics.Raycast(ray, out hit, rayLength, interactableLayer))
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

            if (interactableBox != null)
                interactableBox.SetActive(true);

            if (Input.GetMouseButtonDown(0))
            {
                draggedObject = currentHighlighted;
                Renderer rend = draggedObject.GetComponent<Renderer>();
                if (rend != null && !originalColors.ContainsKey(draggedObject))
                {
                    originalColors[draggedObject] = rend.material.color;
                }
            }
        }
        else
        {
            if (currentHighlighted != null && currentHighlighted != draggedObject)
            {
                ResetObjectColor(currentHighlighted);
                currentHighlighted = null;
                if (interactableBox != null)
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
                if (draggedObject != null)
                {
                    ResetObjectColor(draggedObject);
                    draggedObject = null;
                }
            }

        }

        if (isRayVisible)
        {
            Debug.DrawRay(transform.position, transform.forward * rayLength, Color.red);
        }
    }

    IEnumerator HideTeleportRay()
    {
        yield return new WaitForSeconds(0.5f);
        teleportLineRenderer.enabled = false;
    }

    void SetObjectColor(GameObject obj, Color col)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null)
        {
            if (!originalColors.ContainsKey(obj))
            {
                originalColors[obj] = rend.material.color;
            }

            rend.material = new Material(rend.material);
            rend.material.color = col;
        }
    }


    void ResetObjectColor(GameObject obj)
    {
        if (obj == null) return;

        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null)
        {
            if (originalColors.ContainsKey(obj))
            {
                rend.material.color = originalColors[obj];
                originalColors.Remove(obj); 
            }
        }
    }

}
