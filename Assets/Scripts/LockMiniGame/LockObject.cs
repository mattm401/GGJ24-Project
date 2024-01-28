using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class LockObject : MonoBehaviour
{
    private bool _Locked;
    private float currentLevel;
    private float xRange;
    private float yRange;

    // Start is called before the first frame update
    void Start()
    {
        currentLevel = Random.Range(0.4f, 0.7f);
        xRange = Random.Range(-65.0f, 65.0f);
        yRange = Random.Range(-65.0f, 65.0f);

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

    public void setCurrentLevel(float level)
    {
        currentLevel = level;
    }

    public (float, float) GetXandY()
    {
        return (xRange, yRange);
    }

}
