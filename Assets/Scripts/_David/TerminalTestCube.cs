using System;
using System.Collections;
using System.Collections.Generic;
using CommandTerminal;
using Unity.VisualScripting;
using UnityEngine;

public class TerminalTestCube : MonoBehaviour
{

    private Rigidbody _rigidbody;
    
    void CommandAdd(CommandArg[] args) {
        int a = args[0].Int;
        int b = args[1].Int;
    
        if (Terminal.IssuedError) return; // Error will be handled by Terminal
    
        int result = a + b;
        Terminal.Log("{0} + {1} = {2}", a, b, result);

        Debug.Log(this.gameObject.name);

    }

    private void Awake()
    {
        _rigidbody = this.GetComponent<Rigidbody>();

    }


    //
    // Start is called before the first frame update
    void Start()
    {
        
        Terminal.Shell.AddCommand("add", CommandAdd, 2, 2, "Adds 2 numbers");   
        Terminal.Shell.AddCommand("spin", SpinCube, 0, 3, "Spin the cube. arg x y z");   

    }


    void SpinCube(CommandArg[] args)
    {
        float x = 0.0f;
        float y = 0.0f;
        float z = 0.0f;

        if (args.Length > 0)
        {
            x = args[0].Float;
        }

        if (args.Length > 1)
        {
            y = args[1].Float;
        }

        if (args.Length > 2)
        {
            z = args[2].Float;
        }
        
        
        Vector3 angularVelocity = new Vector3(x, y, z);
        _rigidbody.angularVelocity = angularVelocity;
        
    }
    
    
    // Update is called once per frame
    void Update()
    {
        
    }
    
    
}
