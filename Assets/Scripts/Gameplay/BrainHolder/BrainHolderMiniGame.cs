using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainHolderMiniGame : BrainHolder
{
    public bool DebugTestDisablePlayerWhenHoldingBrain;
    public float RotationSpeed = 50f;
    public Transform RotationReference;
    protected override void PickUpBrain()
    {
        base.PickUpBrain();


        if (DebugTestDisablePlayerWhenHoldingBrain)
        {
            GameManager.Instance.SetPlayerEnabled(false);
        }
    }

    protected override void ManipulateBrainPosition()
    {
        if (_latestBrain != null)
        {
            _latestBrain.transform.position = BrainHoldTransform.position;
            
            Quaternion difference = Quaternion.Inverse(_latestBrain.transform.rotation) * RotationReference.rotation;

            // Convert the difference to Euler angles
            Vector3 differenceEulerAngles = difference.eulerAngles;

            if (Vector3.Distance(differenceEulerAngles, Vector3.one) > 1)
            {
                _latestBrain.transform.rotation = Quaternion.RotateTowards(_latestBrain.transform.rotation, RotationReference.rotation, Time.deltaTime * RotationSpeed);
            }
            else
            {
                _latestBrain.transform.rotation = RotationReference.rotation;
            }
        }
    }
}
