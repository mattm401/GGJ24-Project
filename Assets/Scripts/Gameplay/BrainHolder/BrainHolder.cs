using UnityEngine;

public class BrainHolder : MonoBehaviour
{
    public Collider BrainHolderCollider;
    public Transform BrainHoldTransform;
    protected bool _manipulatingBrain;
    protected BrainController _latestBrain;
    public float BrainLaunchForce = 100f;
    public Color coneColor = Color.yellow;
    public PlayerInteraction PlayerInteraction;
    public string InteractionText = "PLACE BRAIN";

    private void Update()
    {
        if(_manipulatingBrain)
        {
            ManipulateBrainPosition();
        }

        if (_latestBrain != null && !_latestBrain.BeingCarried)
        {
            PickUpBrain();
        }
    }

    protected virtual void PickUpBrain()
    {
        _latestBrain.BrainRB.velocity = Vector3.zero;
        Debug.Log("Holder grabbing brain");
        _manipulatingBrain = true;
        _latestBrain.PickedUp();
        DisableInteractionText();
        //BrainHolderCollider.enabled = false;
    }
    public virtual void ReleaseBrain()
    {
        if (_latestBrain == null)
        {
            //FAIL SOUND
        }
        else
        {
            Debug.Log("Holder releasing brain");
            var rb = _latestBrain.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 pos = transform.position;
                var dir = (pos - RandomPointInCone(Vector3.forward)).normalized;
                rb.AddForce(dir * BrainLaunchForce);
            }

            _latestBrain.Dropped();

            _latestBrain = null;
            _manipulatingBrain = false;
            //BrainHolderCollider.enabled = true;
        }
    }

    protected virtual void ManipulateBrainPosition()
    {
        if (_latestBrain != null)
        {
            _latestBrain.transform.position = BrainHoldTransform.position;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (_manipulatingBrain) return;

        if (collision.gameObject.CompareTag("Interactable"))
        {
            var brainController = collision.gameObject.GetComponent<BrainController>();

            if (brainController != null)
            {
                _latestBrain = brainController;

                if(PlayerInteraction != null)
                {
                    PlayerInteraction.SetCanInteract(true, InteractionText);
                }
            }
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (!_manipulatingBrain && _latestBrain != null && collision.gameObject == _latestBrain.gameObject)
        {
            _latestBrain = null;
            DisableInteractionText();
        }
    }

    private void DisableInteractionText()
    {
        if (PlayerInteraction != null)
        {
            PlayerInteraction.SetCanInteract(false);
        }
    }


    // Calculate random direction within a cone
    Vector3 RandomPointInCone(Vector3 coneDirection)
    {
        Vector3 spread = Random.insideUnitSphere;
        return coneDirection + spread;
    }


    //void OnDrawGizmosSelected()
    //{
    //    // Set the color of the cone wireframe
    //    Gizmos.color = coneColor;

    //    // Draw the cone
    //    Gizmos.DrawRay(transform.position, RandomDirectionInCone(transform.forward));
    //}

}
