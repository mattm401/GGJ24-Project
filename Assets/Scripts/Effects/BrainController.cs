using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainController : MonoBehaviour, IGrabbable
{
    public Rigidbody BrainRB;
    public float DebugBounceForce = 10f;
    public bool DebugBounceTest = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && DebugBounceTest)
        {
            BrainRB.AddForce(Vector3.up * DebugBounceForce);
        }
    }
}
