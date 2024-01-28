using System;
using System.Collections;
using System.Collections.Generic;
using CommandTerminal;
using UnityEngine;

public class MultiPostProcessVolumeController : MonoBehaviour
{
    public PostProcessVolumeController[] allControllers;

    private void Start()
    {
        
        Terminal.Shell.AddCommand("zoneout", ZoneOut, 0, 999, "Pay Attention");   
        Terminal.Shell.AddCommand("warp", Warp, 0, 999, "You're fast, but not really");
        Terminal.Shell.AddCommand("whiteout", WhiteOut, 0, 999, "Who turn on the lights?");   
        Terminal.Shell.AddCommand("blackout", BlackOut, 0, 999, "Who turn off the lights?");   
        Terminal.Shell.AddCommand("bloom", ShowBloom, 0, 999, "That's bright");   
        Terminal.Shell.AddCommand("spots", Spots, 0, 999, "Looked into the sun too much");   
        Terminal.Shell.AddCommand("threed", Aberration, 0, 999, "Looks 2D to me");   
        Terminal.Shell.AddCommand("noglasses", NoGlasses, 0, 999, "Blind without them");   
        
    }

    private void NoGlasses(CommandArg[] obj)
    {
        RunControl(7);
    }

    private void Aberration(CommandArg[] obj)
    {
        RunControl(6);

    }


    private void Spots(CommandArg[] obj)
    {
        RunControl(5);

    }

    private void ShowBloom(CommandArg[] obj)
    {
        RunControl(4);

    }

    private void BlackOut(CommandArg[] obj)
    {
        RunControl(3);

    }

    private void WhiteOut(CommandArg[] obj)
    {
        RunControl(2);

    }

    private void Warp(CommandArg[] obj)
    {
        RunControl(1);

    }

    private void ZoneOut(CommandArg[] obj)
    {
        RunControl(0);
    }


    private void RunControl(int index)
    {
        if (index < 0)
        {
            return;
        }

        if (index >= allControllers.Length)
        {
            return;
        }
        
        allControllers[index].Run();
    }
    
    
}
