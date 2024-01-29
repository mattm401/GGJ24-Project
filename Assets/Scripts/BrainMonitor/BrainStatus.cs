using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainStatus : MonoBehaviour
{
    public Assets.Scripts.LockMiniGame.LockObject Node1;
    public Assets.Scripts.LockMiniGame.LockObject Node2;
    public Assets.Scripts.LockMiniGame.LockObject Node3;
    public Assets.Scripts.LockMiniGame.LockObject Node4;
    private float _node1Level;
    private float _node2Level;

    private float _node3Level;
    private float _node4Level;
    private float _nodeIntegrity;

    private float _overallStability;
    private float _joyLevel;
    private float _terrorLevel;
    private float _angerLevel;
    private float _confusionLevel;
    private bool _integrityDropping;
    private Coroutine _decayRoutine;
    public bool TrackBrain;
    public BrainStatus()
    {
        _node1Level = .5f;
        _node2Level = .5f;
        _node3Level = .7f;
        _node4Level = .7f;
        _nodeIntegrity = 1;

        CalculateEmotionalLevels(); 
    }

    public void SetBrain()
    {
        _node1Level = Node1.GetCurrentLevel();
        _node2Level = Node2.GetCurrentLevel();
        _node3Level = Node3.GetCurrentLevel();
        _node4Level = Node4.GetCurrentLevel();
        CalculateEmotionalLevels();
    }

    private void CalculateEmotionalLevels()
    {

        float joyFactor = (_node1Level + _node2Level + _node3Level + _node4Level) / 4;
        float terrorFactor =_nodeIntegrity-((_node1Level+_node2Level+_node3Level))/4;
        float angerfactor = _nodeIntegrity - ((_node4Level + _node2Level + _node3Level)) / 4;
        float confusionFactor = _nodeIntegrity - ((_node4Level + _node1Level + _node3Level)) / 4;

        _joyLevel = joyFactor;// / totalPoints;
        _terrorLevel = terrorFactor;/// totalPoints;
        _angerLevel = angerfactor;/// totalPoints;
        _confusionLevel = confusionFactor;/// totalPoints;
            
        _overallStability = ((_node1Level + _node2Level + _node3Level + _node4Level)/4) * _nodeIntegrity; 

        if(_overallStability<=0)
        {
            Node1.setLocked(true);
            Node2.setLocked(true);
            Node3.setLocked(true);
            Node4.setLocked(true);
        }
    }


    public void ChangeGoo(float change)
    {
        _node3Level = change > 1 ? 1 : change < 0 ? 0 : change;
        CalculateEmotionalLevels();
    }

    public void ChangeNode(float change)
    {
        _nodeIntegrity = change > 1 ? 1 : change < 0 ? 0 : change;
        CalculateEmotionalLevels();
    }

    public void ChangeConductivity(float change)
    {
        _node4Level = change > 1 ? 1 : change < 0 ? 0 : change;
        CalculateEmotionalLevels();
    }

    public void ChangeEye(float change)
    {
        _node1Level = change > 1 ? 1 : change < 0 ? 0 : change;
        CalculateEmotionalLevels();
    }

    public void ChangeMouth(float change)
    {
        _node2Level = change > 1 ? 1 : change < 0 ? 0 : change;
        CalculateEmotionalLevels();
    }

    public float GetEye()
    {
        return _node1Level;
    }

    public float GetMouth()
    {
        return _node2Level;
    }

    public float GetGoo()
    {
        return _node3Level;
    }

    public float GetNode()
    {
        return _nodeIntegrity;
    }

    public float GetConduct()
    {
        return _node4Level;
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

    public void BeginDroppingIntegrity()
    {
        if(!_integrityDropping)
        {
            _decayRoutine = StartCoroutine(DropIntegrity());
        }
        
    }

    public void EndDroppingIntegrity()
    {
        _integrityDropping= false;
        if(_decayRoutine!=null)
        {
            StopCoroutine(_decayRoutine);
        }
    }

    public IEnumerator DropIntegrity()
    {
        _integrityDropping = true;
        while (_integrityDropping&&_overallStability>0)
        {
            yield return new WaitForSeconds(1);
            _nodeIntegrity -= 0.01f;
            SetBrain();
        }
    }
}
