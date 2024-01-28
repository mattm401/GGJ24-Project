using System;
using System.Collections;
using System.Collections.Generic;
using CommandTerminal;
using UnityEngine;

[RequireComponent(typeof(FirstPersonController))]
public class TwitchCameraControl : MonoBehaviour
{
    private FirstPersonController _firstPersonController = null;

    
    private Coroutine coroutineInverseRotation;
    public float howLongToInverse = 10.0f;

    
    private Coroutine coroutineSpeedChange;
    private float walkSpeedDefault = 5.0f;
    public float walkSpeedUp = 7.0f;
    public float walkSpeedDown = 1.0f;
    public float howLongSpeedChange = 10.0f;


    private Coroutine coroutineHeadBob;
    public float howLongDrunk = 10.0f;
    private Vector3 headBobDefault;
    public Vector3 headBobChange = new Vector3(.5f, 0f, 0f);
    
    
    private void Awake()
    {
        _firstPersonController = GetComponent<FirstPersonController>();

        walkSpeedDefault = _firstPersonController.walkSpeed;

        headBobDefault = _firstPersonController.bobAmount;
    }

    // Start is called before the first frame update
    void Start()
    {
        //set up stuff.
        Terminal.Shell.AddCommand("invert", InverseRotation, 0, 999, "Inverse Player's Rotation");   
        Terminal.Shell.AddCommand("speedup", SpeedUp, 0, 999, "Max Player's Speed");   
        Terminal.Shell.AddCommand("speeddown", SpeedDown, 0, 999, "Min Player's Speed");   
        Terminal.Shell.AddCommand("drunk", DoDrunk, 0, 999, "Player Sway when moving");   

    }

    private void DoDrunk(CommandArg[] obj)
    {
        if (coroutineHeadBob == null)
        {
            coroutineHeadBob = StartCoroutine(CoDrunk());
        }
    }

    private IEnumerator CoDrunk()
    {
        _firstPersonController.bobAmount = headBobChange;
        yield return new WaitForSeconds(howLongDrunk);
        _firstPersonController.bobAmount = headBobDefault;
        coroutineHeadBob = null;
    }

    private void SpeedDown(CommandArg[] obj)
    {
        if (coroutineSpeedChange == null)
        {
            coroutineSpeedChange = StartCoroutine(CoSpeedChange(walkSpeedDown));
        }
    }

    private void SpeedUp(CommandArg[] obj)
    {
        if (coroutineSpeedChange == null)
        {
            coroutineSpeedChange = StartCoroutine(CoSpeedChange(walkSpeedUp));
        }
        
        //throw new NotImplementedException();
    }

    private IEnumerator CoSpeedChange(float newSpeed)
    {
        _firstPersonController.walkSpeed = newSpeed;
        yield return new WaitForSeconds(howLongSpeedChange);
        _firstPersonController.walkSpeed = walkSpeedDefault;
        coroutineSpeedChange = null;

    }

    private void InverseRotation(CommandArg[] obj)
    {
        if (coroutineInverseRotation == null)
        {
            coroutineInverseRotation = StartCoroutine(CoInverseRotation());
        }

    }

    private IEnumerator CoInverseRotation()
    {
        _firstPersonController.invertCamera = true;

        yield return new WaitForSeconds(howLongToInverse);

        _firstPersonController.invertCamera = false;

        coroutineInverseRotation = null;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
