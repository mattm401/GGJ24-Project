using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Television_Behaviors : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayStatic()
    {
        GetComponent<AudioSource>().Play();
    }
}
