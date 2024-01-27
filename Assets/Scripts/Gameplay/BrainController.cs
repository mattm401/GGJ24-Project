using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainController : MonoBehaviour, IGrabbable
{
    public Collider Collider;
    public Rigidbody BrainRB;
    public float DebugBounceForce = 10f;
    public bool DebugBounceTest = false;

    public float MaxHealth = 10f;
    public float Health = 1f;
    public bool BeingCarried;
    public GameObject BloodDecalReference;

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

    public void SetHealth(float health)
    {
        Health = health;

        if(Health > MaxHealth)
        {
            Health = MaxHealth;
        }
    }

    public void PickedUp()
    {
        BeingCarried = true;
        //Collider.enabled = false;
        BrainRB.useGravity = false;
        Debug.Log("Brain picked up");
    }

    public void Dropped()
    {
        BeingCarried = false;
        //Collider.enabled = true;
        BrainRB.useGravity = true;
        Debug.Log("Brain dropped");
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0]; // Get the first contact point

        Vector3 contactPoint = contact.point;
        Vector3 contactNormal = contact.normal;

        var bloodDecal = Instantiate(BloodDecalReference);
        bloodDecal.transform.position = contactPoint + Vector3.up;
        //bloodDecal.transform.LookAt(contactNormal);
    }
}
