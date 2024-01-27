using System;
using System.Collections;
using System.Collections.Generic;
using CommandTerminal;
using NaughtyAttributes;
using UnityEngine;

public class FakeTerminal : MonoBehaviour
{
    private Terminal _terminal;

    public string stringCommand = "";

    [Button]
    private void FireStringCommand()
    {
        _terminal.EnterCommand(stringCommand);
    }

    private void Awake()
    {
        _terminal = FindFirstObjectByType<Terminal>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
