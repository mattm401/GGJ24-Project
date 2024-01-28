using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainController : MonoBehaviour, IGrabbable
{
    public Collider Collider;
    public Rigidbody BrainRB;
    public bool _debug = false;

    private float MaxHealth = 4f;
    private float Health = 0f;
    public bool BeingCarried;
    public GameObject BloodDecalReference;
    public bool DebugRandomHealth;
    public AudioSource ImpactSound;

    public float Node1Score, Node2Score, Node3Score, Node4Score;
    public bool Node1Works, Node2Works, Node3Works, Node4Works = true;

    void Start()
    {
        Node1Score = Random.Range(0.4f, 0.7f);
        Node2Score = Random.Range(0.4f, 0.7f);
        Node3Score = Random.Range(0.4f, 0.7f);
        Node4Score = Random.Range(0.4f, 0.7f);
    }

    public void RegisterRating()
    {
        Health = Node1Score + Node2Score + Node3Score + Node4Score;

        if (Health > MaxHealth)
        {
            Health = MaxHealth; 
        }

        GameManager.Instance.RegisterBrainScore(Health / MaxHealth);
    }

    public void PickedUp()
    {
        BeingCarried = true;
        //Collider.enabled = false;
        BrainRB.useGravity = false;
        if(_debug)Debug.Log("Brain picked up");
    }

    public void Dropped()
    {
        BeingCarried = false;
        //Collider.enabled = true;
        BrainRB.useGravity = true;
        BrainRB.constraints = RigidbodyConstraints.None;
        if(_debug)Debug.Log("Brain dropped");
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
