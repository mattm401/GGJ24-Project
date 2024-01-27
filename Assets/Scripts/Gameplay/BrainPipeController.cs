using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainPipeController : MonoBehaviour
{

    public GameObject BrainObjectReference;
    public Transform BrainSpawnLocation;

    public void CreateBrain()
    {
        GameObject newBrain = GameObject.Instantiate(BrainObjectReference);
        newBrain.transform.position = BrainSpawnLocation.transform.position;
    }

}
