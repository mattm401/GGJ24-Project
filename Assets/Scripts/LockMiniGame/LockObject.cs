using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class LockObject : MonoBehaviour
{
    private bool _Locked;
    private float currentLevel;

    // Start is called before the first frame update
    void Start()
    {
        currentLevel = Random.Range(0.0f, 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setLocked(bool locked)
    {
        _Locked = locked;
    }

    public float getCurrentLevel()
    {
        return currentLevel;
    }
}
