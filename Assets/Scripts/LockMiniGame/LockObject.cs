using UnityEngine;


public class LockObject : MonoBehaviour
{
    private bool _Locked;
    private float currentLevel;
    private float xRange;
    private float yRange;
    private Vector3 origin_pos;
    private Quaternion origin_rot;

    // Start is called before the first frame update
    void Start()
    {
        currentLevel = Random.Range(0.4f, 0.7f);
        xRange = Random.Range(-65.0f, 65.0f);
        yRange = Random.Range(-65.0f, 65.0f);
        origin_pos = transform.position;
        origin_rot = transform.rotation;

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

    public void ResetGame()
    {
        currentLevel = Random.Range(0.4f, 0.7f);
        xRange = Random.Range(-65.0f, 65.0f);
        yRange = Random.Range(-65.0f, 65.0f);
        transform.position = origin_pos;
        transform.rotation = origin_rot;
    }

}
