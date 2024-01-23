using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EKGController : MonoBehaviour
{
    public LineRenderer EKGLine;
    public AnimationCurve EKGCurve;

    [Range(10, 1000)]
    public int Complexity;

    public float Speed = 1;
    public float Magnitude = 1000;
    public float Frequency = 1;
    public float HorizontalLength = 1;
    [Range(0f, 1f)]
    public float Offset;

    // Start is called before the first frame update
    void Start()
    {
    }

    int posx, posy;
    Vector3 _position;

    // Update is called once per frame
    void Update()
    {
        SetComplexity();

        for(int i = 0; i < EKGLine.positionCount ; i++)
        {
            float time = ((float)i / EKGLine.positionCount) + Offset;

            if(time > 1)
            {
                time -= 1;
            }


            float posx = i * HorizontalLength;
            float posy = EKGCurve.Evaluate(time) * Magnitude;

            _position = new Vector3(posx, posy, 0);
            EKGLine.SetPosition(i , _position);
        }
    }

    void SetComplexity()
    {
        if (EKGLine.positionCount != Complexity)
        {
            EKGLine.positionCount = Complexity;
        }
    }
}
