using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;


public class PlayerInteraction : MonoBehaviour
{
    public TextMeshProUGUI InteractTMP;
    private bool _canInteract;
    private bool _canGrab;
    private bool _isCollidingWithInteractable;
    private bool _isCollidingWithGrabbable;
    private bool _isGrabbing;
    public Camera PlayerCam;
    public float InteractDistance = 10f;

    public InputActionAsset Actions;

    public Transform GrabParent;

    private GameObject _interactableObject;
    private GameObject _grabbableObject;
    public float GrabFollowSpeed = 10f;

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
        HoldGrabbbableInPlace();
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

            if (hit.collider.CompareTag("Grabbable"))
            {
                // Draw a debug line in the scene, color it red
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red);

                TrySetCanGrab(true);
            }
            else
            {
                TrySetCanGrab(true);
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
            InteractTMP.text = "INTERACT";
        }
    }

    private void TrySetCanGrab(bool canGrab)
    {
        if (_isCollidingWithGrabbable || !canGrab)
        {
            _canGrab = canGrab;
            InteractTMP.enabled = canGrab;
            InteractTMP.text = "GRAB";
        }
    }

    public void InteractButtonPressed(InputAction.CallbackContext context)
    {
        Debug.Log("Interact button pressed");
        if (_isGrabbing)
        {
            Drop();
        }
        else if (!_isGrabbing && _canGrab)
        {
            Grab();
        }

        if (_canInteract)
        {
            Interact();
        }
    }

    private void Interact()
    {
        Debug.Log("You just interacted something!");
    }

    private void Grab()
    {
        Debug.Log("You just Grabbed something!");
        _isGrabbing = true;
        _grabbableObject.transform.SetParent(GrabParent);
        _grabbableObject.transform.position = GrabParent.position;

        Rigidbody grabbableRB = _grabbableObject.GetComponent<Rigidbody>();

        if (grabbableRB != null) 
        {
            grabbableRB.useGravity = false;
        }
    }

    private void HoldGrabbbableInPlace()
    {
        if (_grabbableObject != null && _isGrabbing)
        {
            _grabbableObject.transform.position = Vector3.MoveTowards(_grabbableObject.transform.position, GrabParent.transform.position, Time.deltaTime * GrabFollowSpeed);
        }
    }

    public void Drop()
    {
        Debug.Log("You just Dropped something!");
        _isGrabbing = false;

        _grabbableObject.transform.SetParent(null); //sets back to root scene

        Rigidbody grabbableRB = _grabbableObject.GetComponent<Rigidbody>();

        if (grabbableRB != null)
        {
            grabbableRB.useGravity = true;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Interactable"))
        {
            _isCollidingWithInteractable = true;
        }
        if (other.CompareTag("Grabbable"))
        {
            _isCollidingWithGrabbable = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            _isCollidingWithInteractable = true;
        }
        if (other.CompareTag("Grabbable"))
        {
            _isCollidingWithGrabbable = true;
            _grabbableObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            TrySetCanInteract(false);
            _isCollidingWithInteractable = false;
        }
        if (other.CompareTag("Grabbable"))
        {
            TrySetCanGrab(false);
            _isCollidingWithGrabbable = false;
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
