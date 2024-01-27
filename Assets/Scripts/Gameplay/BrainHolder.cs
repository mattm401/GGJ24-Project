using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BrainHolder : MonoBehaviour
{
    public Transform BrainHoldTransform;
    private bool _holdingBrain;
    private BrainController _latestBrain;
    public float BrainLaunchForce = 100f;
    public Color coneColor = Color.yellow;

    private void Update()
    {
        if(_holdingBrain)
        {
            HoldBrainInPlace();
        }

        if (_latestBrain != null && !_latestBrain.BeingCarried)
        {
            Debug.Log("Holder grabbing brain");
            _holdingBrain = true;
            _latestBrain.PickedUp();
        }
    }

    private void HoldBrainInPlace()
    {
        if (_latestBrain != null)
        {
            _latestBrain.transform.position = BrainHoldTransform.position;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Interactable"))
        {
            var brainController = collision.gameObject.GetComponent<BrainController>();

            if (brainController != null)
            {
                _latestBrain = brainController;
            }
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (_latestBrain != null && collision.gameObject == _latestBrain.gameObject)
        {
            _latestBrain = null;
        }
    }

    public void ReleaseBrain()
    {
        if(_latestBrain == null)
        {
            //FAIL SOUND
        }
        else
        {
            Debug.Log("Holder releasing brain");
            var rb = _latestBrain.GetComponent<Rigidbody>();
            if(rb != null)
            {
                Vector3 pos = transform.position;
                var dir = (pos - RandomPointInCone(Vector3.forward)).normalized;
                rb.AddForce(dir * BrainLaunchForce);
            }

            _latestBrain.Dropped();

            _latestBrain = null;
            _holdingBrain = false;
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
