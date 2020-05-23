using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteract : MonoBehaviour
{
    public bool isOpen;
    public GameObject OpenGate;
    public GameObject ClosedGate;
    public Collider2D GateCollider;

    bool canInteract = true;
    readonly WaitForSeconds shortWait = new WaitForSeconds(0.1f);

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerControl playerControl) && canInteract)
        {
            if (Input.GetKey(KeyCode.Return))
            {
                ToggleGate();
                StartCoroutine(InteractionDelay());
            }
        }
    }
    // If the player leaves the trigger area while still holding enter
    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.gameObject.TryGetComponent(out PlayerControl playerControl))
    //    {
    //        if (Input.GetKey(KeyCode.Return))
    //        {
    //            ToggleGate();
    //            StartCoroutine(InteractionDelay());
    //        }
    //    }
    //}

    void ToggleGate ()
    {
        if (isOpen)
        {
            // Close the gate
            ClosedGate.SetActive(true);
            OpenGate.SetActive(false);
            GateCollider.enabled = true;
            isOpen = false;
        }
        else
        {
            // Open the gate
            ClosedGate.SetActive(false);
            OpenGate.SetActive(true);
            GateCollider.enabled = false;
            isOpen = true;
        }
    }
    // Prevent accidentally repeating interactions by disabling for 0.1 seconds
    IEnumerator InteractionDelay ()
    {
        canInteract = false;

        yield return shortWait;

        canInteract = true;
    }
}
