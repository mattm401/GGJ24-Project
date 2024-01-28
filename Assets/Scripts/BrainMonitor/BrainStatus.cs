using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainStatus : MonoBehaviour
{
    private float _eyeSlider;
    private float _mouthSlider;

    private float _gooLevel;
    private float _conductivity;
    private float _nodeIntegrity;

    private float _overallStability;
    private float _joyLevel;
    private float _terrorLevel;
    private float _angerLevel;
    private float _confusionLevel;

    public BrainStatus()
    {
        _eyeSlider = .5f;
        _mouthSlider = .5f;
        _gooLevel = 1;
        _conductivity = 1;
        _nodeIntegrity = 1;

        CalculateEmotionalLevels(); 
    }

    private void CalculateEmotionalLevels()
    {

        float joyFactor = _overallStability;
        float terrorFactor = _eyeSlider * (1 - _mouthSlider) * (1- _gooLevel)*3;
        float angerfactor = (1 - _eyeSlider) * (1 - _mouthSlider) * (1- _nodeIntegrity)*3;
        float confusionFactor = _eyeSlider * _mouthSlider * (1-_conductivity)*3;

        float totalPoints = joyFactor + terrorFactor + angerfactor + confusionFactor;

        _joyLevel = joyFactor;// / totalPoints;
        _terrorLevel = terrorFactor;/// totalPoints;
        _angerLevel = angerfactor;/// totalPoints;
        _confusionLevel = confusionFactor;/// totalPoints;
            
        _overallStability = (_gooLevel + _conductivity + _nodeIntegrity) / 3;
    }

    public void ChangeGoo(float change)
    {
        _gooLevel = change > 1 ? 1 : change < 0 ? 0 : change;
        CalculateEmotionalLevels();
    }

    public void ChangeNode(float change)
    {
        _nodeIntegrity = change > 1 ? 1 : change < 0 ? 0 : change;
        CalculateEmotionalLevels();
    }

    public void ChangeConductivity(float change)
    {
        _conductivity = change > 1 ? 1 : change < 0 ? 0 : change;
        CalculateEmotionalLevels();
    }

    public void ChangeEye(float change)
    {
        _eyeSlider = change > 1 ? 1 : change < 0 ? 0 : change;
        CalculateEmotionalLevels();
    }

    public void ChangeMouth(float change)
    {
        _mouthSlider = change > 1 ? 1 : change < 0 ? 0 : change;
        CalculateEmotionalLevels();
    }

    public float GetEye()
    {
        return _eyeSlider;
    }

    public float GetMouth()
    {
        return _mouthSlider;
    }

    public float GetGoo()
    {
        return _gooLevel;
    }

    public float GetNode()
    {
        return _nodeIntegrity;
    }

    public float GetConduct()
    {
        return _conductivity;
    }


    public float GetJoy()
    {
        return _joyLevel;
    }

    public float GetTerror()
    {
        return _terrorLevel;
    }

    public float GetAnger()
    {
        return _angerLevel;
    }

    public float GetConfusion()
    {
        return _confusionLevel;
    }

    public float GetOverallStability()
    {
        return _overallStability;
    }

    public int HealthLevel()
    {
        if(_overallStability>.66f)
        {
            return 0;
        }

        if(_overallStability>.33f)
        {
            return 1;
        }

        return 2;
    }
}
