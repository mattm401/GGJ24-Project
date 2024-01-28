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
        GameManager.Instance.TurnOnMiniGame();
    }

    public override void ReleaseBrain()
    {
        base.ReleaseBrain();
    }

    public void ReLaunchMiniGame()
    {
        if (_latestBrain != null)
        {
            GameManager.Instance.TurnOnMiniGame();
        }
    }

    protected override void ManipulateBrainPosition()
    {
        if (_latestBrain != null)
        {
            _latestBrain.transform.position = BrainHoldTransform.position;
            _latestBrain.transform.rotation = RotationReference.rotation;
            _latestBrain.BrainRB.constraints = RigidbodyConstraints.FreezeAll;
            
            //Quaternion difference = Quaternion.Inverse(_latestBrain.transform.rotation) * RotationReference.rotation;

            //// Convert the difference to Euler angles
            //Vector3 differenceEulerAngles = difference.eulerAngles;

            //if (Vector3.Distance(differenceEulerAngles, Vector3.one) > 1)
            //{
            //    _latestBrain.transform.rotation = Quaternion.RotateTowards(_latestBrain.transform.rotation, RotationReference.rotation, Time.deltaTime * RotationSpeed);
            //}
            //else
            //{
            //    _latestBrain.transform.rotation = RotationReference.rotation;
            //}
        }
    }
}
