using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainController : MonoBehaviour, IGrabbable
{
    public Collider Collider;
    public Rigidbody BrainRB;

    public float MaxHealth = 10f;
    public float Health = 1f;
    public bool BeingCarried;
    public GameObject BloodDecalReference;
    public bool DebugRandomHealth;
    public AudioSource ImpactSound;

    public void RegisterRating()
    {
        if (DebugRandomHealth)
        {
            Health = Random.Range(1, MaxHealth);
        }

        GameManager.Instance.RegisterBrainScore(Health / MaxHealth);
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
        BrainRB.constraints = RigidbodyConstraints.None;
        Debug.Log("Brain dropped");
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0]; // Get the first contact point

        Vector3 contactPoint = contact.point;
        Vector3 contactNormal = contact.normal;

        var bloodDecal = Instantiate(BloodDecalReference);
        bloodDecal.transform.position = contactPoint + Vector3.up;
        ImpactSound.pitch = Random.Range(1f, 2f);
        ImpactSound.Play();
        //bloodDecal.transform.LookAt(contactNormal);
    }
}
