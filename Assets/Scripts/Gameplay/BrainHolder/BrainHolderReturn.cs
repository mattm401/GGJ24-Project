using Assets.Scripts.LockMiniGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainHolderReturn : BrainHolder
{
    public Transform BrainTerminationPoint;
    public float BrainTravelSpeed = 1f;

    protected override void ManipulateBrainPosition()
    {
        if (_latestBrain != null)
        {
            _latestBrain.transform.position = Vector3.MoveTowards(_latestBrain.transform.position, BrainTerminationPoint.position, Time.deltaTime * BrainTravelSpeed);

            if (Vector3.Distance(_latestBrain.transform.position, BrainTerminationPoint.position) < 1f)
            {
                RegisterBrain();

                Destroy(_latestBrain.gameObject);

                _manipulatingBrain = false;
                GameManager.Instance.SetResetNeeded(true);
            }
        }
    }

    private void RegisterBrain()
    {
        _latestBrain.RegisterRating();
    }
}
