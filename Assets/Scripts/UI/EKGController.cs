using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EKGController : MonoBehaviour
{
    public LineRenderer EKGLine;
    public AnimationCurve EKGCurve;
    public AnimationCurve EKGCurve_Unhealthy;

    [Range(10, 1000)]
    public int Complexity;

    public float Speed = 1;
    public float Magnitude = 200;
    public float Frequency = 1;
    [Range(0f, 1f)]
    public float Offset;

    [Range(0f, 10f)]
    public float EKGHealth;
    public float EKGMaxHealth = 10;

    public Camera UICamera;
    public RectTransform EKGUIStart;
    public RectTransform EKGUIEnd;

    //Private
    private Vector3 _position;

    Vector3 _lineStartPos;
    Vector3 _lineEndPos;

    // Start is called before the first frame update
    void Start()
    {
    }


    // Update is called once per frame
    void Update()
    {
        _lineStartPos = Util.GetRectWorldPosition(EKGUIStart);
        _lineEndPos = Util.GetRectWorldPosition(EKGUIEnd);

        SetComplexity();

        Offset += Time.deltaTime * Speed;
        if(Offset > 1)
        {
            Offset = 0;
        }


        for(int i = 0; i < EKGLine.positionCount ; i++)
        {
            float time = ((float)i / EKGLine.positionCount) + Offset;

            if(time > 1)
            {
                time -= 1;
            }


            float posx = Mathf.Lerp(_lineStartPos.x, _lineEndPos.x, (float)i / EKGLine.positionCount);
            float healthyPosY = EKGCurve.Evaluate(time) * Magnitude;
            float unhealthyPosY = EKGCurve_Unhealthy.Evaluate(time) * Magnitude;
            float posY = _lineStartPos.y + Mathf.Lerp(healthyPosY, unhealthyPosY, EKGHealth / EKGMaxHealth);

            _position = new Vector3(posx, posY, 0);
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
