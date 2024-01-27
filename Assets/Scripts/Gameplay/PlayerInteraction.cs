using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;


public class PlayerInteraction : MonoBehaviour
{
    public TextMeshProUGUI InteractTMP;
    private bool _canInteract;
    private bool _isGrabbing;
    public Camera PlayerCam;
    public float InteractDistance = 10f;

    public InputActionAsset Actions;

    public Transform GrabParent;

    private GameObject _interactableObject;
    public float GrabFollowSpeed = 10f;

    private IGrabbable _heldObject;

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
        SetCanInteract(false);
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
        if (!_isGrabbing && Physics.Raycast(ray, out hit, InteractDistance))
        {
            // If the ray hits a collider, you can do something here
            //Debug.Log("Hit: " + hit.collider.gameObject.name);

            if (hit.collider.CompareTag("Interactable"))
            {
                // Draw a debug line in the scene, color it red
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red);
                _interactableObject = hit.transform.gameObject;
                string interactDescription = hit.transform.name;
                SetCanInteract(true, interactDescription);
            }
            else
            {
                _interactableObject = null;
                SetCanInteract(false);
            }
        }
        else
        {
            // If the ray doesn't hit anything, draw the debug line, color it green
            Debug.DrawRay(ray.origin, ray.direction * InteractDistance, Color.green);
            SetCanInteract(false);
        }
    }

    private void SetCanInteract(bool canInteract, string interactDescription = null)
    {
        _canInteract = canInteract;
        InteractTMP.enabled = canInteract;
        InteractTMP.text = interactDescription;
    }

    public void InteractButtonPressed(InputAction.CallbackContext context)
    {
        Debug.Log("Interact button pressed");
        if (_isGrabbing)
        {
            Drop();
        }

        if (_canInteract)
        {
            Interact();
        }
    }

    private void Interact()
    {
        var allScriptOnObject = _interactableObject.GetComponents<MonoBehaviour>();

        foreach(var script in allScriptOnObject) 
        {
            if(script is IInteractable)
            {
                var interactableScript = script as IInteractable;
                interactableScript.Interact();

                Debug.Log("You just interacted something!");
            }

            //TODO I don't like this workflow necessarily... grabbable objects should either be in charge of how they are grabbed, or "interactable" should be a class derived from monobehavior, i.e. the grabbing happens as part of "Interact()".
            if(script is IGrabbable)
            {
                var grabbableScript = script as IGrabbable;
                Grab(grabbableScript);
            }
        }
    }

    private void Grab(IGrabbable grabbable)
    {
        Debug.Log("You just Grabbed something!");
        _isGrabbing = true;
        _interactableObject.transform.SetParent(GrabParent);
        _interactableObject.transform.position = GrabParent.position;

        _heldObject = grabbable;
        grabbable.PickedUp();

        Rigidbody grabbableRB = _interactableObject.GetComponent<Rigidbody>();

        if (grabbableRB != null) 
        {
            grabbableRB.useGravity = false;
        }
    }

    private void HoldGrabbbableInPlace()
    {
        if (_interactableObject != null && _isGrabbing)
        {
            _interactableObject.transform.position = Vector3.MoveTowards(_interactableObject.transform.position, GrabParent.transform.position, Time.deltaTime * GrabFollowSpeed);
        }
    }

    public void Drop()
    {
        Debug.Log("You just Dropped something!");
        _isGrabbing = false;

        _interactableObject.transform.SetParent(null); //sets back to root scene

        if(_heldObject != null)
        {
            _heldObject.Dropped();
        }

        Rigidbody grabbableRB = _interactableObject.GetComponent<Rigidbody>();

        if (grabbableRB != null)
        {
            grabbableRB.useGravity = true;
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
