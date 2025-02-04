using UnityEngine;

public class ButtonObjectScript : MonoBehaviour
{
    public Animator doorAnimator; 
    public AudioSource doorSound; 
    private bool check=false;
    public DoorController doorController;
    public void OpenDoor()
    {
        Debug.Log("dentro");
        if (!check)
        {
            if (doorAnimator != null)
            {
                doorAnimator.enabled = true;
                doorAnimator.Play("ButtonPressed");
            }

            if (doorSound != null)
            {
                doorSound.Play(); 
            }
            check = true;
            doorController.isOpen = true;
        }
        

    }
    private void Start()
    {
        doorAnimator = GetComponent<Animator>();
        doorAnimator.enabled = false;
    }

}
