using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;


public class PlayerInteraction : MonoBehaviour
{
    public TextMeshProUGUI InteractTMP;
    private bool _canInteract;
    private bool _isCollidingWithInteractable;
    private bool _isGrabbing;
    public Camera PlayerCam;
    public float InteractDistance = 10f;

    public InputActionAsset Actions;

    private void Awake()
    {
        HookupInputs();
    }

    private void HookupInputs()
    {
        Actions.FindActionMap(InputMap.DEFAULT_CONTROL_MAP_KEY).FindAction(InputMap.INTERACT_CONTROL_INPUT_KEY).performed += InteractButtonPressed;
    }

    private void Start()
    {
        TrySetCanInteract(false);
    }

    void Update()
    {
        TestInteraction();
    }

    private void TestInteraction()
    {
        // Cast a ray from the center of the screen
        Ray ray = PlayerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, InteractDistance))
        {
            // If the ray hits a collider, you can do something here
            Debug.Log("Hit: " + hit.collider.gameObject.name);

            if (hit.collider.CompareTag("Interactable"))
            {
                // Draw a debug line in the scene, color it red
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red);

                TrySetCanInteract(true);
            }
            else
            {
                TrySetCanInteract(false);
            }

        }
        else
        {
            // If the ray doesn't hit anything, draw the debug line, color it green
            Debug.DrawRay(ray.origin, ray.direction * InteractDistance, Color.green);
            TrySetCanInteract(false);
        }
    }

    private void TrySetCanInteract(bool canInteract)
    {
        if (_isCollidingWithInteractable || !canInteract)
        {
            _canInteract = canInteract;
            InteractTMP.enabled = canInteract;
        }
    }

    public void InteractButtonPressed(InputAction.CallbackContext context)
    {
        Debug.Log("Interact button pressed");
        if (!_isGrabbing)
        {
            TryGrab();
        }
        else
        {
            Drop();
        }
    }

    public void TryGrab()
    {
        if (!_canInteract)
        {
            Debug.Log("Nothing to grab!");
        }
        else
        {
            Debug.Log("You just Grabbed something!");
            _isGrabbing = true;
        }

    }

    public void Drop()
    {
        Debug.Log("You just Dropped something!");
        _isGrabbing = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Interactable"))
        {
            _isCollidingWithInteractable = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            _isCollidingWithInteractable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            TrySetCanInteract(false);
            _isCollidingWithInteractable = false;
        }
    }
    void OnEnable()
    {
        Actions.FindActionMap(InputMap.DEFAULT_CONTROL_MAP_KEY).Enable();
    }
    void OnDisable()
    {
        Actions.FindActionMap(InputMap.DEFAULT_CONTROL_MAP_KEY).Disable();
    }
}
